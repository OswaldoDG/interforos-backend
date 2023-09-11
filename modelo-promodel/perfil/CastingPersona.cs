using Newtonsoft.Json;

namespace promodel.modelo.perfil
{
    /// <summary>
    /// Vincula un modelo a un casting
    /// </summary>
    public class CastingPersona
    {
        /// <summary>
        /// IDentificador único del cliente 
        /// </summary>
        [JsonProperty("c")]
        public string ClienteId { get; set; }

        /// <summary>
        /// Identificador único del casting
        /// </summary>
        [JsonProperty("cid")]
        public string CastingId { get; set; }

        /// <summary>
        /// Identifica si modelo ha sido declinado para el castins
        /// </summary>
        [JsonProperty("d")]
        public bool Declinado { get; set; } = false;

        /// <summary>
        /// Fecha de adición del modelo al casting
        /// </summary>
        [JsonProperty("f")]
        public DateTime FechaAdicion { get; set; }

        /// <summary>
        /// Identificador único del folder en google drive para la personas
        /// </summary>
        public string FolderId { get; set; }
    }
}
