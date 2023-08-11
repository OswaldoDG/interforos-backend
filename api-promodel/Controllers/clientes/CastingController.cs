using api_promodel.middlewares;
using Bogus.DataSets;
using ImageMagick;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using promodel.modelo;
using promodel.modelo.castings;
using promodel.modelo.perfil;
using promodel.modelo.proyectos;
using promodel.modelo.registro;
using promodel.servicios;
using promodel.servicios.castings.Mock;
using promodel.servicios.proyectos;
using System.Text.Json;

namespace api_promodel.Controllers.clientes;

[ServiceFilter(typeof(ControladorAutenticadoFilter))]
[Route("api/[controller]")]
[ApiController]
public class CastingController : ControllerUsoInterno
{
    private ICastingService castingService;
    private readonly IBogusService bogus;
    private readonly IServicioIdentidad identidad;

    public CastingController(ICastingService castingService, IServicioClientes clientes, IBogusService Bogus, IServicioIdentidad servicioIdentidad) : base(clientes,servicioIdentidad)
    {
        this.castingService = castingService;
        bogus = Bogus;
        this.identidad = servicioIdentidad;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<CastingListElement>>> MisCastings([FromQuery] bool? Inactivos = false)
    {
        // El id del usaurio se obtiene del JWT 
        var result = await castingService.Casting(ClienteId, this.UsuarioId, this.RolUsuario, Inactivos.Value);

        if (result.Ok)
        {
            return Ok(result.Payload);

        }
        else
        {
            return ActionFromCode(result.HttpCode, result.Error);
        }
    }

    [HttpGet("actuales")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<List<CastingListElement>>> CastingsActuales()
    {           
        var result = await castingService.CastingsActuales(ClienteId);
        if (result.Ok)
        {
            return Ok(result.Payload);
        }
        else
        {
            return ActionFromCode(result.HttpCode, result.Error);
        }
    }


    [HttpGet("id/{id}", Name = "CastingPorId")]
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

    [HttpGet("anonimo/{id}", Name = "CastingAnonimo")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Casting>> CastingAnomino(string id)
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
    public async Task<ActionResult> ActualizaCasting([FromBody] Casting casting, string Id)
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

    [HttpPut("contactos/{castingId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<ContactoUsuario>>> ActualizaContactosCasting([FromBody] List<ContactoUsuario> contactos, string castingId)
    {
        foreach (var contacto in contactos)
        {               
      
        if (contacto.Id==null)
            {
                var usuario = new RegistroUsuario()
                {
                    Email=contacto.Email,
                    Nombre=contacto.NombreCompleto,
                    Rol=(TipoRolCliente)contacto.Rol,
                    CastingId = castingId,
                    ClienteId = this.ClienteId
                };
                await RegistroContacto(usuario);
            }
        }



        var result = await castingService.ActualizaContactosCasting(ClienteId,castingId, UsuarioId, contactos);
        if (result.Ok)
        {
            return Ok(result.Payload);

        }
        else
        {
            return ActionFromCode(result.HttpCode, result.Error);
        }
    }


    [HttpGet("{CastingId}/logo")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<string>> ObtieneLogo([FromRoute] string CastingId)
    {
        byte[] content = await castingService.ObtieneLogo(ClienteId, CastingId);

        if (content == null)
        {
            return Ok(null);
        }
        var result = "data:image/jpeg;base64," + Convert.ToBase64String(content);
        return Ok(JsonConvert.SerializeObject(result));
    }


    [HttpPut("{castingId}/logo")]
    public async Task<ActionResult> EstableceLog(string castingId, [FromBody] string image)
    {
        var defalut = 250;
        var cadena = image.Split(",");
        //Convirtiendo de base64 a Stream
        byte[] imgByte = Convert.FromBase64String(cadena[1]);
        var imgStream = new MemoryStream(imgByte,0,imgByte.Length);
        using var img = new MagickImage(imgStream);

        //Calculando Proporcion para resolucion 
        var ancho = defalut;
        var alto = (defalut * img.Height) / img.Width;

        //Cambiando resolucion
        var size = new MagickGeometry(ancho,alto);
        img.Resize(size);

        //Cmabiando Formato
        img.Quality = 80;
        img.Format = MagickFormat.Jpg;

        //Conviertiendo a Byte[]
        var logoByte = Convert.FromBase64String(img.ToBase64());
     
       
        var result = await castingService.LogoCasting(ClienteId, UsuarioId, castingId, logoByte);
        if (result.Ok)
        {
            return Ok();

        }
        else
        {
            return ActionFromCode(result.HttpCode, result.Error);
        }
    }

    [HttpPut("{castingId}/eventos")]
    public async Task<ActionResult> EstableceEventos(string castingId, [FromBody] List<EventoCasting> eventos)
    {
        // Reemplazr la totalidad de eventos en el casting a partir de los enviados a este endpoint
        var r = await castingService.ActualizaEventosCasting(ClienteId, UsuarioId, castingId, eventos);
        if (r.Ok)
        {
            return Ok();
        }
        else
        {
            return ActionFromCode(r.HttpCode, r.Error);
        }
    }


    // solo se llama cuando el usuario contacto del casting no existe
    private async Task<ActionResult> RegistroContacto(RegistroUsuario registro)
    {
        registro.ClienteId = this.ClienteId;
        if (!this.ModelState.IsValid || registro.ClienteId == null)
        {
            return BadRequest();
        }

        var u = await identidad.UsuarioPorEmail(registro.Email);
        if (u != null && u.Clientes.Any(x => x == registro.ClienteId))
        {
            return Conflict();
        }

        await identidad.Registro(registro);
    
        return Ok();
    }

    [HttpPut("{castingId}/categorias")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> EstabeceCategorias(string castingId, [FromBody] List<CategoriaCasting> categorias)
    {
        // Reemplazr la totalidad de eventos en el casting a partir de los enviados a este endpoint
        await castingService.ActualizaCategoríasCasting(ClienteId, UsuarioId, castingId, categorias);
        return Ok();
    }
    [HttpPut("{castingId}/categoria/{categoriaId}/modelo/{modeloId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> AgregarModeloCategoria(string castingId, string modeloId,string categoriaId)
    {
        var result = await castingService.AdicionarModeloCategoria(ClienteId, castingId, UsuarioId, categoriaId, modeloId, OrigenInscripcion.staff);
        if (result.Ok)
        {
            return Ok();

        }
        else
        {
            return ActionFromCode(result.HttpCode, result.Error);
        }
    }

    [HttpDelete("{castingId}/categoria/{categoriaId}/modelo/{modeloId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> RemoverModeloCataegoria(string castingId, string modeloId,string categoriaId)
    {

        var result = await castingService.EliminarModeloCategoria(ClienteId,castingId,UsuarioId,categoriaId,modeloId,OrigenInscripcion.staff);
        if (result.Ok)
        {
            return Ok();
        }
        else
        {
            return ActionFromCode(result.HttpCode, result.Error);
        }
    }

    [HttpGet("{CastingId}/selector/revisor")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<SelectorCastingCategoria>> SelectorCategoriaRevisor([FromRoute] string CastingId)
    {
        var result = await castingService.SelectorCastingCategoriaRevisor(this.ClienteId, CastingId, this.UsuarioId);
        if (result != null)
        {
            return Ok(result);
        }
        else
        {
            return StatusCode(403);
        }
    }

    [HttpGet("{CastingId}/selector")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<SelectorCastingCategoria>> SelectorCategoria([FromRoute]string CastingId)
    {
        var result = await castingService.SelectorCastingCategoria(this.ClienteId,CastingId,this.UsuarioId);
        if (result!=null)
        {
            return Ok(result);
        }
        else
        {
            return StatusCode(403);
        }
    }

    [HttpPut("{castingId}/comentarios")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> AgregaComentarioCasting(string castingId,[FromBody]string comentario)
    {
       var result =  await castingService.AdicionarComentarioCasting(ClienteId, castingId, UsuarioId, comentario);
        if (result.Ok)
        {
            return Ok();
        }
        else
        {
            return ActionFromCode(result.HttpCode, result.Error);
        }
    }

    [HttpDelete("{castingId}/comentarios/{comentarioId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> EliminaComentarioCasting(string castingId,string comentarioId)
    {
        var result = await castingService.EliminarComentarioCasting(ClienteId, castingId, UsuarioId, comentarioId);
        if (result.Ok)
        {
            return Ok();
        }
        else
        {
            return ActionFromCode(result.HttpCode, result.Error);
        }
    }

    [HttpPost("{castingId}/categoria/{categoriaId}/modelo/{modeloId}/comentario")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ComentarioCategoriaModeloCasting>> AgregaComentarioModeloCategoria(string castingId, string categoriaId, string modeloId, [FromBody] string comentario)
    { 
        if (this.RolUsuario== TipoRolCliente.RevisorExterno || this.RolUsuario == TipoRolCliente.Staff || this.RolUsuario == TipoRolCliente.Administrador)
        {

            var result = await castingService.AdicionarComentarioModeloCategoria(ClienteId, castingId, UsuarioId, categoriaId, modeloId, comentario);
            if (result.Ok)
            {
                return Ok(result.Payload);
            }
            else
            {
                return ActionFromCode(result.HttpCode, result.Error);
            }
        }
    return Unauthorized();
    }

    [HttpDelete("{castingId}/categoria/{categoriaId}/modelo/{modeloId}/comentario/{comentarioId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> EliminaComentarioModeloCategoria(string castingId, string categoriaId, string modeloId, string comentarioId)
    {
        if (this.RolUsuario == TipoRolCliente.RevisorExterno || this.RolUsuario == TipoRolCliente.Staff || this.RolUsuario == TipoRolCliente.Administrador)
        {
            var result = await castingService.EliminarComentarioModeloCategoria(this.ClienteId, castingId, UsuarioId, categoriaId, modeloId, comentarioId);
        if (result.Ok)
        {
            return Ok();
        }
        else
        {
            return ActionFromCode(result.HttpCode, result.Error);
        }
    }
        return Unauthorized();
    }

    [HttpPost("{CastingId}/categoria/{categoriaid}/modelo/{modeloId}/like/{nivel}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> VotoModelo(string modeloId, string castingId, string categoriaId, string nivel)
    {
        var result = await castingService.VotoModelo(this.UsuarioId, modeloId, this.ClienteId, castingId, categoriaId, nivel);

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

