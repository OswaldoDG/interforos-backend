using promodel.modelo.webhooks;

namespace promodel.servicios.webhooks;

/// <summary>
/// INterfaz para el servicio de procesamiento del webhook de google drive
/// </summary>
public interface IServicioGoogleDrivePushNotifications
{
    /// <summary>
    /// Inserta un elemento en el resporotorio para su proceso
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    Task<Respuesta> InsertaEvento(string ClienteId, GoogleDrivePushNotification data);

    /// <summary>
    /// DEvuelve el siguiente elemento de la cola por procesar
    /// </summary>
    /// <returns></returns>
    Task<RespuestaPayload<GoogleDrivePushNotification?>> ObtieneEventoPendiente();
    /// <summary>
    /// Finaliza el evento de proceso, si el valor OK es true lo elimina de la base de datos
    /// si es fols Marca el evento con Procesado=true y establece el Error al erro enviado
    /// </summary>
    /// <param name="Id"></param>
    /// <param name="Ok"></param>
    /// <param name="error"></param>
    /// <returns></returns>
    Task<Respuesta> FinalizaProcesamientoEvento(string Id, bool Ok, string? error);
    /// <summary>
    /// Procesa un archivo que ha sido agregado del folder de un modelo en un casting en drive, genera sus miniaturas y lo almacena como un element Media
    /// </summary>
    /// <param name="ClienteId"></param>
    /// <param name="Id"></param>
    /// <returns></returns>
    Task<Respuesta> ProcesaEventoCrear(string ClienteId, string Id);
    /// <summary>
    /// Procesa un archivo que ha sido eliminado del folder de un modelo en un casting en drive, borrando todos sus registros
    /// </summary>
    /// <param name="ClienteId"></param>
    /// <param name="Id"></param>
    /// <returns></returns>
    Task<Respuesta> ProcesaEventoEliminar(string ClienteId, string Id);

}
