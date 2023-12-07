using almacenamiento.GoogleDrive;
using api_promodel.Controllers.publico;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using promodel.modelo.webhooks;
using promodel.servicios.webhooks;

namespace api_promodel.Controllers.webhooks;

[Route("[controller]")]
[AllowAnonymous]
[ApiController]

public class GoogleWebhooksController : ControllerPublico
{

    private readonly IServicioGoogleDrivePushNotifications servicioGoogleDrivePush;
    private readonly IConfiguration configuracion;
    private readonly string ClienteId;

    public GoogleWebhooksController(IServicioGoogleDrivePushNotifications servicioGoogleDrivePush,IConfiguration configuracion) {
        this.servicioGoogleDrivePush = servicioGoogleDrivePush;
        this.configuracion = configuracion;
        this.ClienteId = configuracion.GetValue<string>("ClienteId");
    }

    [HttpGet("echo")]
    public async Task<IActionResult> Echo() {
        return Ok(DateTime.UtcNow);
    }
    [HttpDelete("evento/{id}")]
    public async Task<IActionResult> ProcesaEventoDelete(string id)
    {

        var resultado = await servicioGoogleDrivePush.ProcesaEventoEliminar("ed21e274ab20a8b8059db4f0d1002259", id);

        if (resultado.Ok)
        {
            return Ok();
        }
        else
        {
            // Google require 500 en caso de error para reintentar
            return StatusCode(500);
        }
    }

    [HttpGet("evento/{id}")]
    public async Task<IActionResult> ProcesaEventoCrear(string id)
    {

        var resultado = await servicioGoogleDrivePush.ProcesaEventoCrear("ed21e274ab20a8b8059db4f0d1002259", id);

        if (resultado.Ok)
        {
            return Ok();
        }
        else
        {
            // Google require 500 en caso de error para reintentar
            return StatusCode(500);
        }
    }

    /// <summary>
    /// Procesa un cambio al adicionar recursos a un folder especifico
    /// Los encabezados fueron tomados de https://developers.google.com/drive/api/guides/push
    /// </summary>
    /// <param name="channelId"></param>
    /// <param name="messageNumber"></param>
    /// <param name="resourceId"></param>
    /// <param name="resourceState"></param>
    /// <param name="resourceUri"></param>
    /// <param name="chnages"></param>
    /// <param name="chennelExpiration"></param>
    /// <param name="channelToken"></param>
    /// <returns></returns>
    [HttpPost("drivechange")]
    public async Task<IActionResult> PostDriveChangeEvent(
        [FromHeader(Name = "X-Goog-Channel-ID")] Guid channelId,
        [FromHeader(Name = "X-Goog-Message-Number")] long messageNumber,
        [FromHeader(Name = "X-Goog-Resource-ID")] string resourceId,
        [FromHeader(Name = "X-Goog-Resource-State")] string resourceState,
        [FromHeader(Name = "X-Goog-Resource-URI")] string resourceUri,
        [FromHeader(Name = "X-Goog-Changed")] string? changes,
        [FromHeader(Name = "X-Goog-Channel-Expiration")] string? chennelExpiration,
        [FromHeader(Name = "X-Goog-Channel-Token")] string? channelToken
        )
    {

        // Convertir al modelo leyendo los valores de los headers
        GoogleDrivePushNotification evento = new ()
        {
            ChannelId = channelId,
            MessageNumber = messageNumber,
            ResourceId = resourceId,
            ChannelExpiration = chennelExpiration,
            ChannelToken = channelToken,
            Procesado = false,
            ResourceUri = resourceUri,
            ResourceState = (ReourceState)Enum.Parse(typeof(ReourceState), resourceState)
        };

        if (!string.IsNullOrEmpty(changes)) {
            evento.Changes = new List<ResourceChanges>();
            changes.Split(',').ToList().ForEach(c =>
            {
                evento.Changes.Add((ResourceChanges)Enum.Parse(typeof(ResourceChanges), c));
            });
        }

        var resultado = await servicioGoogleDrivePush.InsertaEvento(this.ClienteId,evento);

        if (resultado.Ok)
        {
            return Ok();
        }
        else {
            // Google require 500 en caso de error para reintentar
            return StatusCode(500);
        }
        
    }

}
