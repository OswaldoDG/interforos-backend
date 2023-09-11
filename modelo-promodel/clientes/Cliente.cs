using CouchDB.Driver.Types;
using Newtonsoft.Json;

namespace promodel.modelo.clientes
{
    /// <summary>
    /// Representa un cliente agencia a castinera con una URL activada
    /// </summary>
    public class Cliente: CouchDocument
    {


        public Cliente ()
        {
            Documentacion = new List<DocumentoModelo>();
            Id = DateTime.UtcNow.Ticks.ToString();
        }

        /// <summary>
        /// Nombre del cliente
        /// </summary>
        [JsonProperty("n")]
        public string Nombre { get; set; }

        /// <summary>
        /// Url asociado al cliente
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; set; }

        /// <summary>
        /// DEtermina si el cliente se encuntra activo
        /// </summary>
        [JsonProperty("on")]
        public bool Activo { get; set; }

        /// <summary>
        /// Logo WEB B64
        /// </summary>
        [JsonProperty("lw")]
        public string? WebLogoBase64 { get; set; }

        /// <summary>
        /// Logo email b64
        /// </summary>
        [JsonProperty("lm")]
        public string? MailLogoURL{ get; set; }

        /// <summary>
        /// Clave del pais por default del cliente
        /// </summary>
        [JsonProperty("paisId")]
        public string PaisDefault { get; set; } 

        [JsonProperty("cn")]
        public Contacto Contacto { get; set; }

        [JsonProperty("docs")]
        public List<DocumentoModelo> Documentacion { get; set; }

        /// <summary>
        /// Lista de consentimiantos del cliente
        /// </summary>
        [JsonProperty("showCns")]
        public bool MostrarConsentimientos { get; set; }

        /// <summary>
        /// Lista de consentimiantos del cliente
        /// </summary>
        [JsonProperty("cns")]
        public List<Consentimiento> Consentimientos { get; set; }

    }
}
