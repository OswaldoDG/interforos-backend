using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace promodel.modelo.registro
{



    public class RolCliente
    {
        [JsonProperty("i")]
        public string ClienteId { get; set; }

        [JsonProperty("r")]
        public TipoRolCliente Rol { get; set; }
    }
}
