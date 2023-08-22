﻿using promodel.modelo;

namespace promodel.servicios;

public interface IServicioIdentidad
{
    Task<Usuario?> UsuarioPorEmail(string Email);
    Task<Usuario?> ActualizaUsuario(Usuario usuario);
    Task<Usuario> CreaUsuario(Usuario usuario);
    Task<InvitacionRegistro?> RegistroPorId(string Id);
    Task EliminaRegistroPorId(string Id);
    Task Registro(RegistroUsuario r);
    Task<RespuestaLogin?> Login(string usuario, string contrasena,string ClienteId);
    Task<RespuestaLogin?> RefreshToken(string RefreshToken, string UsuarioId, string clienteId);
    Task<Usuario?> UsuarioPorId(string id);
    Task<Respuesta> RestablecerPassword(string UsuarioId,string ContrasenaNueva);
    Task<Respuesta> CambiarPassword(string UsuarioId, string ContrasenaActual,string ContrasenaNueva);
}
