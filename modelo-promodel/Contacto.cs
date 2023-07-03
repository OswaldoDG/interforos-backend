using Newtonsoft.Json;
using promodel.modelo.perfil;

namespace promodel.modelo
{


    public class Contacto
    {

        public Contacto ()
        {
            this.AccesoDireccion = new AccesoInformacion();
            this.AccesoEmail = new AccesoInformacion();
            this.AccesoTelefono = new AccesoInformacion();
            this.AccesoRedes = new AccesoInformacion();
        }

        [JsonProperty("d")]
        public string? Direccion { get; set; }

        [JsonProperty("m")]
        public string? Email { get; set; }

        [JsonProperty("t")]
        public string? Telefono { get; set; }

        [JsonProperty("tw")]
        public string? Twitter { get; set; }

        [JsonProperty("fb")]
        public string? FaceBook { get; set; }

        [JsonProperty("ln")]
        public string? LinkedIn { get; set; }

        [JsonProperty("in")]
        public string? Instagram { get; set; }

        [JsonProperty("x")]
        public bool OmitirDatos { get; set; }

        /// <summary>
        /// Indica si el telefono puede recibir mensajes de whats app
        /// </summary>
        [JsonProperty("wa")]
        public bool? TelefonoWhatsApp { get; set; }

        /// <summary>
        /// Espedifica si el télefono puede recibir mensajes de SMS
        /// </summary>
        [JsonProperty("sms")]
        public bool? TelSMS { get; set; }

        public AccesoInformacion? AccesoDireccion { get; set; }
        public AccesoInformacion? AccesoEmail { get; set; }
        public AccesoInformacion? AccesoTelefono { get; set; }
        public AccesoInformacion? AccesoRedes { get; set; }

    }
}
