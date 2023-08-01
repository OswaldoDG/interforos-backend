using comunicaciones.email;
using CouchDB.Driver.Extensions;
using Humanizer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using promodel.modelo;
using promodel.modelo.registro;
using promodel.servicios.identidad;
using promodel.servicios.SolicitudSoporte;
using System;

namespace promodel.servicios.SolicitudServicio;

public class ServicioSolicitudSoporteUsuario: IServicioSolicitudSoporteUsuario
{
    private readonly SolicitudSoporteCouchDbContext db;
    private readonly IConfiguration configuration;
    private readonly IWebHostEnvironment environment;
    private readonly IServicioEmail servicioEmail;

    public ServicioSolicitudSoporteUsuario(SolicitudSoporteCouchDbContext solicitudSoporteCouchDb, IConfiguration configuration,
                                            IWebHostEnvironment environment, IServicioEmail servicioEmail)
    {
       this.db= solicitudSoporteCouchDb;
        this.configuration = configuration;
        this.environment = environment;
        this.servicioEmail = servicioEmail;
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

        DatosPlantillaPassword data = new()
        {
            Activacion = solicitud.Id,
            Email = solicitud.Email,
            UrlBase = configuration.LeeUrlBase(),
        };

        MensajeEmail m = new()
        {
            DireccionPara = usuario.Email,
            NombrePara = usuario.Email,
            JSONData = JsonConvert.SerializeObject(data),
            PlantillaCuerpo = configuration.LeePlantillaTipoServcio(environment, solicitud),
            PlantillaTema = configuration.LeeTemaTipoServicio(solicitud)
        };

        await servicioEmail.Enviar(m);

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
