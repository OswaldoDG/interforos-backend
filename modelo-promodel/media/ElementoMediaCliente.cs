using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace promodel.modelo.media
{
    public class ElementoMediaCliente
    {
        public string Id { get; set; }
        public string Extension { get; set; }
        public string MimeType { get; set; }
        public bool Imagen { get; set; }
        public bool Video { get; set; }
        public bool Pdf { get; set; }
        public bool Permanente { get; set; }
        public bool Principal { get; set; }
        public bool Landscape { get; set; }
        public TipoMedio Tipo { get; set; }
        /// <summary>
        /// El id del medio para el frame de video salvado como foto
        /// </summary>
        public string FrameVideoId { get; set; }

        public string ? Titulo { get; set; }

        /// <summary>
        /// IDentificador único del casting al que pertenece el medio, 
        /// nulo para medios del perfil
        /// </summary>
        [JsonProperty("casid")]
        public string? CastingId { get; set; }

    }
}
