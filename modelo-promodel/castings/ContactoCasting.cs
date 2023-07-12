using promodel.modelo.registro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace promodel.modelo.proyectos
{

    public class ContactoCasting
    {
        /// <summary>
        /// Identificador del usuario si ya se encuentra registrado
        /// </summary>
        public string? UsuarioId { get; set; }

        /// <summary>
        /// Email de usuario registrado o invitado
        /// </summary>
        public string?  Email { get; set; }

        /// <summary>
        ///  Indica si el usuario ya a ingresado al menos una vezal proyecto
        /// </summary>
        public bool Confirmado { get; set; } = false;

        // Tipo de rol asignado al contacto del casting
        public TipoRolCliente Rol { get; set; }

        /// <summary>
        /// Mantiene la fecha y hora del ultimo ingreso del usurio al proyecto
        /// </summary>
        public DateTime? UltimoIngreso { get; set; }

    }
}
