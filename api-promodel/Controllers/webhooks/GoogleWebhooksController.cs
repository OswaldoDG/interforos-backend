using almacenamiento;
using almacenamiento.GoogleDrive;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using promodel.modelo.webhooks;
using promodel.servicios.webhooks;

namespace api_promodel.Controllers.webhooks
{
    [Route("[controller]")]
    [AllowAnonymous]
    [ApiController]
    public class GoogleWebhooksController : ControllerBase
    {
        private readonly IServicioGoogleDrivePushNotifications servicioGoogleDrivePush;
        private readonly IAlmacenamiento almacenamiento;
        public GoogleWebhooksController(IServicioGoogleDrivePushNotifications servicioGoogleDrivePush, IAlmacenamiento almacenamiento )
        {
            this.servicioGoogleDrivePush = servicioGoogleDrivePush;
            this.almacenamiento = almacenamiento;
        }

        [HttpGet("token")]
        public async Task<IActionResult> Echo()
        {
            var r  = await almacenamiento.ObtieneToken();
            return Ok(r);
        }



        [HttpGet("evento/{id}")]
        public async Task<IActionResult> ProcesaEvento(string id)
        {
            // 1. Llamar con el Id de google drive de un archivo en el fodler de un modelos que pertenezca a un casting 
            // 2. Obetener el Id del folder padre y almacenarlo como FolderModeloId
            // 3. Utilizando FolderModeloId obtener el Id del folder padre y almacenarlo como FolderCastingId
            // 4. Obtener el casting utilizando como clave el valor de FolderCastingId, si el casting no existe detener el proceso
            // 5. Obener el Id el modelo utilizando el valor FolderModeloId en un query a las categorías y modelos asociados del casting, si no existe un modelo  con  fodler id = FolderModeloId detener el proceso
            // 6. Obener el thumbnail de archivo ya sea imagen o video y almacenarlo para su consulta y despliegue por la UI de la manera habitual
            // 7. Si es una fotografí actualizar en ModeloCasting la propiedad ImagenPortadaId al id del archivo 
            // 8. Si es un video actualizar en ModeloCasting la propiedad VideoPortadaId al id del archivo 
            // 9. Fin del proceso 
            return Ok();
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
            GoogleDrivePushNotification evento = new()
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

            if (!string.IsNullOrEmpty(changes))
            {
                evento.Changes = new List<ResourceChanges>();
                changes.Split(',').ToList().ForEach(c =>
                {
                    evento.Changes.Add((ResourceChanges)Enum.Parse(typeof(ResourceChanges), c));
                });
            }

            var resultado = await servicioGoogleDrivePush.InsertaEvento(evento);

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

    }
}
