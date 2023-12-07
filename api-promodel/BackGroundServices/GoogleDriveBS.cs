using promodel.modelo.webhooks;
using promodel.servicios.webhooks;
using Serilog;

namespace api_promodel.BackGroundServices;

public class GoogleDriveBS: BackgroundService
{
    private readonly TimeSpan _period = TimeSpan.FromSeconds(5);
    private readonly IServicioGoogleDrivePushNotifications _servicioGoogleDrivePush;
    private bool EnEjecucionServicioGoogleDrivePush = false;

    public GoogleDriveBS(IServicioGoogleDrivePushNotifications servicioGoogleDrivePush)
    {
        _servicioGoogleDrivePush = servicioGoogleDrivePush;
        Log.Debug("Iniciando servicio GoogleDriveBS");
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
