namespace promodel.modelo.clientes;

/// <summary>
/// Aceptacion del consentimiento por parte del usuario
/// </summary>
public class AceptacionConsentimiento
{
    /// <summary>
    /// Identificador único del consentimiento
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Fecha en la que fue aceptado el cosentimiento
    /// </summary>
    public DateTime FechaAceptacion { get; set; }
}
