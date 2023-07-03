using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace promodel.modelo.proyectos
{
    public class ComentarioCasting
    {
        /// <summary>
        /// Identificador único del comentario
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// FEcha del comentario en Ticks
        /// </summary>
        [JsonProperty("f")]

        public DateTime Fecha { get; set; } = DateTime.UtcNow;


        /// <summary>
        /// Identificador único del usuario que creó el comentario
        /// </summary>
        [JsonProperty("u")]
        public string UsuarioId { get; set; }


        /// <summary>
        /// Contenido del comentario
        /// </summary>
        [JsonProperty("c")]
        public string Comentario { get; set; }
    }
}
