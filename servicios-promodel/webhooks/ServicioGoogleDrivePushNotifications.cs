using promodel.modelo.webhooks;

namespace promodel.servicios.webhooks;

public class ServicioGoogleDrivePushNotifications : IServicioGoogleDrivePushNotifications
{

    public ServicioGoogleDrivePushNotifications(GoogleDriveDbContext db) { 
    }

    /// <summary>
    /// Inserta un evento del webhoo para proceso asincrono
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public async Task<Respuesta> InsertaEvento(GoogleDrivePushNotification data)
    {
        Respuesta r = new ();
        try
        {
            // Para insertar validar si no esiste un elemento con el mismo channelId y MessageNumber
            // Si no existe Insertar y devolver Ok, 
            // Si ya existe simplmente devolver OK
            r.Ok = true;
        }
        catch (Exception ex)
        {

            r.Ok = false;
            r.Error = ex.Message;

        }
        return r;
    }

    public Task<RespuestaPayload<GoogleDrivePushNotification?>> ObtieneEventoPendiente()
    {
        // Debe devolver el siguiente evento de procesamiento cuyo valor Procesado = false o null si no se licaliza
        return Task.FromResult (new RespuestaPayload<GoogleDrivePushNotification?>() {  Ok = true, Payload = null });
    }

    public Task<Respuesta> FinalizaProcesamientoEvento(string Id, bool Ok, string? error)
    {
        /// Finaliza el evento de proceso, si el valor OK es true lo elimina de la base de datos
        /// si es fols Marca el evento con Procesado=true y establece el Error al erro enviado
        throw new NotImplementedException();
    }

}
