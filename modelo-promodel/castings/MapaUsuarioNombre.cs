using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace promodel.modelo.castings;

/// <summary>
///  Devuelve el nombre del usaurio en base a su Id
/// </summary>
public class MapaUsuarioNombre
{
    public string Id { get; set; }

    public string Nombre { get; set; }

    public string? Email { get; set; }
}
