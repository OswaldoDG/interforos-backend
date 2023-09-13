using CouchDB.Driver.Types;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace promodel.modelo.castings
{
    public class HistorialCasting: CouchDocument
    {
        /// <summary>
        /// Nombre del casting 
        /// </summary>
        [JsonProperty("n")]
        
        public string Nombre
            ;
        /// <summary>
        /// Lista de eventos ocurridos al casting
        /// </summary>
        [JsonProperty("evs")]

        public List<BitacoraCasting> Eventos;
    }
}
