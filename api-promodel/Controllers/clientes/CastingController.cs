using api_promodel.middlewares;
using Microsoft.AspNetCore.Mvc;
using promodel.modelo.castings;
using promodel.modelo.proyectos;
using promodel.servicios;
using promodel.servicios.proyectos;

namespace api_promodel.Controllers.clientes
{
    [ServiceFilter(typeof(ControladorAutenticadoFilter))]
    [Route("api/[controller]")]
    [ApiController]
    public class CastingController : ControllerUsoInterno
    {
        private ICastingService castingService;
        public CastingController(ICastingService castingService, IServicioClientes clientes) : base(clientes)
        {
            this.castingService = castingService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<CastingListElement>>> MisCastings([FromQuery] bool? Inactivos = false)
        {
            var result = await castingService.Casting(ClienteId, Inactivos.Value);
            if(result.Ok)
            {
                return Ok(result.Payload);

            } else
            {
                return ActionFromCode(result.HttpCode, result.Error);
            }
        }

        [HttpGet("$id")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Casting>> FullCastingPorId(string id)
        {
            var result = await castingService.FullCasting(ClienteId, id, UsuarioId);
            if (result.Ok)
            {
                return Ok(result.Payload);

            }
            else
            {
                return ActionFromCode(result.HttpCode, result.Error);
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Casting>> CrearCasting([FromBody] Casting  casting)
        {
            var result = await castingService.CreaCasting(ClienteId, UsuarioId, casting);
            if (result.Ok)
            {
                return Ok(result.Payload);

            }
            else
            {
                return ActionFromCode(result.HttpCode, result.Error);
            }
        }


        [HttpPut("{Id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Casting>> ActualizaCasting([FromBody] Casting casting, string Id)
        {
            //if (casting.Id != Id)
            //{
            //    return BadRequest();
            //}

            var result = await castingService.ActualizaCasting(ClienteId, UsuarioId, Id, casting);
            if (result.Ok)
            {
                return Ok();

            }
            else
            {
                return ActionFromCode(result.HttpCode, result.Error);
            }
        }

        [HttpDelete("{CastingId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<CastingListElement>> EliminarCasting([FromRoute] string CastingId)
        {

            var result = await castingService.EliminarCasting(ClienteId, UsuarioId, CastingId);
            if (result.Ok)
            {
                return Ok();

            }
            else
            {
                return ActionFromCode(result.HttpCode, result.Error);
            }
        }



    }
}
