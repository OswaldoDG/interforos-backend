using Newtonsoft.Json;

namespace promodel.modelo.castings;

/// <summary>
/// Determina los permisos de visualización de los elementos del casting
/// </summary>
public class PermisosCasting
{
    /// <summary>
    /// Determina si pueden verse las redes sociales del modelo
    /// </summary>
    [JsonProperty("rs")]
    public bool VerRedesSociales { get; set; } = false;

    /// <summary>
    /// Determina si pueden verse el teléfono del modelo
    /// </summary>
    [JsonProperty("tel")]
    public bool VerTelefono { get; set; } = false;


    /// <summary>
    /// Determina si pueden verse la dirección del modelo
    /// </summary>
    [JsonProperty("dir")]
    public bool VerDireccion { get; set; } = false;

    /// <summary>
    /// Determina si pueden verse el email del modelo
    /// </summary>
    [JsonProperty("em")]
    public bool VerEmail { get; set; } = false;

    /// <summary>
    /// Determina si pueden verse la lista de habilidades del modelo
    /// </summary>
    [JsonProperty("ha")]
    public bool VerHabilidades { get; set; } = false;

    /// <summary>
    /// Determina si pueden verse lso dtos generales del modelo
    /// </summary>
    [JsonProperty("ge")]
    public bool VerDatosGenerales { get; set; } = false;

    /// <summary>
    /// Determina si pueden verse la galería personal del modelo
    /// </summary>
    [JsonProperty("ga")]
    public bool VerGaleriaPersonal { get; set; } = false;


    /// <summary>
    /// Define si los externos pueden ver y crear comntarios
    /// </summary>
    [JsonProperty("co")]
    public bool VerComentarios { get; set; } = false;
}
