

using Newtonsoft.Json;
using static promodel.modelo.castings.ConstantesCasting;

namespace promodel.modelo.castings;
public  class BitacoraCasting
{
    /// <summary>
    /// Usuario  en sesion que realizao el evento
    /// </summary>
    [JsonProperty("uid")]

    public string? UsuarioId;

    /// <summary>
    /// Fecha en que se realizo el evento
    /// </summary>
    [JsonProperty("f")]

    public DateTime Fecha;

    /// <summary>
    /// Tipo de evento
    /// </summary>
    [JsonProperty("t")]

    public TipoEventoCasting Evento;

    /// <summary>
    /// Descripcion general del evento
    /// </summary>
    [JsonProperty("text")]

    public string? TextoEvento;

}
