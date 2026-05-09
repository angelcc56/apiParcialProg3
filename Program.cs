using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// 1. Extraer la cadena de conexión del appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// 2. Registrar el DbContext con Pomelo MySQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString) // Esto detecta si es MySQL 8.0, 5.7, etc.
    )
);

builder.Services.AddCors(options => {
    options.AddDefaultPolicy(policy => {
        // Para desarrollo pueden usar AllowAnyOrigin()
        // Para producción, especifiquen su URL de Azure: .WithOrigins("https://mi-sitio.azurewebsites.net")
        policy.AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.MapOpenApi();
    app.MapScalarApiReference(); // Accedes vía /scalar/v1
}

app.UseAuthorization();

app.UseCors();

app.MapControllers();

app.Run();