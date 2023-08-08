using Newtonsoft.Json;


namespace promodel.modelo.castings;

public  class BusquedaPersonasId
{
    /// <summary>
    /// ID del cliente asociado a la persona para buscar
    /// </summary>
    public string? ClienteId { get; set; }


    /// <summary>
    /// Ids para buscar
    /// </summary>
    [JsonProperty("ids")]
    public List<string>? Ids { get; set; }
}
