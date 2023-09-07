using CouchDB.Driver.Types;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using promodel.modelo.clientes;
using promodel.modelo.registro;

namespace promodel.modelo
{
    /// <summary>
    /// Usuario de la aplicación, los usurios son personas registradas en la base de datos para el acceso
    /// a alguno de los dominios registrados para la aplicación
    /// </summary>
    public class Usuario: CouchDocument
    {

        public Usuario()
        {
            RolesCliente = new List<RolCliente>();
            Clientes = new List<string>();
        }

        /// <summary>
        /// Nombre del usuario para el despliegue en la UI
        /// </summary>
        [JsonProperty("uname")]
        public string? NombreUsuario { get; set; }

        /// <summary>
        /// Nombre utilziado para el acceso
        /// </summary>
        [JsonProperty("nacc")]
        public string NombreAcceso { get; set; }

        /// <summary>
        /// Correo electrónico del usuario
        /// </summary>
        [JsonProperty("eml")]
        public string Email { get; set; }

        /// <summary>
        /// Hash de la contraseña
        /// </summary>
        [JsonProperty("chash")]
        public string HashContrasena { get; set; }


        /// <summary>
        /// Idetifica si el usuario se encuentra activo
        /// </summary>
        [JsonProperty("act")]
        public bool Activo { get; set; }


        /// <summary>
        /// FEcha del último inicio de sesión
        /// </summary>
        [JsonProperty("uacc")]
        public DateTime? UltimoAcceso { get; set; }

        /// <summary>
        /// Indica si el usuario se encuentra bloqueado
        /// </summary>
        [JsonProperty("blq")]
        public bool Bloqueado { get; set; } =false;

        /// <summary>
        /// Identificadro de desbloqueo
        /// </summary>
        [JsonProperty("bl1k")]
        public string? ClaveDesbloqueo { get; set; }

        /// <summary>
        /// Fecha límite para el desbloqueo
        /// </summary>
        [JsonProperty("blqt")]
        public DateTime? LimiteDesbloqueo { get; set; }


        /// <summary>
        /// Identificador del idioma por defecto utilziado para la UI
        /// </summary>
        [JsonProperty("idm")]
        public string? IdiomaId { get; set; }

        /// <summary>
        /// Identificador único de la zona horaria
        /// </summary>
        [JsonProperty("zh")]
        public string? ZonaHorariaId { get; set; }

        /// <summary>
        /// Desviación horaria
        /// </summary>
        [JsonProperty("zht")]
        public decimal? OffsetHorario { get; set; }

        /// <summary>
        /// Avatar del usuario en formato base 64, la imagen almacenad debe medir 150 X 150 PX
        /// </summary>
        [JsonProperty("avt")]
        public string? Avatar { get; set; }


        /// <summary>
        /// Lista de clientes a los que el usuario se encuentra adherido
        /// </summary>
        public List<string> Clientes { get; set; }


        /// <summary>
        /// Lista de roles por cliente
        /// </summary>
        public List<RolCliente> RolesCliente { get; set; }


        /// <summary>
        /// Identificador de la agencia a la que pertenece el usuario
        /// </summary>
        public string? AgenciaId { get; set; }

        /// <summary>
        /// Consentimientos aceptados por el usuario
        /// </summary>
        [JsonProperty("acns")]
        public List<AceptacionConsentimiento> AceptacionConsentimientos { get; set; }
    }
}
