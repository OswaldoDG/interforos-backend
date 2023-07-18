

using promodel.modelo;
using promodel.modelo.castings;
using promodel.modelo.perfil;
using promodel.modelo.proyectos;


namespace promodel.servicios.castings;

 public static class ExtencionesCastingServicios
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
            Id = Guid.NewGuid().ToString(),
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
}
