using CouchDB.Driver.Types;
using Newtonsoft.Json;

namespace promodel.modelo.media
{
    public class MediaModelo : CouchDocument
    {
        public MediaModelo()
        {
            Elementos = new List<ElementoMedia>();
            TamanoBytes = 0;
        }

        /// <summary>
        /// Identificador único del usuario
        /// </summary>
        [JsonProperty("uid")]
        public string UsuarioId { get; set; }

        /// <summary>
        /// Lista de los elementos de medios del usuario
        /// </summary>
        [JsonProperty("el")]
        public List<ElementoMedia> Elementos { get; set; }

        /// <summary>
        /// Ticks de la fecha de eliminacion para el elemento más proximo,
        /// Esta propiead se actualiza al añadir nuevos elementos caducibles y 
        /// durante el proceso de eliminación asociado
        /// </summary>
        [JsonProperty("z")]
        public long? EliminacionTicks { get; set; }

        /// <summary>
        /// Tamaño del portafolios de medios
        /// </summary>
        [JsonProperty("s")]
        public long TamanoBytes { get; set; }
    }


}
