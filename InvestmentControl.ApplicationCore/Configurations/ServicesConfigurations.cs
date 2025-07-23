using InvestmentControl.ApplicationCore.Services;
using InvestmentControl.Domain.Models.Abstractions.Repositories;
using InvestmentControl.Infrastructure.Configurations.Context;
using InvestmentControl.Infrastructure.Repositories;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace InvestmentControl.ApplicationCore.Configurations;

public static class ServicesConfigurations
{
    public static IServiceCollection AddInvestmentControlServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddBankDbContext(configuration);
        services.AddSwagger();
        services.AddCorsConfig();

        services.AddScoped<OperationService>();
        services.AddScoped<PositionService>();
        services.AddScoped<AssetService>();

        services.AddScoped<IAssetRepository, AssetRepository>();
        services.AddScoped<IOperationRepository, OperationRepository>();
        services.AddScoped<IPositionRepository, PositionRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }

    private static void AddSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "Controle de Investimentos"
            });

            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        });
    }

    #pragma warning disable CS1591
    public static string CorsAllowAll = "AllowAll";

    public static string CorsAllowSpecific = "AllowSpecific";
    #pragma warning restore CS1591
    private static void AddCorsConfig(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(CorsAllowAll, policy =>
            {
                policy.AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });

            options.AddPolicy(CorsAllowSpecific, policy =>
            {
                policy.WithOrigins("http://localhost:3000")
                      //"https://urlprod.com")
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            });
        });
    }
}
