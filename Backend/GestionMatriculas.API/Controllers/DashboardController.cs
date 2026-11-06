using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestionMatriculas.API.Data;

namespace GestionMatriculas.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("estadisticas")]
        public async Task<IActionResult> ObtenerEstadisticas()
        {
            try
            {
                // 1. Totales básicos
                var totalEstudiantes = await _context.Estudiantes.CountAsync();
                var totalColegios = await _context.Colegios.CountAsync();
                var totalDocentes = await _context.Docentes.CountAsync();

                // 2. Estudiantes por Grado (Gráfica de Barras)
                var estudiantesPorGrado = await _context.Matriculas
                    .Include(m => m.Grado)
                    .GroupBy(m => m.Grado!.Nombre)
                    .Select(g => new { Grado = g.Key, Cantidad = g.Count() })
                    .ToListAsync();

                // 3. Estudiantes por Sector (Gráfica de Dona)
                var estudiantesPorSector = await _context.Estudiantes
                    .Include(e => e.Colegio)
                    .GroupBy(e => e.Colegio!.Sector)
                    .Select(g => new { Sector = g.Key, Cantidad = g.Count() })
                    .ToListAsync();

                // 4. Contar estudiantes por rangos de edad (Pregunta 2 del PDF)
                var hoy = DateTime.UtcNow;
                var estudiantes = await _context.Estudiantes.ToListAsync();

                int entre3y7 = 0, entre8y12 = 0, mayores12 = 0;
                foreach (var est in estudiantes)
                {
                    var edad = hoy.Year - est.FechaNacimiento.Year;
                    if (hoy < est.FechaNacimiento.AddYears(edad)) edad--;

                    if (edad >= 3 && edad <= 7) entre3y7++;
                    else if (edad >= 8 && edad <= 12) entre8y12++;
                    else if (edad > 12) mayores12++;
                }

                return Ok(new
                {
                    Totales = new { totalEstudiantes, totalColegios, totalDocentes },
                    GraficaGrados = estudiantesPorGrado,
                    GraficaSectores = estudiantesPorSector,
                    Edades = new { entre3y7, entre8y12, mayores12 }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.InnerException?.Message ?? ex.Message });
            }
        }
    }
}