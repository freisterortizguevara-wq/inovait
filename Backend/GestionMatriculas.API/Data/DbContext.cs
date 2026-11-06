using GestionMatriculas.API.Models;
using Microsoft.EntityFrameworkCore;

namespace GestionMatriculas.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Colegio> Colegios { get; set; }
        public DbSet<Grado> Grados { get; set; }
        public DbSet<Docente> Docentes { get; set; }
        public DbSet<Estudiante> Estudiantes { get; set; }
        public DbSet<Matricula> Matriculas { get; set; }
        public DbSet<DocenteColegio> DocenteColegios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DocenteColegio>()
                .HasKey(dc => new { dc.DocenteId, dc.ColegioId });

            modelBuilder.Entity<DocenteColegio>()
                .HasOne(dc => dc.Docente)
                .WithMany(d => d.DocenteColegios)
                .HasForeignKey(dc => dc.DocenteId);

            modelBuilder.Entity<DocenteColegio>()
                .HasOne(dc => dc.Colegio)
                .WithMany()
                .HasForeignKey(dc => dc.ColegioId);
        }
    }
}
