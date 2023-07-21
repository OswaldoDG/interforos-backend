using promodel.modelo;
using promodel.modelo.castings;
using promodel.modelo.perfil;
using promodel.modelo.proyectos;

namespace promodel.servicios.proyectos;

public interface ICastingService
{
    Task<RespuestaPayload<CastingListElement>> Casting(string ClienteId, string usuarioId, TipoRolCliente rol, bool incluirInactivos);
    Task<Respuesta> EliminarCasting(string ClienteId, string CastingId, string UsuarioId);
    Task<Respuesta> EstadoCasting(string ClienteId, string CastingId, string UsuarioId, bool Activo);
    Task<RespuestaPayload<Casting>> CreaCasting(string ClienteId, string UsuarioId, Casting casting);
    Task<Respuesta> ActualizaCasting(string ClienteId, string UsuarioId, string CastingId, Casting casting);
    Task<RespuestaPayload<Casting>> FullCasting(string ClienteId, string CastingId, string UsuarioId);
    Task<Respuesta> EliminarCategoria(string ClienteId, string CastingId, string UsuarioId, string CategoríaId);
    Task<RespuestaPayload<CategoriaCasting>> CrearCategoria(string ClienteId, string CastingId, string UsuarioId, CategoriaCasting categoria);
    Task<RespuestaPayload<CategoriaCasting>> ActualizarCategoria(string ClienteId, string CastingId, string UsuarioId, string CategoriaId, CategoriaCasting categoria);
    Task<Respuesta> EliminarModeloCategoria(string ClienteId, string CastingId, string UsuarioId, string CategoríaId, string PersonaId, OrigenInscripcion origen);
    Task<Respuesta> AdicionarModeloCategoria(string ClienteId, string CastingId, string UsuarioId, string CategoríaId, string PersonaId, OrigenInscripcion origen);
    Task<Respuesta> EliminarComentarioModeloCategoria(string ClienteId, string CastingId, string UsuarioId, string CategoríaId, string PersonaId, string ComentarioId);
    Task<RespuestaPayload<ComentarioCasting>> AdicionarComentarioModeloCategoria(string ClienteId, string CastingId, string UsuarioId, string CategoríaId, string PersonaId, string Comentario);
    Task<Respuesta> EliminarComentarioCasting(string ClienteId, string CastingId, string UsuarioId, string ComentarioId);
    Task<RespuestaPayload<ComentarioCasting>> AdicionarComentarioCasting(string ClienteId, string CastingId, string UsuarioId, string comentario);
    Task<Respuesta> AdicionarColaboradoresCasting(string ClienteId, string CastingId, string UsuarioId, List<string> ColaboradoresIds);
    Task<Respuesta> RemoverColaboradoresCasting(string ClienteId, string CastingId, string UsuarioId, List<string> ColaboradoresIds);
    Task<RespuestaPayload<Casting>> ActualizaContactosCasting(string ClienteId, string CastingId, string UsuarioId, List<ContactoUsuario> Contactos);
    Task<Casting?> ObtieneCasting(string CLienteId, string CastingId, string UsuarioId);
    Task<RespuestaPayload<CastingListElement>> CastingsActuales(string CLienteId);

    Task LogoCasting(string CLienteId, string UsuarioId, string CastingId, string imagenbase64);
    Task ActualizaEventosCasting(string CLienteId, string UsuarioId, string CastingId, List<EventoCasting> eventos);
}


