using Amazon.Runtime.Internal.Util;
using promodel.modelo;
using promodel.modelo.castings;
using promodel.modelo.clientes;
using promodel.modelo.perfil;
using promodel.modelo.proyectos;
using System.Reflection;

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
    Task<RespuestaPayload<CategoriaCasting>> ActualizarCategoria(string ClienteId, string CastingId, string UsuarioId, string CategoriaId, CategoriaCasting categoria);
    Task<Respuesta> EliminarModeloCategoria(string ClienteId, string CastingId, string UsuarioId, string CategoríaId, string PersonaId, OrigenInscripcion origen);
    Task<Respuesta> AdicionarModeloCategoria(string ClienteId, string CastingId, string UsuarioId, string CategoríaId, string PersonaId, OrigenInscripcion origen);
    Task<Respuesta> EliminarComentarioModeloCategoria(string ClienteId, string CastingId, string UsuarioId, string CategoríaId, string PersonaId, string ComentarioId);
    Task<RespuestaPayload<ComentarioCategoriaModeloCasting>> AdicionarComentarioModeloCategoria(string ClienteId, string CastingId, string UsuarioId, string CategoríaId, string PersonaId, string Comentario);
    Task<Respuesta> EliminarComentarioCasting(string ClienteId, string CastingId, string UsuarioId, string ComentarioId);
    Task<RespuestaPayload<ComentarioCasting>> AdicionarComentarioCasting(string ClienteId, string CastingId, string UsuarioId, string comentario);
    Task<Respuesta> AdicionarColaboradoresCasting(string ClienteId, string CastingId, string UsuarioId, List<string> ColaboradoresIds);
    Task<Respuesta> RemoverColaboradoresCasting(string ClienteId, string CastingId, string UsuarioId, List<string> ColaboradoresIds);
    Task<RespuestaPayload<List<ContactoUsuario>>> ActualizaContactosCasting(string ClienteId, string CastingId, string UsuarioId, List<ContactoUsuario> Contactos);
    Task<Casting?> ObtieneCasting(string CLienteId, string CastingId, string UsuarioId);
    Task<RespuestaPayload<CastingListElement>> CastingsActuales(string CLienteId);
    Task<Respuesta> LogoCasting(string CLienteId, string UsuarioId, string CastingId, byte[] imagenByte);
    Task<Respuesta> ActualizaCategoríasCasting(string CLienteId, string UsuarioId, string CastingId, List<CategoriaCasting> categorias);
    Task<Respuesta> ActualizaEventosCasting(string CLienteId, string UsuarioId, string CastingId, List<EventoCasting> eventos);
    Task<byte[]> ObtieneLogo(string ClienteId, string CastingId);
    Task<SelectorCastingCategoria> SelectorCastingCategoria(string ClienteId,string CastingId,string UsuarioId);
    Task<SelectorCastingCategoria> SelectorCastingCategoriaRevisor(string ClienteId, string CastingId, string UsuarioId, TipoRolCliente rol);
    Task<RespuestaPayload<VotoModeloCategoria>> VotoModelo(string userId, string modeloId, string clienteId, string castingId, string categoriaId, string nivel);
    Task<Respuesta> InscripcionCasting(string ClienteId, string PersonaId, string CastingId, string CategoriaId, bool Abandonar, string UsuarioId);
    Task<RespuestaPayload<List<string>>> CategoriasModeloCasting(string ClienteId, string CastingId, string PersonaId, string UsuarioId);
    Task<Respuesta> EstablecerEstadoCasting(string clienteId, string usuarioId, string castingId, EstadoCasting estado, TipoRolCliente Rol);
    Task<string?> NombreActivo(string ClienteId, string UsuarioId,string castingId);
    Task<RespuestaPayload<Casting>> FullCastingByFolderId(string ClienteId, string FolderId);
    Task<Respuesta> ActualizarModeloCasting(string ClienteId, string castingId, string categoriaId, ModeloCasting Modelo);
    Task<Respuesta> AdicionarModeloCategoriaConsecutivo(string ClienteId, string CastingId, string UsuarioId, string CategoríaId, int consecutivo, OrigenInscripcion origen);
}


