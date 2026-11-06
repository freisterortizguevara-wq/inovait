using Microsoft.EntityFrameworkCore;
using GestionMatriculas.API.Data;
using System;

var builder = WebApplication.CreateBuilder(args);

// --- CONFIGURACIÓN PARA RENDER (PUERTO DINÁMICO) ---
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
// ---------------------------------------------------

// Add services to the container.
builder.Services.AddControllers();

// Configuración de OpenAPI (Swagger)
builder.Services.AddOpenApi();

// 1. Configurar la Base de Datos (PostgreSQL / Neon)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Configurar CORS (Para permitir Angular en Render y Local)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
        policy.WithOrigins(
            "http://localhost:4200",
            "\"https://freisterortizguevara-wq.github.io\"" // <-- Permite cualquier subdominio de Render
        )
        .AllowAnyHeader()
        .AllowAnyMethod());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "v1");
    });
}

// Nota: Comentamos UseHttpsRedirection para evitar bucles de redirección en producción.
// app.UseHttpsRedirection(); 

// 3. Usar CORS
app.UseCors("AllowAngular");

app.UseAuthorization();

app.MapControllers();

// --- BLOQUE PARA PRECARGAR DATOS (SEEDING) ---
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    if (!context.Colegios.Any())
    {
        context.Colegios.AddRange(
            new GestionMatriculas.API.Models.Colegio { Nombre = "Colegio San José", Sector = "Privado" },
            new GestionMatriculas.API.Models.Colegio { Nombre = "I.E. Ciudadela", Sector = "Público" }
        );
        context.Grados.AddRange(
            new GestionMatriculas.API.Models.Grado { Nombre = "Transición" },
            new GestionMatriculas.API.Models.Grado { Nombre = "Primero" },
            new GestionMatriculas.API.Models.Grado { Nombre = "Segundo" }
        );
        context.Docentes.AddRange(
            new GestionMatriculas.API.Models.Docente { Nombre = "Ana Gómez" },
            new GestionMatriculas.API.Models.Docente { Nombre = "Carlos Ruiz" }
        );
        context.SaveChanges();
    }
}
// ---------------------------------------------

app.Run();