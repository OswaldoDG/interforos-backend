using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace promodel.modelo.perfil
{
    public class PerfilPublicoUsuario
    {
        /// <summary>
        /// Identificador único del usuario
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Nombre del usuario para ser identificado por otros usuarios
        /// </summary>
        public string Nombre { get; set; }

        /// <summary>
        /// Avatar del usuario en base 64 
        /// </summary>
        public string Avatar { get; set; }
    }
}
