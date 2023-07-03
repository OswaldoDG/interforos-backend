using CouchDB.Driver.Types;
using Newtonsoft.Json;


namespace promodel.modelo
{

    public class InvitacionRegistro: CouchDocument
    {
        public InvitacionRegistro ()
        {
            Id = new Guid().ToString();
        }

        [JsonProperty("e")]
        public DateTime Emitida { get; set; }
        
        [JsonProperty("l")]
        public DateTime LimiteUso { get; set; }

        [JsonProperty("r")]
        public RegistroUsuario Registro { get; set; }

    }
}
