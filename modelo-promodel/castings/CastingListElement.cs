namespace promodel.modelo.castings;

/// <summary>
/// DTO para despliegue de castings en la UI
/// </summary>
public class CastingListElement
{
    /// <summary>
    /// Identificador unico del casting
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Nombre del castinf
    /// </summary>

    public string Nombre { get; set; }


    /// <summary>
    /// NOmbre del cliente para quien va el proyecto por ejemplo la emrpesa
    /// </summary>
    public string NombreCliente { get; set; }

   
    /// <summary>
    /// FEcha opcional de apertura a los candidatos y revisores del proyecto
    /// </summary>
    public DateTime? FechaApertura { get; set; }

    /// <summary>
    /// Fecha opcional de cierre del proyecto 
    /// </summary>
    public DateTime? FechaCierre { get; set; }

    /// <summary>
    /// Muestra el estado del casting.
    /// </summary>
    public EstadoCasting Status { get; set; } = EstadoCasting.EnEdicion;

    /// <summary>
    /// DEtrmina si acepta inscripcion por los modelos
    /// </summary>
    public bool AceptaAutoInscripcion { get; set; } = false;

    /// <summary>
    /// Indica si el proyecto se encuentra activo
    /// </summary>
    public bool Activo { get; set; } = true;

    /// <summary>
    /// Indica si el proyecto se marca como activo en base a su fecha de apertura
    /// </summary>
    public bool AperturaAutomatica { get; set; } = true;

    /// <summary>
    /// Especifica si el proyecto debe marcarse como inactivo al superarse la fecha de cierro
    /// </summary>
    public bool CierreAutomatico { get; set; } = true;

    /// <summary>
    /// Rol del usuario actual
    /// </summary>
    public TipoRolCliente Rol { get; set; }
}
