using api_promodel.Controllers.publico;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using promodel.modelo;
using promodel.servicios;
using promodel.servicios.proyectos;

namespace api_promodel.Controllers
{
    [Route("registro")]
    [ApiController]
    [Authorize]
    public class RegistroController : ControllerPublico
    {

        private IServicioIdentidad identidad;
        private readonly IServicioClientes clientes;
        private readonly ICastingService castingService;

        public RegistroController(IServicioIdentidad identidad, IServicioClientes clientes,ICastingService castingService) : base()
        {
            this.identidad = identidad; ;
            this.clientes = clientes;
            this.castingService = castingService;
        }


        /// <summary>
        /// Crea un usuario y emite una notificación para la confirmación de la cuenta
        /// </summary>
        /// <param name="usuario"></param>
        /// <returns></returns>
        [HttpPost(Name = "CrearRegistro")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult> Registro([FromBody] RegistroUsuario registro)
        {
            registro.ClienteId = this.ClienteId;
            if (!this.ModelState.IsValid || registro.ClienteId == null)
            {
                return BadRequest();
            }

            var u = await identidad.UsuarioPorEmail(registro.Email);
            if(u != null && u.Clientes.Any(x=>x == registro.ClienteId))
            {
                return Conflict();
            }

            await identidad.Registro(registro);

            return Ok();
        }




        [HttpGet("{id}", Name = "RegistroPorId")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<InvitacionRegistro>> RegistrPorId(string id)
        {

            if (ClienteId == null)
            {
                return BadRequest();
            }

            var r = await identidad.RegistroPorId(id);
            if (r != null)
            {
                if (r.LimiteUso >= DateTime.UtcNow && r.Registro.ClienteId == ClienteId)
                {
                    return Ok(r.Sanitiza());

                }
            }

            return NotFound();
        }

        [HttpPost("completar/{id}", Name = "CompletarRegistro")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> CompletarRegistro(string id, [FromBody] CreacionUsuario usuario)
        {

            if (ClienteId == null || !ModelState.IsValid)
            {
                return BadRequest();
            }

            var r = await identidad.RegistroPorId(id);
            if (r != null)
            {
                if (r.LimiteUso >= DateTime.UtcNow && r.Registro.ClienteId == ClienteId
                    && r.Registro.Email.Equals(usuario.Email, StringComparison.InvariantCultureIgnoreCase))
                {

                    var u = await identidad.UsuarioPorEmail(r.Registro.Email);
                    if (u == null)
                    {
                        
                        var usuarioNuevo = r.AUsuario();
                        if (usuarioNuevo != null)
                        {
                            usuarioNuevo.HashContrasena = SecretHasher.Hash(usuario.Contrasena);
                            usuarioNuevo.NombreUsuario = usuario.NombreUsuario;
                            usuarioNuevo = await identidad.CreaUsuario(usuarioNuevo);
                        }
                    }
                    else
                    {
                        if (!u.RolesCliente.Any(x => x.ClienteId == r.Registro.ClienteId && x.Rol == r.Registro.Rol))
                        {
                            u.Clientes.Add(r.Registro.ClienteId);
                            u.RolesCliente.Add(new promodel.modelo.registro.RolCliente() { ClienteId = r.Registro.ClienteId, Rol = r.Registro.Rol });
                            await identidad.ActualizaUsuario(u);
                        }
                    }

                    var user = await identidad.UsuarioPorEmail(r.Registro.Email);

                    if (r.Registro.CastingId != null && user !=null)
                    {
                        var casting = await castingService.ObtieneCasting(this.ClienteId, r.Registro.CastingId, this.UsuarioId);

                        var contacto = casting.Contactos.FirstOrDefault(_ => _.Email == user.Email);

                        if (contacto != null)
                        {
                            contacto.Confirmado = true;
                            contacto.UsuarioId = user.Id;
                            contacto.NombreUsuario = user.NombreUsuario;
                        }

                        await castingService.ActualizaCasting(this.ClienteId, this.UsuarioId, casting.Id, casting);
                    }




                    await identidad.EliminaRegistroPorId(r.Id);

                    return Ok();
                }
            }

            return NotFound();
        }
    }
}
