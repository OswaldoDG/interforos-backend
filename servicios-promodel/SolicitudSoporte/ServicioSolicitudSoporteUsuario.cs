using promodel.modelo.registro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace promodel.servicios.SolicitudServicio;

public class ServicioSolicitudSoporteUsuario
{

    public async Task<string> CreaSolicitudRecuperacionContrasena(string usuarioId)
    {
        SolicitudSoporteUsuario solicitud = new SolicitudSoporteUsuario()
        {
             FechaEnvio = DateTime.UtcNow,
             Id = Guid.NewGuid().ToString(),
             UsuarioId = usuarioId,
             FechaLimiteConfirmacion = DateTime.UtcNow.AddHours(1),
             Tipo =  TipoServicio.RecuperacionContrasena
        };

        // Aqui vamos a enviar el correo para el usuario

        CrearSoporteUsuario(solicitud);

        return solicitud.Id;
    }

    private Task CrearSoporteUsuario(SolicitudSoporteUsuario solicitud)
    {
        throw new NotImplementedException();
        // Añade la solicitud a la base de datos
    }

}
