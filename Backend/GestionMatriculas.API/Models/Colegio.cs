namespace GestionMatriculas.API.Models
{
    public class Colegio
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Sector { get; set; } = string.Empty; // "Público" o "Privado"
    }
}