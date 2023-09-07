using Newtonsoft.Json;

namespace promodel.modelo.clientes;

/// <summary>
/// Consentimientos del usuario por cliente
/// </summary>
public class Consentimiento
{

    /// <summary>
    /// Consentimiento que se muestra al modelo cuando confirma su cuenta
    /// </summary>
    public const string ConsentimientoRegistroModelo = "c-modelo";

    /// <summary>
    /// Consentimiento que se muestra al usaurio de agencia cuando confirma su cuenta
    /// </summary>

    public const string ConsentimientoRegistroAgencia = "c-agencia";

    /// <summary>
    /// Consentimiento que se muestra al activar la funcionalidad para la alta de modelos 
    /// </summary>
    public const string ConsentimientoAltaModelos = "c-altamodelos";

    /// <summary>
    /// Identificador único del consentimiento
    /// </summary>
    [JsonProperty("id")]
    public string Id { get; set; }

    /// <summary>
    /// Idioma del texto de consentimiento
    /// </summary>
    [JsonProperty("i")]
    public string Idioma { get; set; }

    /// <summary>
    /// Define si el cosentimiento es el default de idioma para el contenido de hmtl 
    /// si no existe el idioma solicitado
    /// </summary>
    [JsonProperty("di")]
    public bool IdiomaDefault { get; set; }

    /// <summary>
    /// Título para el cosentimiento
    /// </summary>
    [JsonProperty("t")]
    public string Titulo { get; set; }

    /// <summary>
    /// Contenido HTML del consentimiento
    /// </summary>
    [JsonProperty("c")]
    public string ContenidoHTML { get; set; }
}
