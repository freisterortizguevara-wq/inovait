using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestionMatriculas.API.Data;
using GestionMatriculas.API.Models;

namespace GestionMatriculas.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocentesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DocentesController(AppDbContext context)
        {
            _context = context;
        }

        // Asignar un docente a un colegio
        [HttpPost("{docenteId}/asignar-colegio/{colegioId}")]
        public async Task<IActionResult> AsignarColegio(int docenteId, int colegioId)
        {
            // Verificar si ya existe la asignación
            var existe = await _context.DocenteColegios
                .AnyAsync(dc => dc.DocenteId == docenteId && dc.ColegioId == colegioId);

            if (existe) return BadRequest("El docente ya está asignado a este colegio.");

            var asignacion = new DocenteColegio
            {
                DocenteId = docenteId,
                ColegioId = colegioId
            };

            _context.DocenteColegios.Add(asignacion);
            await _context.SaveChangesAsync();

            return Ok("Docente asignado al colegio exitosamente.");
        }

        // Consultar en qué colegios está un docente
        [HttpGet("{docenteId}/colegios")]
        public async Task<IActionResult> ObtenerColegiosDocente(int docenteId)
        {
            var colegios = await _context.DocenteColegios
                .Include(dc => dc.Colegio)
                .Where(dc => dc.DocenteId == docenteId)
                .Select(dc => new { dc.Colegio!.Id, dc.Colegio.Nombre, dc.Colegio.Sector })
                .ToListAsync();

            return Ok(colegios);
        }
    }
}