using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace promodel.servicios.comunes
{
    public class ResponsePaginado<T> where T : class
    {
        public List<T> Elementos { get; set; }
        public int Total { get; set; }
        public int Pagina { get; set; }
        public int Tamano { get; set; }
    }
}
