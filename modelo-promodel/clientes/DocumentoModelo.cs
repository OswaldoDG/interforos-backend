using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace promodel.modelo.clientes
{
    public class DocumentoModelo
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("n")]
        public string Nombre { get; set; }

        [JsonProperty("o")]
        public bool Obligatorio { get; set; }

    }
}
