using CouchDB.Driver.Types;
using Newtonsoft.Json;

namespace promodel.modelo.webhooks;

public class GoogleDrivePushNotification: CouchDocument
{
    [JsonProperty("cid")]
    public Guid ChannelId { get; set; }

    [JsonProperty("n")]
    public long MessageNumber { get; set; }
    
    [JsonProperty("rid")]
    public string ResourceId { get; set; }

    [JsonProperty("s")]
    public ReourceState ResourceState { get; set; }

    [JsonProperty("u")]
    public string ResourceUri { get; set; }

    [JsonProperty("cs")]
    public List<ResourceChanges>? Changes { get; set; }

    [JsonProperty("x")]
    public string? ChannelExpiration { get; set; }

    [JsonProperty("t")]
    public string? ChannelToken { get; set; }

    /// <summary>
    /// Determina si el evento de la cola ya ha sido procesado
    /// </summary>
    [JsonProperty("p")]
    public bool Procesado { get; set; } = false;

    /// <summary>
    /// Indica el error ocurrido durante el proceso
    /// </summary>
    [JsonProperty("err")]
    public string? Error { get; set; }
}
