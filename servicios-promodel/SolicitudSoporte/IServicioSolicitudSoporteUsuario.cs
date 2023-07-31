using promodel.modelo;
using promodel.modelo.registro;

namespace promodel.servicios.SolicitudSoporte;

public interface IServicioSolicitudSoporteUsuario
{
    Task<string> CreaSolicitudRecuperacionContrasena(Usuario usuario);
    Task<SolicitudSoporteUsuario?> SolicitudPorId(string Id);
    Task EliminaSolicitudPorId(string Id);
}
