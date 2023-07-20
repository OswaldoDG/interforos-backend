namespace promodel.modelo.castings;

/// <summary>
/// Define fechas de eventos para el casting
/// </summary>
public class EventoCasting
{
    /// <summary>
    /// Id del evento puede ser cualquier numero queno exista en la lista de eventos del casting
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Fecha y hora de inicio del evento
    /// </summary>
    public DateTime FechaInicial { get; set; }

    /// <summary>
    /// Fecha y hora de fin del evento
    /// </summary>
    public DateTime FechaFinal { get; set; }

    /// <summary>
    /// Natas para el evento, por ejemplo vestiment a traer o documentos
    /// </summary>
    public string? Notas { get; set; }

    /// <summary>
    /// Dirección del lugar
    /// </summary>
    public string? Lugar { get; set; }

    /// <summary>
    /// Coordenadas en google maps o similar
    /// </summary>
    public string? Coordenadas { get; set; }
}
