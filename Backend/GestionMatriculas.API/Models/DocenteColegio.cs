namespace GestionMatriculas.API.Models
{
    public class DocenteColegio
    {
        public int DocenteId { get; set; }
        public Docente? Docente { get; set; }
        public int ColegioId { get; set; }
        public Colegio? Colegio { get; set; }
    }
}
