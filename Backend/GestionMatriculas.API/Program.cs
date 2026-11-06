using Microsoft.EntityFrameworkCore;
using GestionMatriculas.API.Data;
using System; // Necesario para Environment

var builder = WebApplication.CreateBuilder(args);

// --- CONFIGURACIÓN PARA RAILWAY (PUERTO DINÁMICO) ---
// Railway asigna un puerto en la variable de entorno PORT. 
// Si no existe, usa el 8080 por defecto.
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

// 2. Configurar CORS (Para que Angular y Vercel puedan conectarse)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
        policy.WithOrigins(
            "http://localhost:4200",
            "https://*.vercel.app" // Permite cualquier subdominio de Vercel
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

// Nota: Comentamos UseHttpsRedirection porque en producción (Railway) 
// el proxy ya maneja el HTTPS. Si lo dejamos, puede causar un bucle infinito de redirección.
// app.UseHttpsRedirection(); 

// 3. Usar CORS (Debe ir antes de Authorization y MapControllers)
app.UseCors("AllowAngular");

app.UseAuthorization();

app.MapControllers();

// --- BLOQUE PARA PRECARGAR DATOS (SEEDING) ---
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // Si no hay colegios, los creamos
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