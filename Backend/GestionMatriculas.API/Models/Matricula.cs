namespace GestionMatriculas.API.Models
{
    public class Matricula
    {
        public int Id { get; set; }
        public int EstudianteId { get; set; }
        public Estudiante? Estudiante { get; set; }
        public int GradoId { get; set; }
        public Grado? Grado { get; set; }
        public int Anio { get; set; }
        public string Grupo { get; set; } = string.Empty;
        public int? DocenteId { get; set; }
        public Docente? Docente { get; set; }
    }
}