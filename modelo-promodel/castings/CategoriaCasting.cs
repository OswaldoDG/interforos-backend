using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace promodel.modelo.proyectos
{
    public class CategoriaCasting
    {
        /// <summary>
        /// Identificador único de la categoria
        /// </summary>
        [JsonProperty("i")]
        public string Id { get; set; }

        /// <summary>
        /// Nombre de la categoría
        /// </summary>
        [JsonProperty("n")]
        public string Nombre { get; set; }


        [JsonProperty("d")]
        public string? Descripcion{ get; set; }


        /// <summary>
        /// Busuqeda default, sólo visible para los usurios con rol staff del proyecto para añadir 
        /// más modelos
        /// </summary>
        [JsonProperty("s")]
        public BusquedaPersonas? BusquedaDefault { get; set; }

        /// <summary>
        /// Modelos propuestos en la categoría
        /// </summary>
        [JsonProperty("ms")]
        public List<ModeloCasting> Modelos { get; set; } = new List<ModeloCasting>();
    }
}
