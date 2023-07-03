using Newtonsoft.Json;
using promodel.modelo.perfil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace promodel.modelo.proyectos
{

    public enum TipoCuerpo
    {
        Bajo=0, Normal=1, Sobrepeso=2, Obeso=3
    }

    /// <summary>
    /// Define una busqueda default para la categoria para facilitar la busquea de moelos
    /// </summary>
    public class BusquedaPersonas
    {

        /// <summary>
        /// ID del cliente asociado a la persona para buscar
        /// </summary>
        public string? ClienteId { get; set; }


        /// <summary>
        /// Generos a incluir para la categoría
        /// </summary>
        [JsonProperty("g")]
        public List<string>? GenerosId { get; set; }

        /// <summary>
        /// Edad mínimo para los modelos a incluir
        /// </summary>
        [JsonProperty("e1")]
        public int? EdadMinima { get; set; }

        /// <summary>
        /// Edad máxima para los modelos a incluir
        /// </summary>
        [JsonProperty("e2")]
        public int? EdadMaxima { get; set; }


        /// <summary>
        /// Tipo de cuerpos a bsucar en base al IMC
        /// </summary>
        [JsonProperty("tcs")]
        public List<TipoCuerpo>? TipoCuerpos { get; set; }

        /// <summary>
        /// Nombre o nombre artistico se busca parcialmente
        /// </summary>
        [JsonProperty("nm")]
        public string? Nombre { get; set; }


        /// <summary>
        /// Etnias de la selección
        /// </summary>
        [JsonProperty("et")]
        public List<string>? EtniasIds { get; set; }

        /// <summary>
        ///  Colo res de ojos para incluir en la busqueda 
        /// </summary>
        [JsonProperty("o")]
        public List<string>? ColorOjosIds { get; set; }

        [JsonProperty("tc")]
        public List<string>? TipoCabelloIds { get; set; }


        [JsonProperty("cc")]
        public List<string>? ColorCabelloIds { get; set; }

        [JsonProperty("ln")]
        public List<string>? IdiomasIds { get; set; }

        [JsonProperty("hs")]
        public List<string>? HabilidadesIds { get; set; }

    }
}
