using comunicaciones.email;
using CouchDB.Driver.Extensions;
using Humanizer;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using promodel.modelo;
using promodel.modelo.registro;
using promodel.servicios.SolicitudSoporte;
using System;

namespace promodel.servicios.SolicitudServicio;

public class ServicioSolicitudSoporteUsuario: IServicioSolicitudSoporteUsuario
{
    private readonly SolicitudSoporteCouchDbContext db;
    public ServicioSolicitudSoporteUsuario(SolicitudSoporteCouchDbContext solicitudSoporteCouchDb)
    {
       this.db= solicitudSoporteCouchDb;
    }

    public async Task<string> CreaSolicitudRecuperacionContrasena(Usuario usuario)
    {
        SolicitudSoporteUsuario solicitud = new SolicitudSoporteUsuario()
        {
             FechaEnvio = DateTime.UtcNow,
             Id = Guid.NewGuid().ToString(),
             UsuarioId = usuario.Id,
             Email=usuario.Email,
             FechaLimiteConfirmacion = DateTime.UtcNow.AddHours(1),
             Tipo =  TipoServicio.RecuperacionContrasena
        };

        // Aqui vamos a enviar el correo para el usuario    

        CrearSoporteUsuario(solicitud);

        return solicitud.Id;
    }

    private async Task CrearSoporteUsuario(SolicitudSoporteUsuario solicitud)
    {
        await db.SoporteUsuario.AddOrUpdateAsync(solicitud);       
    }

    public async Task<SolicitudSoporteUsuario?> SolicitudPorId(string Id)
    {
        SolicitudSoporteUsuario? solicitud = await db.SoporteUsuario.FirstOrDefaultAsync(x => x.Id == Id);
        return solicitud;
    }

    public async Task EliminaSolicitudPorId(string Id)
    {
        var solicitud = await db.SoporteUsuario.FirstOrDefaultAsync(x => x.Id == Id);
        if (solicitud != null)
        {
            await db.SoporteUsuario.RemoveAsync(solicitud);
        }
    }

}
