using System.ComponentModel.DataAnnotations;

namespace promodel.modelo
{
    public class CreacionUsuario
    {
        [Required]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string Contrasena { get; set; }
    }
}
