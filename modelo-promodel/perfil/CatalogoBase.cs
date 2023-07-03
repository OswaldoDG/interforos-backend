using CouchDB.Driver.Types;
using Newtonsoft.Json;

namespace promodel.modelo.perfil
{

    public class CatalogoBase : CouchDocument
    {
        /// <summary>
        /// DEfine la propiedad a la que se vincula el catálogo por ejemplo genero o país
        /// </summary>
        [JsonProperty("p")]
        public string TipoPropiedad { get; set; }

        /// <summary>
        /// Identificador único del cliente al que pertenece el catálogo
        /// </summary>
        [JsonProperty("cl")]
        public string ClienteId { get; set; }

        /// <summary>
        /// Lista de elementos pertenecientes al catálogo
        /// </summary>
        [JsonProperty("el")]
        public List<ElementoCatalogo> Elementos { get; set; }

    }


    public class ElementoCatalogo
    {
        /// <summary>
        /// Clave del élemento
        /// </summary>
        [JsonProperty("c")]
        public string Clave { get; set; }

        /// <summary>
        /// Clave del padre del elemento para elementos vinculados
        /// </summary>
        [JsonProperty("pc")]
        public string? ClavePadre { get; set; }


        /// <summary>
        /// Texto a mostrar en el idioma
        /// </summary>
        [JsonProperty("t")]
        public string Texto { get; set; }

        /// <summary>
        /// Idioma del elemento
        /// </summary>
        [JsonProperty("i")]
        public string Idioma { get; set; }
    }

}
