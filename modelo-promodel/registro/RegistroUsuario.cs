using System.ComponentModel.DataAnnotations;

namespace promodel.modelo
{
    public class RegistroUsuario
    {
        /// <summary>
        /// Correo electrónico del usuarios
        /// </summary>
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        /// <summary>
        /// Nombre de la persona que se registrs
        /// </summary>
        [Required]
        public string Nombre { get; set; }
        /// <summary>
        /// Identificador del cleinte al que se añade el usuario, se obitiene de lalista de clientes en base al HostDetector
        /// </summary>
        public string? ClienteId { get; set; }
        /// <summary>
        /// Identifica el rol del usuario en relación al cliente
        /// </summary>
        public TipoRolCliente Rol { get; set; }

        /// <summary>
        /// Se llena cuando la invitación se origina en un casting
        /// </summary>
        public string? CastingId { get; set; }

        /// <summary>
        /// Identificador de la agencia a la que pertenece el usuario
        /// </summary>
        public string? AgenciaId { get; set; }

        public string? Mensaje { get; set; }
        public string? MensajeDe { get; set; }
    }
}
