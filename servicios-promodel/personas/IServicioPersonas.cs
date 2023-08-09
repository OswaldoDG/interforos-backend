using promodel.modelo.castings;
using promodel.modelo.perfil;
using promodel.modelo.proyectos;
using promodel.servicios.comunes;

namespace promodel.servicios;

public interface IServicioPersonas
{
    Task<ResponsePaginado<Persona>> BuscarPersonas(RequestPaginado<BusquedaPersonas> busqueda);
    Task<ResponsePaginado<Persona>> BuscarPersonasId(RequestPaginado<BusquedaPersonasId> busqueda);
    Task<bool> EstableceFotoPrincipal(string UsuarioId, string ElementoId );
    Task<RespuestaPayload<Persona>> PorUsuarioId(string Id);
    Task<InformacionPerfil?> PerfilCliente(string UsuarioId, string ClienteId);
    Task<RespuestaPayload<Persona>> Crear(Persona persona);
    Task<RespuestaPayload<Persona>> Actualizar(Persona persona);
    Task<RespuestaPayload<Persona>> PorId(string Id);
    Task<Respuesta> Elmiminar(string Id);
    Task<bool> EliminarLinkDocumento(string CLienteId, string UsuarioId, string DocumentoId);
    Task<bool> UpsertLinkDocumento(string CLienteId, string UsuarioId, string DocumentoId, string AlmacenamientoId);
}
