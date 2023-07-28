using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace promodel.servicios.SolicitudSoporte;

public interface IServicioSolicitudSoporteUsuario
{
    Task<string> CreaSolicitudRecuperacionContrasena(string usuarioId);
}
