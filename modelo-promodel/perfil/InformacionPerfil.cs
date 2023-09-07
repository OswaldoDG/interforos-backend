using CouchDB.Driver.Types;
using Newtonsoft.Json;
using promodel.modelo.clientes;
using System.Security.Principal;

namespace promodel.modelo.perfil
{
    public class InformacionPerfil
    {

        /// <summary>
        /// Nombre completo del usuario en sesión
        /// </summary>
        public string NombreCompleto { get; set; }

        /// <summary>
        /// Identificador único del usuario
        /// </summary>
        public string UsuarioId { get; set; }

        /// <summary>
        /// Nombre de usario
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// Imagen avatar en formato base 64
        /// </summary>
        public string AvatarBase64 { get; set; }

        /// <summary>
        /// DEtermina si el usuario require tener un perfil en la aplicación
        /// </summary>
        public bool RequirePerfil { get; set; }

        /// <summary>
        /// Determina si el usuario tiene un peefil registrado en la app
        /// </summary>
        public bool TienePerfil { get; set; }

        /// <summary>
        /// Roles del usuario en el cliente
        /// </summary>
        public List<TipoRolCliente> Roles { get; set; }

        /// <summary>
        /// LIsta de los consentimientos aceptados por el usuario
        /// </summary>
        public List<AceptacionConsentimiento> CosentimientosAceptados { get; set; }
    }
}
