using api_promodel.Controllers.publico;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using promodel.modelo;
using promodel.modelo.registro;
using promodel.servicios;
using promodel.servicios.SolicitudSoporte;

namespace api_promodel.Controllers;

[Route("acceso")]
[ApiController]
[Authorize]
public class AccesoController : ControllerPublico
{

    private IServicioIdentidad identidad;
    private readonly IServicioClientes clientes;
    private readonly IServicioSolicitudSoporteUsuario servicioSoporte;

    public AccesoController(IServicioIdentidad identidad, IServicioClientes clientes, IServicioSolicitudSoporteUsuario servicioSolicitudSoporteUsuario) : base()
    {
        this.identidad = identidad; ;
        this.clientes = clientes;
        this.servicioSoporte = servicioSolicitudSoporteUsuario;
    }


    [AllowAnonymous]
    [HttpPost("login", Name = "Login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<RespuestaLogin>> Authenticate(SolicitudAcceso solicitud)
    {
        var response = await identidad.Login(solicitud.Usuario, solicitud.Contrasena,this.ClienteId);
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
    public async Task<ActionResult<Respuesta>> RecuperarContrasena([FromBody] string email)
    {
        Respuesta respuesta = new Respuesta();

        var usuario = await identidad.UsuarioPorEmail(email);
        if (usuario==null)
        {
            respuesta.HttpCode = HttpCode.NotFound;
            respuesta.Error = "No existe usuario";
            return respuesta;
        }

        await servicioSoporte.CreaSolicitudRecuperacionContrasena(usuario);

        respuesta.Ok = true;
        return Ok(respuesta);
    }

    [AllowAnonymous]
    [HttpPost("password/id/{id}", Name = "RestablecerContrasena")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> RestablecerContrasena([FromRoute] string id ,[FromBody] string nuevoPassword)
    {
        var r = await servicioSoporte.SolicitudPorId (id);
        if (r != null)
        {
            var usuario = await identidad.UsuarioPorId(r.UsuarioId);

            if (usuario != null && r.FechaLimiteConfirmacion >= DateTime.UtcNow && r.Tipo == TipoServicio.RecuperacionContrasena
                && r.Email.Equals(usuario.Email, StringComparison.InvariantCultureIgnoreCase) )
            {
                await identidad.RestablecerPassword(r.UsuarioId,nuevoPassword);
                await servicioSoporte.EliminaSolicitudPorId(r.Id);
                return Ok();
            }
        }
        return NotFound();
    }
    [AllowAnonymous]
    [HttpGet("password/{id}", Name = "Solicitud")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SolicitudSoporteUsuario>> ExisteSolicitud([FromRoute] string id)
    {
        var r = await servicioSoporte.SolicitudPorId(id);
        if (r != null)
        {                    
           return Ok(r);
        }
        return NotFound();
    }


    [HttpPost("token-refresh/{token}", Name = "RefreshToken")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [AllowAnonymous]
    public async Task<ActionResult<RespuestaLogin>> TokenRefresh(string token)
    {
        if(UsuarioId != null)
        {
            var response = await identidad.RefreshToken(token, UsuarioId,this.ClienteId);
            if (response != null)
            {
                return Ok(response);
            }
        }
        return Unauthorized();
    }

    [HttpPost("password/establecer", Name = "EstablecerContrasena")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> EstablecerContrasena([FromBody] EstablecerContrasena data)
    {
        var r = await identidad.CambiarPassword(this.UsuarioId, data.Actual, data.Nueva);
        if (r.Ok)
        {
            return Ok();
        }
        return BadRequest(r);
    }
}
