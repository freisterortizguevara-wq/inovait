namespace GestionMatriculas.API.Models
{
    public class Estudiante
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public DateTime FechaNacimiento { get; set; }
        public int ColegioId { get; set; }
        public Colegio? Colegio { get; set; }
        public List<Matricula> Matriculas { get; set; } = new();
    }
}
