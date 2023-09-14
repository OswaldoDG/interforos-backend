using promodel.modelo.castings;
using promodel.servicios.proyectos;


namespace promodel.servicios.BitacoraCastings;

public class ServicioBitacoraCasting : IServicioBitacoraCasting
{
    private readonly BitacoraCastingCouchDbContext db;
    private readonly ICastingService castingService;

    public ServicioBitacoraCasting(BitacoraCastingCouchDbContext db,ICastingService castingService)
    {
        this.db = db;
        this.castingService = castingService;
    }
    public async Task<Respuesta> CrearBitacora(string clienteId,string castingId, string usuarioId, ConstantesCasting.TipoEventoCasting evento, string? TextoEvento)
    {
        var r = new Respuesta();

        var casting = await castingService.ObtieneCasting(clienteId, castingId, usuarioId);

      if(casting!=null)
        {
            var bitacora = await db.Bitacoras.FindAsync(castingId);

            var Evento = new BitacoraCasting()
            {
                UsuarioId = usuarioId,
                Evento = evento,
                Fecha = DateTime.UtcNow,
                TextoEvento = TextoEvento
            };

            if (bitacora == null)
            {
                var bitacoraCasting = new HistorialCasting() { Id = castingId, Nombre = casting.Nombre, Eventos = new List<BitacoraCasting>() { Evento } };
                db.Bitacoras.AddAsync(bitacoraCasting);
            }
            else
            {
                bitacora.Eventos.Add(Evento);
                db.Bitacoras.AddOrUpdateAsync(bitacora);
            }
            r.Ok = true;
        }
        else
        {
            r.HttpCode = HttpCode.NotFound;
            r.Error = "El casting no Existe";
        }
        return r;
        
    }


    public async Task<RespuestaPayload<List<BitacoraCasting>>> LeerBitacora(string clienteId, string castingId, string usuarioId)
    {
        var r = new RespuestaPayload<List<BitacoraCasting>>();

        var casting = await castingService.ObtieneCasting(clienteId, castingId, usuarioId);

        if (casting != null)
        {
         if(casting.Contactos.Any(_=>_.UsuarioId==usuarioId && _.Rol!=modelo.TipoRolCliente.Modelo))
            {
                var bitacora = await db.Bitacoras.FindAsync(castingId);       

                if (bitacora == null)
                {
                    r.Payload = new List<BitacoraCasting>();
                }
                else
                {
                    r.Payload = bitacora.Eventos;
                }
                r.Ok = true;
            }
            
        }
        else
        {
            r.HttpCode = HttpCode.Forbidden;
            r.Error = "Sin Acceso";
        }
        return r;
    }
}
