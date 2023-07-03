using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace almacenamiento
{
    public class CacheAlmacenamientoLocalConfig
    {
        /// <summary>
        /// Rutal del almacenamiento temporal
        /// </summary>
        public string Ruta { get; set; }

        /// <summary>
        /// Porcentaje del disco a mantener libre
        /// </summary>
        public int MinProcentajeLibre { get; set; }

        /// <summary>
        /// Establece el número de días para eliminar elementos en base a su antigüedad
        /// </summary>
        public int DiasLimpieza { get; set; }

        public int TamanoCard { get; set; }
        public int TamanoThumb { get; set; }
        public int Quality { get; set; }


    }
}
