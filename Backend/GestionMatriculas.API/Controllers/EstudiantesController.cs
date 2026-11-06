using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestionMatriculas.API.Data;
using GestionMatriculas.API.Models;

namespace GestionMatriculas.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EstudiantesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EstudiantesController(AppDbContext context)
        {
            _context = context;
        }

        // 1. CREAR
        [HttpPost]
        public async Task<IActionResult> CrearEstudiante([FromBody] CrearEstudianteDto dto)
        {
            try
            {
                var estudiante = new Estudiante
                {
                    Nombre = dto.Nombre,
                    FechaNacimiento = DateTime.SpecifyKind(dto.FechaNacimiento, DateTimeKind.Utc),
                    ColegioId = dto.ColegioId
                };
                _context.Estudiantes.Add(estudiante);
                await _context.SaveChangesAsync();

                var matricula = new Matricula
                {
                    EstudianteId = estudiante.Id,
                    GradoId = dto.GradoId,
                    Anio = dto.Anio,
                    Grupo = dto.Grupo,
                    DocenteId = dto.DocenteId
                };
                _context.Matriculas.Add(matricula);
                await _context.SaveChangesAsync();

                return Ok(new { mensaje = "Estudiante matriculado con éxito", estudianteId = estudiante.Id });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.InnerException?.Message ?? ex.Message });
            }
        }

        // 2. CONSULTAR CON FILTROS
        [HttpGet("filtrar")]
        public async Task<IActionResult> ConsultarEstudiantes(int colegioId, int gradoId, int anio)
        {
            try
            {
                var resultados = await _context.Matriculas
                    .Include(m => m.Estudiante)
                    .Include(m => m.Grado)
                    .Where(m => m.Estudiante!.ColegioId == colegioId && m.GradoId == gradoId && m.Anio == anio)
                    .Select(m => new
                    {
                        EstudianteId = m.EstudianteId,
                        NombreEstudiante = m.Estudiante!.Nombre,
                        Grado = m.Grado!.Nombre,
                        Grupo = m.Grupo,
                        Anio = m.Anio
                    })
                    .ToListAsync();

                return Ok(resultados);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.InnerException?.Message ?? ex.Message });
            }
        }

        // NUEVO: CONSULTAR TODOS (SIN FILTROS)
        [HttpGet("todos")]
        public async Task<IActionResult> ObtenerTodos()
        {
            try
            {
                var resultados = await _context.Matriculas
                    .Include(m => m.Estudiante)
                    .Include(m => m.Grado)
                    .Select(m => new
                    {
                        EstudianteId = m.EstudianteId,
                        NombreEstudiante = m.Estudiante!.Nombre,
                        Grado = m.Grado!.Nombre,
                        Grupo = m.Grupo,
                        Anio = m.Anio,
                        ColegioId = m.Estudiante!.ColegioId
                    })
                    .ToListAsync();

                return Ok(resultados);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.InnerException?.Message ?? ex.Message });
            }
        }

        // 3. ACTUALIZAR
        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarEstudiante(int id, [FromBody] CrearEstudianteDto dto)
        {
            try
            {
                var estudiante = await _context.Estudiantes.FindAsync(id);
                if (estudiante == null) return NotFound("Estudiante no encontrado.");

                estudiante.Nombre = dto.Nombre;
                estudiante.FechaNacimiento = DateTime.SpecifyKind(dto.FechaNacimiento, DateTimeKind.Utc);
                estudiante.ColegioId = dto.ColegioId;

                var matricula = await _context.Matriculas.FirstOrDefaultAsync(m => m.EstudianteId == id);
                if (matricula != null)
                {
                    matricula.GradoId = dto.GradoId;
                    matricula.Anio = dto.Anio;
                    matricula.Grupo = dto.Grupo;
                    matricula.DocenteId = dto.DocenteId;
                }

                await _context.SaveChangesAsync();
                return Ok(new { mensaje = "Estudiante actualizado correctamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.InnerException?.Message ?? ex.Message });
            }
        }

        // 4. ELIMINAR
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarEstudiante(int id)
        {
            try
            {
                var estudiante = await _context.Estudiantes.FindAsync(id);
                if (estudiante == null) return NotFound("Estudiante no encontrado.");

                var matriculas = _context.Matriculas.Where(m => m.EstudianteId == id);
                _context.Matriculas.RemoveRange(matriculas);
                _context.Estudiantes.Remove(estudiante);

                await _context.SaveChangesAsync();
                return Ok(new { mensaje = "Estudiante eliminado correctamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.InnerException?.Message ?? ex.Message });
            }
        }
    }

    public class CrearEstudianteDto
    {
        public string Nombre { get; set; } = string.Empty;
        public DateTime FechaNacimiento { get; set; }
        public int ColegioId { get; set; }
        public int GradoId { get; set; }
        public int Anio { get; set; }
        public string Grupo { get; set; } = string.Empty;
        public int? DocenteId { get; set; }
    }
}