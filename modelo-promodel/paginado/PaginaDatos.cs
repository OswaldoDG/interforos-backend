using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace promodel.modelo.paginado;

public class PaginaDatos<T>
{
    /// <summary>
    /// PAgina de datos actual comienza en cero
    /// </summary>
    public int Pagina { get; set; }

    /// <summary>
    /// Tamano de paginado
    /// </summary>
    public int Tamano { get; set; }

    /// <summary>
    /// Total de elementos en el repositorio
    /// </summary>
    public int Total { get; set; }

    /// <summary>
    /// Lista de elementos de la pagina actual
    /// </summary>
    public List<T> Elementos { get; set; }
}
