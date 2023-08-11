using promodel.modelo.proyectos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace promodel.modelo.castings
{
    public class ComentarioCategoriaModeloCasting: ComentarioCasting
    {
        public string CategoriaId { get; set; }
        public string PersonaId { get; set; }
    }
}
