

using CouchDB.Driver.Query.Extensions;
using promodel.modelo;
using promodel.modelo.castings;
using promodel.modelo.perfil;
using promodel.modelo.proyectos;
using System.Globalization;

namespace promodel.servicios.castings;

 public static class ExtensionesCastingServicios
{
    public static ContactoCasting aContactoCasting(this ContactoUsuario usuario,DateTime? UltimoAcceso)
    {
        return new ContactoCasting()
        {
            Confirmado = false,
            Email = usuario.Email.ToLower(),
            Rol = (TipoRolCliente)usuario.Rol,
            UltimoIngreso = UltimoAcceso,
            UsuarioId = usuario.Id,              
        };
    }

    public static CastingListElement aCastingListElement(this Casting casting,TipoRolCliente rol)
    {
        return new CastingListElement()
        {
            Id = casting.Id,
            Nombre = casting.Nombre,
            NombreCliente = casting.NombreCliente,
            FechaApertura = casting.FechaApertura,
            FechaCierre = casting.FechaCierre,
            AceptaAutoInscripcion = casting.AceptaAutoInscripcion,
            Activo = casting.Activo,
            AperturaAutomatica = casting.AperturaAutomatica,
            CierreAutomatico = casting.CierreAutomatico,
            Rol =rol
        };
    }
    public static CastingListElement aCastingListElement(this Casting casting)
    {  
        return new CastingListElement()
        {            
            Id = casting.Id,
            Nombre = casting.Nombre,
            NombreCliente = casting.NombreCliente,
            FechaApertura = casting.FechaApertura,
            FechaCierre = casting.FechaCierre,
            AceptaAutoInscripcion = casting.AceptaAutoInscripcion,
            Activo = casting.Activo,
            AperturaAutomatica = casting.AperturaAutomatica,
            CierreAutomatico = casting.CierreAutomatico,
        };
    }
<<<<<<< Updated upstream
    /// <summary>
    /// Actualiza una categoria existente.
    /// </summary>
    /// <param name="categoria"></param>
    /// <param name="categoriaNueva"></param>
    public static void Actualizar(this CategoriaCasting categoria, CategoriaCasting categoriaNueva)
    {
        categoria.Nombre = categoriaNueva.Nombre;
        categoria.Descripcion = categoriaNueva.Descripcion;
    }
    /// <summary>
    /// remueve las categorias de un casting que no se encuentren en <param name="categoriaNueva"></param>
    /// </summary>
    /// <param name="categoria"></param>
    /// <param name="categoriaNueva"></param>
    public static List<CategoriaCasting> RemoverCategoriaElminada(this List<CategoriaCasting> categorias, List<CategoriaCasting> categoriasNuevas)
    {
       var categoriaselimnadas = categorias.Where(p => categoriasNuevas.All(p2 => p2.Id != p.Id)).ToList();
        categoriaselimnadas.ForEach(_ => categorias.Remove(_));
        return categorias;

    }

    /// <summary>
    /// Actualiza , agrega y remueve categorias eliminadas de un casting dado  una lista de categorias
    /// </summary>
    /// <param name="casting"></param>
    /// <param name="categoriasNuevas"></param>
    public static void ActulizarCategorias(this Casting casting, List<CategoriaCasting> categoriasNuevas)
    {       
        foreach (var categoria in categoriasNuevas)
        {
            var existeCategoria = casting.Categorias.FirstOrDefault(_ => _.Id == categoria.Id);
            if (existeCategoria != null)
            {
                existeCategoria.Actualizar(categoria);
            }
            else
            {
                casting.Categorias.Add(categoria);
            }
        }
        casting.Categorias = casting.Categorias.RemoverCategoriaElminada(categoriasNuevas);
    }
      

    public static EventoCasting aEventoCasting(this EventoCasting eventoCasting)
    {
        return new EventoCasting()
        {
            Id = eventoCasting.Id,
            FechaInicial = eventoCasting.FechaInicial,
            FechaFinal = eventoCasting.FechaFinal,
            Notas = eventoCasting.Notas,
            Lugar = eventoCasting.Lugar,
            Coordenadas = eventoCasting.Coordenadas
            
        };
    }
=======
>>>>>>> Stashed changes
}
