using api_promodel.Controllers.publico;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using promodel.modelo;
using promodel.modelo.perfil;
using promodel.servicios;

namespace api_promodel.Controllers
{
    [Route("acceso")]
    [ApiController]
    [Authorize]
    public class AccesoController : ControllerPublico
    {

        private IServicioIdentidad identidad;
        private readonly IServicioClientes clientes;
        public AccesoController(IServicioIdentidad identidad, IServicioClientes clientes) : base()
        {
            this.identidad = identidad; ;
            this.clientes = clientes;
        }


        [AllowAnonymous]
        [HttpPost("login", Name = "Login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<RespuestaLogin>> Authenticate(SolicitudAcceso solicitud)
        {
            var response = await identidad.Login(solicitud.Usuario, solicitud.Contrasena);
            if (response != null)
            {
                return Ok(response);
            }
            return Unauthorized();
        }


        
        [HttpPost("token-refresh/{token}", Name = "RefreshToken")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<RespuestaLogin>> TokenRefresh(string token)
        {
            if(UsuarioId != null)
            {
                var response = await identidad.RefreshToken(token, UsuarioId);
                if (response != null)
                {
                    return Ok(response);
                }
            }
            return Unauthorized();
        }

    }
}
