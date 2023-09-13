using promodel.modelo.castings;
using static promodel.modelo.castings.ConstantesCasting;

namespace promodel.servicios.BitacoraCastings;

public  interface IServicioBitacoraCasting
{
    Task <Respuesta>CrearBitacora(string clienteId, string castingId, string usuarioId, TipoEventoCasting evento, string? TextoEvento);
    Task<RespuestaPayload<List<BitacoraCasting>>> LeerBitacora(string clienteId, string castingId, string usuarioId);
}
