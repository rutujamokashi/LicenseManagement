using LicenseManagement.API.Services;
using LicenseManagement.API.Data;
using LicenseManagement.API.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);





// Dependency Injection
builder.Services.AddScoped<ILicenseService, LicenseService>();
builder.Services.AddSingleton<IEncryptionService, AesEncryptionService>();

// Swagger

builder.Services.AddDbContext<LicenseDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString(
            "LicenseManagementDb")));
//CORS Policy

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

// Enable Swagger in Development
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

//app.UseHttpsRedirection();

app.UseCors("ReactPolicy");

app.UseAuthorization();

app.MapControllers();

app.Run();