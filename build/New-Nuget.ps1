[CmdletBinding()]
param (
    [Parameter(Mandatory = $true)]
    [ValidateNotNullOrWhiteSpace()]
    [ValidateSet("Dzaba.AspNetUtils", "Dzaba.IntegrationTestUtils", "Dzaba.TestUtils", "Dzaba.Utils", "Dzaba.Utils.Configuration")]
    [string]$PackageName,

    [Parameter()]
    [switch]$PreRelease,

    [Parameter()]
    [switch]$UpgradeMajor,

    [Parameter()]
    [switch]$UpgradeBuild
)

function Get-LatestNugetVersion {
    [OutputType([string])]
    param (
        [Parameter(Mandatory = $true)]
        [ValidateNotNullOrWhiteSpace()] 
        [string]$PackageName,

        [Parameter()]
        [switch]$PreRelease
    )
    
    $packageNameLower = $PackageName.ToLowerInvariant()
    # NuGet.org API endpoint for package versions
    $apiUrl = "https://api.nuget.org/v3-flatcontainer/$($packageNameLower)/index.json"
    
    try {
        # Fetch available versions from NuGet.org
        $response = Invoke-RestMethod -Uri $apiUrl -Method Get -ErrorAction Stop
        
        # Get all version numbers from the response
        $versions = @($response.versions | Where-Object { $_ -match '^\d' })  # Filter to numeric versions only
        
        if ($versions.Count -eq 0) {
            throw "No versions found for package: $PackageName"
        }
        
        # Sort versions in descending order (newest first)
        $versions = @($versions | Sort-Object -Descending)
        
        # If PreRelease switch is not checked, filter out pre-release versions
        if (-not $PreRelease) {
            $versions = @($versions | Where-Object { 
                $_ -notmatch '-alpha|-beta|-preview|-rc|-canary'
            })
        }
        
        # Return the latest version
        return $versions[0]
        
    }
    catch {
        Write-Error "Failed to get latest version for $PackageName" -ErrorAction Continue
        throw
    }
}

$ErrorActionPreference = 'Stop'
$scriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
$rootPath = Join-Path $scriptPath ".." -Resolve
Push-Location $rootPath
try {
    $outputPath = Join-Path $rootPath "nugets"
    $latestVersion = Get-LatestNugetVersion -PackageName $PackageName -PreRelease:$PreRelease
    Write-Host "Latest version for $PackageName is: $latestVersion"

    if (-not $PreRelease) {
        $versionParsed = [version]$latestVersion
        $versionArray = @($versionParsed.Major, $versionParsed.Minor, $versionParsed.Build)
    }

    if ($UpgradeMajor) {
        $nextVersion = @(($versionArray[0] + 1), $versionArray[1], $versionArray[2])
    }
    elseif ($UpgradeBuild) {
        $nextVersion = @($versionArray[0], $versionArray[1], ($versionArray[2] + 1))
    }
    else {
        $nextVersion = @($versionArray[0], ($versionArray[1] + 1), $versionArray[2])
    }

    $nextVersionJoined = $nextVersion -join "."
    Write-Host "Next version for $PackageName is: $nextVersionJoined"

    if (-not (Test-Path $outputPath)) {
        New-Item -ItemType Directory -Force -Path $outputPath | Out-Null
    }

    $projectPath = Join-Path $rootPath "src" $PackageName
    dotnet pack $projectPath -o $outputPath -c Release /p:Version=$nextVersionJoined
}
finally {
    Pop-Location
}
