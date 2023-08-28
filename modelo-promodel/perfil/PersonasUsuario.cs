using CouchDB.Driver.Types;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace promodel.modelo.perfil
{
    /// <summary>
    /// Establece una relación de las personas dadas de alta por una cuenta de usuarios
    /// </summary>
    public class PersonasUsuario: CouchDocument
    {
        /// <summary>
        /// Identificador del usuario que da de alta los usuarios 
        /// </summary>
        [JsonProperty("idp")]
        public string UsuarioId { get; set; }

        /// <summary>
        /// Id del cliente donde fueron creados los perfiles
        /// </summary>
        [JsonProperty("idc")]
        public string ClienteId { get; set; }

        /// <summary>
        /// Id de la agencia involucrada en el alta
        /// </summary>
        [JsonProperty("ida")]
        public string? AgenciaId { get; set; }

        /// <summary>
        /// Lista de IDs de las personas creadas
        /// </summary>
        [JsonProperty("lp")]
        public List<string> IdPersonas { get; set; }
    }
}
