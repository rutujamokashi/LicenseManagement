
using LicenseManagement.API.Data;
using LicenseManagement.API.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Register application services using dependency injection
builder.Services.AddScoped<ILicenseService, LicenseService>();
builder.Services.AddSingleton<IEncryptionService, AesEncryptionService>();

// Register Entity Framework Core with SQL Server
builder.Services.AddDbContext<LicenseDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString(
            "LicenseManagementDb")));

// Register health check services for application monitoring
builder.Services.AddHealthChecks();

// Register MVC controllers
builder.Services.AddControllers();

// Register services required for Swagger/OpenAPI documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure CORS policy to allow requests from the React application
builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactPolicy", policy =>
    {
        policy
            .WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

// Enable Swagger UI and OpenAPI documentation
app.UseSwagger();
app.UseSwaggerUI();

// Configure CORS middleware
app.UseCors("ReactPolicy");

// Configure authorization middleware
app.UseAuthorization();

// Expose a health check endpoint for Docker and monitoring systems
app.MapHealthChecks("/health");

// Map API controllers and their routes
app.MapControllers();

app.Run();

