using Newtonsoft.Json;
using promodel.modelo.castings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace promodel.modelo.proyectos
{
    public enum OrigenInscripcion
    {
        publico = 0, staff = 1
    }


    /// <summary>
    /// Persona propuesta como candidato a una categoría del proyecto
    /// </summary>
    public class ModeloCasting
    {
        /// <summary>
        /// Identificador úico de la persona
        /// </summary>
        [JsonProperty("p")]
        public string PersonaId { get; set; }

        /// <summary>
        /// Determina si la persona ha sido aceptadta en el proyectp
        /// </summary>
        [JsonProperty("a")]
        public bool Aceptada { get; set; } = false;

        /// <summary>
        /// Determina si la persona ha sido declinada
        /// </summary>
        [JsonProperty("d")]
        public bool Declinada { get; set; } = false;

        [JsonProperty("o")]
        public OrigenInscripcion Origen { get; set; }

        /// <summary>
        /// Identificador único del medio utilizado para la prtada
        /// </summary>
        [JsonProperty("pimid")]
        public string? ImagenPortadaId { get; set; }

        /// <summary>
        /// Identificador único del medio utilizado para el video
        /// </summary>
        [JsonProperty("vimid")]
        public string? VideoPortadaId { get; set; }

        /// <summary>
        /// Comentarios realizados sobre la persona en la categoría
        /// </summary>
        [JsonProperty("cs")]
        public List<ComentarioCasting> Comentarios { get; set; } = new List<ComentarioCasting>();

        [JsonProperty("vo")]
        public List<VotoModeloCategoria> Votos { get; set; } = new List<VotoModeloCategoria>();
        /// <summary>
        /// Identificador único del folder de almacenamiento en google drive de casting
        /// </summary>
        [JsonProperty("fid")]
        public string? FolderId { get; set; }

    }
}
