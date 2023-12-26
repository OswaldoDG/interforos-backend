using promodel.modelo.webhooks;
using promodel.servicios;
using promodel.servicios.webhooks;
using Serilog;

namespace api_promodel.BackGroundServices;

public class GoogleDriveBS: BackgroundService
{
    private readonly TimeSpan _period = TimeSpan.FromSeconds(5);
    private readonly IServicioGoogleDrivePushNotifications _servicioGoogleDrivePush;
    private readonly IConfiguration configuracion;
    private bool EnEjecucionServicioGoogleDrivePush = false;
    private string ClienteId = null;

    public GoogleDriveBS(IServicioGoogleDrivePushNotifications servicioGoogleDrivePush, IConfiguration configuracion)
    {
        _servicioGoogleDrivePush = servicioGoogleDrivePush;
        this.configuracion = configuracion;
        this.ClienteId = configuracion.GetValue<string>("ClienteId")
;        Log.Debug("Iniciando servicio GoogleDriveBS");
    }

    protected override async Task ExecuteAsync(CancellationToken stopToken)
    {
        using PeriodicTimer timer = new PeriodicTimer(_period);
        while (!stopToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stopToken))
        {

            if (!EnEjecucionServicioGoogleDrivePush)
            {
                EnEjecucionServicioGoogleDrivePush = true;
                await ProcesaColaGoogleDrive();
                EnEjecucionServicioGoogleDrivePush = false;
            }
        }
    }

    private async Task ProcesaColaGoogleDrive()
    {
        try
        {
            var resultado = await _servicioGoogleDrivePush.ObtieneEventoPendiente();
            if (resultado.Ok)
            {
                if (resultado.Payload != null)
                {
                    var evento = (GoogleDrivePushNotification)resultado.Payload!;
                    Log.Debug($"Procesando evento {evento.Id}");
                    Respuesta respuesta = new Respuesta();
                    switch (evento.ResourceState)
                    {
                        case ReourceState.sync:
                            respuesta.Ok = true;
                            respuesta.Error = "Sincronisacion de Canal";
                            break;
                            
                        case ReourceState.update:
                            respuesta = await _servicioGoogleDrivePush.ProcesaEventoCrear(this.ClienteId, evento.ResourceUri.Split("files/")[1].Split("?")[0]);
                            break;
                        case ReourceState.trash:
                            respuesta = await _servicioGoogleDrivePush.ProcesaEventoEliminar(this.ClienteId, evento.ResourceUri.Split("files/")[1]);
                            break;
                        default:
                            break;
                    }

                    await _servicioGoogleDrivePush.FinalizaProcesamientoEvento(evento.Id, respuesta.Ok, respuesta.Error);
                }
                else
                {
                    Log.Debug($"No hay eventos de GoogleDrive pendientes");
                }
            }
            else
            {
                Log.Debug($"Error al obtener eventos {resultado.HttpCode} {resultado.Error}");
            }
        }
        catch (Exception ex)
        {
            Log.Debug($"Error al ejecutar GoogleDriveBS {ex}");
        }     
    }
}
