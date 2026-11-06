namespace GestionMatriculas.API.Models
{
    public class Docente
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public List<DocenteColegio> DocenteColegios { get; set; } = new();
    }
}