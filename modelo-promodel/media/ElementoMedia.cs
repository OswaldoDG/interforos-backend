
using Newtonsoft.Json;

namespace promodel.modelo.media
{
    public enum TipoMedio
    {
        Galería = 0, Documento = 1
    }

    public class ElementoMedia
    {
        public ElementoMedia()
        {
            ClienteIds = new List<string>();
        }

        /// <summary>
        /// Identificador unico del elemento
        /// </summary>
        [JsonProperty("i")]
        public string Id { get; set; }

        [JsonProperty("tm")]
        public TipoMedio Tipo { get; set; }

        /// <summary>
        /// Extensión del archivo original
        /// </summary>
        [JsonProperty("x")]
        public string Extension { get; set; }

        /// <summary>
        /// Lista de clientes que tienen acceso al elemento
        /// </summary>
        [JsonProperty("cl")]
        public List<string> ClienteIds { get; set; }

        /// <summary>
        /// Tipo asociado
        /// </summary>
        [JsonProperty("t")]
        public string MimeType { get; set; }

        /// <summary>
        /// Determina si el elemento es una imagen
        /// </summary>
        [JsonProperty("m")]
        public bool Imagen { get; set; }

        /// <summary>
        /// Determina si el elemento es un video
        /// </summary>
        [JsonProperty("v")]
        public bool Video { get; set; }


        /// <summary>
        /// DEtermina si el contenido es un PDF
        /// </summary>
        [JsonProperty("pdf")]
        public bool Pdf { get; set; }

        /// <summary>
        /// Tamaño del elemento en bytes
        /// </summary>
        [JsonProperty("s")]
        public long TamanoBytes { get; set; }

        /// <summary>
        /// DEtermina si la imagen se orienta landscape
        /// </summary>
        [JsonProperty("l")]
        public bool Landscape { get; set; } 


        /// <summary>
        /// DEtermina si la imagen es la utilizada como principal
        /// </summary>
        [JsonProperty("a")]
        public bool Principal { get; set; } = false;

        /// <summary>
        /// Determina si el elemento se considera permanente
        /// </summary>
        [JsonProperty("p")]
        public bool Permanente { get; set; }

        /// <summary>
        /// Ticks de la fecha de creacion
        /// </summary>
        [JsonProperty("f")]
        public long CreacionTicks { get; set; }


        /// <summary>
        /// Ticks de la fecha de eliminacion
        /// </summary>
        [JsonProperty("z")]
        public long? EliminacionTicks { get; set; }

        [JsonProperty("fv")]
        public string ? FrameVideoId { get; set; }

    }
}
