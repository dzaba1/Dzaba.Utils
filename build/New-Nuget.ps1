[CmdletBinding()]
param (
    [Parameter(Mandatory = $true)]
    [ValidateNotNullOrWhiteSpace()]
    [ValidateSet("Dzaba.Utils.AspNet", "Dzaba.TestUtils.Integration", "Dzaba.TestUtils.Integration.AspNet", "Dzaba.TestUtils", "Dzaba.Utils.Configuration")]
    [string]$PackageName,

    [Parameter()]
    [switch]$PreRelease,

    [Parameter()]
    [switch]$UpgradeMajor,

    [Parameter()]
    [switch]$UpgradeBuild
)

function Get-LatestNugetVersion {
    [OutputType([version])]
    param (
        [Parameter(Mandatory = $true)]
        [ValidateNotNullOrWhiteSpace()]
        [string]$PackageName
    )

    $apiUrl = "https://api.nuget.org/v3-flatcontainer/$($PackageName.ToLowerInvariant())/index.json"

    try {
        $response = Invoke-RestMethod -Uri $apiUrl -Method Get -ErrorAction Stop
    }
    catch [Microsoft.PowerShell.Commands.HttpResponseException] {
        if ($_.Exception.Response.StatusCode -eq [System.Net.HttpStatusCode]::NotFound) {
            # Never published yet.
            return $null
        }
        throw
    }

    # Prerelease versions contain '-'. They never become the base for the next version.
    # Compare as [version], not as strings: "1.10.0" must sort above "1.9.0".
    $latest = @($response.versions) |
        Where-Object { $_ -notmatch '-' } |
        ForEach-Object { [version]$_ } |
        Sort-Object |
        Select-Object -Last 1

    return $latest
}

$ErrorActionPreference = 'Stop'
$scriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
$rootPath = Join-Path $scriptPath ".." -Resolve
Push-Location $rootPath
try {
    $gitStatus = git status --porcelain
    if ($LASTEXITCODE -ne 0) {
        throw "git status failed with exit code $LASTEXITCODE."
    }
    if ($gitStatus) {
        throw "The working tree has uncommitted changes. Commit or stash them before packing, so the package matches a commit.`n$($gitStatus -join "`n")"
    }

    $latestVersion = Get-LatestNugetVersion -PackageName $PackageName
    if ($null -eq $latestVersion) {
        Write-Host "$PackageName is not published yet."
        $nextVersion = [version]"1.0.0"
    }
    else {
        Write-Host "Latest stable version for $PackageName is: $latestVersion"
        if ($UpgradeMajor) {
            $nextVersion = [version]::new($latestVersion.Major + 1, 0, 0)
        }
        elseif ($UpgradeBuild) {
            $nextVersion = [version]::new($latestVersion.Major, $latestVersion.Minor, $latestVersion.Build + 1)
        }
        else {
            $nextVersion = [version]::new($latestVersion.Major, $latestVersion.Minor + 1, 0)
        }
    }

    $nextVersionString = $nextVersion.ToString(3)
    if ($PreRelease) {
        $nextVersionString += "-preview.$((Get-Date).ToUniversalTime().ToString('yyyyMMddHHmm'))"
    }
    Write-Host "Next version for $PackageName is: $nextVersionString"

    dotnet test -c Release
    if ($LASTEXITCODE -ne 0) {
        throw "Tests failed with exit code $LASTEXITCODE."
    }

    $outputPath = Join-Path $rootPath "nugets"
    if (-not (Test-Path $outputPath)) {
        New-Item -ItemType Directory -Force -Path $outputPath | Out-Null
    }

    $projectPath = Join-Path $rootPath "src" $PackageName
    $packArgs = @(
        "pack", $projectPath,
        "-o", $outputPath,
        "-c", "Release",
        "-p:Version=$nextVersionString",
        "-p:ContinuousIntegrationBuild=true"
    )
    # A major bump is allowed to break the API, so only compare against the previous version otherwise.
    if ($null -ne $latestVersion -and -not $UpgradeMajor) {
        $packArgs += "-p:ValidationBaselineProject=$PackageName"
        $packArgs += "-p:ValidationBaselineVersion=$($latestVersion.ToString(3))"
    }

    # Package validation is incremental and the baseline is not one of its inputs, so an earlier local pack would make it skip.
    $validationSemaphore = Join-Path $projectPath "obj" "Release" "Microsoft.NET.ApiCompat.ValidatePackage.semaphore"
    if (Test-Path $validationSemaphore) {
        Remove-Item $validationSemaphore -Force
    }

    dotnet @packArgs
    if ($LASTEXITCODE -ne 0) {
        throw "dotnet pack failed with exit code $LASTEXITCODE."
    }
}
finally {
    Pop-Location
}
