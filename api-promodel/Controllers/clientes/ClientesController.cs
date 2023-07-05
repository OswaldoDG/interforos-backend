using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using promodel.modelo.clientes;
using promodel.modelo.perfil;
using promodel.servicios;

namespace api_promodel.Controllers.clientes
{
    [Route("clientes")]
    [Authorize]
    [ApiController]
    public class ClientesController : ControllerUsoInterno
    {
        public ClientesController(IServicioClientes clientes): base (clientes) { 
        }


        [HttpGet("config", Name = "ConfiguracionCliente")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ClienteView>> GetporUrl()
        {
            if(string.IsNullOrEmpty(Request.Headers["Origin"]))
            {
                return BadRequest("Origen no válido");

            }
            var c = await servicioClientes.ClientePorUrl(Request.Headers["Origin"]);
            if (c == null)
            {
                return NotFound();
            }

            return Ok(c.ToClienteView());
        }


        [HttpGet("porurl", Name = "ClientePorUrl")]
        public async Task<ActionResult<Cliente>> GetporUrl([FromQuery] string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return BadRequest();
            }

            var c = await servicioClientes.ClientePorUrl(url);
            if(c == null)
            {
                return NotFound();
            }

            return Ok(c);
        }



        [HttpGet("contactos", Name = "ContactosCliente")]
        public async Task<ActionResult<List<ContactoUsuario>>> ObtieneContactos([FromQuery] string buscar)
        {
            if (string.IsNullOrEmpty(buscar))
            {
                return BadRequest();
            }

            var c = await servicioClientes.BuscaContactosClientePorTexto(ClienteId, buscar);
            
            return Ok(c);
        }

    }
}
