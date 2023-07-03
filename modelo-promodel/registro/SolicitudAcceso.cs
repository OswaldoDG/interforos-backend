using System.ComponentModel.DataAnnotations;

namespace promodel.modelo
{
    public class SolicitudAcceso
    {
        [Required]
        public string Usuario { get; set; }

        [Required]
        public string Contrasena { get; set; }
    }
}
