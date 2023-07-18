using promodel.modelo;
using promodel.modelo.perfil;

namespace promodel.servicios;

public interface IServicioIdentidad
{
    Task<Usuario?> UsuarioPorEmail(string Email);
    Task<Usuario?> ActualizaUsuario(Usuario usuario);
    Task<Usuario> CreaUsuario(Usuario usuario);
    Task<InvitacionRegistro?> RegistroPorId(string Id);
    Task EliminaRegistroPorId(string Id);
    Task Registro(RegistroUsuario r);
    Task<RespuestaLogin?> Login(string usuario, string contrasena);
    Task<RespuestaLogin?> RefreshToken(string RefreshToken, string UsuarioId);
    Task<Usuario?> UsuarioPorId(string id);
}
