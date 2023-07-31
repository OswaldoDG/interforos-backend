

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
}
