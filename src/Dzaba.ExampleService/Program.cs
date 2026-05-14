using Dzaba.Utils.AspNet;
using Dzaba.Utils.Configuration;

public partial class Program
{
    private static IServiceProvider Container { get; set;}

    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddSettings<JwtSettings>("Jwt");

        builder.Services.AddJwtAuthentication(() => Container.GetRequiredService<JwtSettings>());

        builder.Services.AddAuthorization();

        builder.Services.AddControllers();
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        Container = app.Services;

        app.Run();
    }
}