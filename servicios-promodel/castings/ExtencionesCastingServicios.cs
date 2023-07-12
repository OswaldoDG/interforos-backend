

using Bogus;
using promodel.modelo;
using promodel.modelo.perfil;
using promodel.modelo.proyectos;
using System.Reflection.Metadata.Ecma335;

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
}
