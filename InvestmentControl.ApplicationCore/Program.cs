using InvestmentControl.ApplicationCore.Configurations;
using InvestmentControl.ApplicationCore.Middlewares;
using InvestmentControl.Infrastructure.Context;
using InvestmentControl.Infrastructure.SeedData;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using NodaTime.Serialization.SystemTextJson;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
builder.Services.AddInvestmentControlServices(builder.Configuration);
builder.Services.AddExceptionHandler<ExceptionHandlerMiddleware>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors(ServicesConfigurations.CorsAllowAll);

    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    try
    {
        var bankDbContext = services.GetRequiredService<BankDbContext>();
        bankDbContext.Database.Migrate();

        var databaseSeeder = services.GetRequiredService<DatabaseSeeder>();
        await databaseSeeder.SeedAsync();

        app.Logger.LogInformation("Database seeded successfully in Development environment.");
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "An error occurred while migrating or seeding the database in Development environment.");
    }
}

app.UseExceptionHandler(o => { });

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
