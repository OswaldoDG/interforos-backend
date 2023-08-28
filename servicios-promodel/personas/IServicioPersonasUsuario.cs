using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace promodel.servicios.personas
{
    public interface IServicioPersonasUsuario
    {

        Task<Respuesta> AdicionaPersona(string personaId, string clienteId, string usuarioId, string? agenciaId = null);
        Task<Respuesta> RemuevePersona(string personaId, string clienteId, string usuarioId, string? agenciaId = null);
        Task<RespuestaPayload<List<string>>> ObtienePersonasRegistradas(string clienteId, string usuarioId, string? agenciaId = null);
    }
}
