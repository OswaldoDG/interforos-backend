using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace promodel.modelo.media
{
    public class MediaCliente
    {
        public MediaCliente()
        {
            Elementos = new List<ElementoMediaCliente>();
        }
        public string UsuarioId { get; set; }
        public List<ElementoMediaCliente> Elementos { get; set; }
    }
}
