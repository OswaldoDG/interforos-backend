
using System.Text.Json.Serialization;


namespace promodel.modelo.castings;

public class ConstantesCasting
{
    /// <summary>
    /// Define el tipo de evento ocurrido en el casting
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TipoEventoCasting
    {
        CastingCreado = 0, CastingAbierto = 1, CastingCerrado = 2, CastingCancelado = 3, ModeloAdicionado = 10, ModeloEliminado = 11, VotoRecibido = 20, ComentarioModeloRecibido = 30, ComentarioModeloEliminado = 40
    }
}
