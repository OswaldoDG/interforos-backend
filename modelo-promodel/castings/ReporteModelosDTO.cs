using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace promodel.modelo.castings
{
    public class ReporteModelosDTO
    {
        public string Categoria { get; set; }
        public string? FotoPrincipal { get; set; }
        public string NombreArtistico { get; set; }
        public string NombrePersona { get; set; }
        public string Genero { get; set; }
        public int Edad { get; set; }
        public string Habilidades { get; set; }
    }
}
