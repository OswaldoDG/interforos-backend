using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace promodel.modelo.castings
{
    public class VotoModeloMapeo
    {
        /// <summary>
        /// Id del modelo votado
        /// </summary>
        public string PersonaId { get; set; }


        /// <summary>
        /// Id del usaurio revisor que emitió el voto
        /// </summary>
        public  string UsuarioId { get; set; }


        /// <summary>
        /// Determina el grado de aceptación de un modelo en el casting
        /// Los valores ´validos son 
        /// 0 = no me gusta 
        /// 1 = no sé si me gusta
        /// 2 = me gusta
        /// 3 = me gusta mucho
        /// </summary>
        public int NivelLike { get; set; }
    }
}
