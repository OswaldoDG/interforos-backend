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

        [AllowAnonymous]
        [HttpPost("password", Name = "RecuperarContrasena")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RespuestaLogin>> RecuperarContrasena([FromBody] string email)
        {
            // LLamar al servicio ServicioSolicitudSoporteUsuario.CreaSolicitudRecuperacionContrasena una vz que se obtenga el id del usuario por email

            // Si no se encuentra el email y su usuario asociado devolver 404

            // SOLO DEBE REGRESAR OK pues la notificación legará vía corero eletronico
            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("password/{id}", Name = "RestablecerContrasena")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<RespuestaLogin>> RestablecerContrasena([FromBody] string nuevoPassword)
        {
            // LLamar al servicio ServicioSolicitudSoporteUsuario.CreaSolicitudRecuperacionContrasena una vz que se obtenga el id del usuario por email

            // Si no se encuentra la confirmación en la nueva base de couch buscando pr su si devolver 404

            // Si la contraseña NO cumple con los requerimientos devolver 400 

            // Si la contraseña cumple con los requerimientos realizar el cambio y eliminar el registro de couchbase


            // SOLO DEBE REGRESAR OK pues la notificación legará vía corero eletronico
            return Ok();
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
