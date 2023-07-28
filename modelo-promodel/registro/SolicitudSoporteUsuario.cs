using CouchDB.Driver.Types;

namespace promodel.modelo.registro;

public enum TipoServicio
{
    RecuperacionContrasena = 1
}

/// <summary>
/// Clase para realizar la solicitue de servicios para los usuarios
/// </summary>
public class SolicitudSoporteUsuario: CouchDocument
{

    /// <summary>
    /// ID del usuario solicitante
    /// </summary>
    public string UsuarioId { get; set; }
    
    /// <summary>
    /// Tipo de servicio solicitado
    /// </summary>
    public TipoServicio Tipo { get; set; }
    
    /// <summary>
    /// Email de envio de la confirmación
    /// </summary>
    public string Email { get; set; }
    
    /// <summary>
    /// Fecha de envío de la confirmación
    /// </summary>
    public DateTime FechaEnvio { get; set; }
    
    /// <summary>
    /// FEcha límite para realizar la confirmación
    /// </summary>
    public DateTime? FechaLimiteConfirmacion { get; set; }

}
