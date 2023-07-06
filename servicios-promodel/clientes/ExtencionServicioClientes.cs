

using promodel.modelo;
using promodel.modelo.perfil;

namespace promodel.servicios.clientes;

public static  class ExtencionServicioClientes
{

    public static ContactoUsuario aContactoUsuario(this Usuario usuario, TipoRolCliente? tipo)
    {


        return new ContactoUsuario()
        {
            Id = usuario.Id,
            Email = usuario.Email,
            Localizado = false,
            NombreCompleto = usuario.NombreAcceso,
            Rol = tipo
        };
    }
}
