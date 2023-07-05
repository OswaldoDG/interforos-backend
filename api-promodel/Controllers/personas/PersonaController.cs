﻿using almacenamiento;
using api_promodel.Controllers.publico;
using Bogus;
using ImageMagick;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using promodel.modelo.perfil;
using promodel.modelo.proyectos;
using promodel.servicios;
using promodel.servicios.comunes;
using promodel.servicios.perfil;
using System.Net;

namespace api_promodel.Controllers
{
    [Route("persona")]
    [ApiController]
    [Authorize]
    public class PersonaController : ControllerPublico
    {

        private readonly IServicioPersonas personas;
        private readonly IServicioCatalogos catalogos;
        private readonly IAlmacenamiento almacenamiento;


        public PersonaController(IServicioPersonas personas, IServicioCatalogos catalogos, 
            IAlmacenamiento almacenamiento,  IServicioClientes servicioClientes) : base(servicioClientes)
        {
            this.personas = personas; 
            this.catalogos = catalogos;
            this.almacenamiento = almacenamiento;
        }

        [HttpGet("mi",Name = "MiPerfile")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Persona>> MiPerfil()
        {
            var r = await personas.PorUsuarioId(this.UsuarioId);
            if (r.Ok)
            {
                return Ok(r.Payload);
            } else
            {
                if(r.HttpCode == HttpCode.NotFound)
                {
                    Persona persona = new Persona();
                    var rPersona = await CrearPersona(persona);
                    if(rPersona.Ok)
                    {
                        return Ok(rPersona.Payload);
                    } else
                    {
                        return ActionFromCode(rPersona.HttpCode, rPersona.Error);
                    }
                }
            }
            return ActionFromCode(r.HttpCode, r.Error);
        }


        [HttpPost("buscar", Name = "BuscarPersonas")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ResponsePaginado<Persona>>> BuscarPersonas([FromBody] RequestPaginado<BusquedaPersonas> busqueda)
        {
            busqueda.Request.ClienteId = this.ClienteId;
            return Ok(await personas.BuscarPersonas(busqueda));
        }

        [HttpGet("{usuarioid}", Name = "ObtenerPersonaUsuarioId")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Persona>> PurUsuarioId(string usuarioid)
        {
            var r = await personas.PorUsuarioId(usuarioid);
            if (r.Ok)
            {
                return Ok(r.Payload);
            }

            return ActionFromCode(r.HttpCode, r.Error);
        }


        private async Task<RespuestaPayload<Persona>> CrearPersona(Persona persona)
        {
            persona.UsuarioId = this.UsuarioId;
            if (persona.Clientes == null || persona.Clientes.Count==0)
            {
                persona.Clientes = new List<string>() { this.ClienteId };
            }
            else
            {
                if (!persona.Clientes.Any(x => x == ClienteId))
                {
                    persona.Clientes.Add(ClienteId);
                }
            }

            var folderId = await CreaFolderAlmacenamiento(persona);
            persona.FolderContenidoId = folderId;

            var r = await personas.Crear(persona);

            return r;

        }


        [HttpPost(Name = "CrearPersona")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Persona>> Crear([FromBody] Persona persona)
        {
            var r= await CrearPersona(persona);
            if(r.Ok)
            {
                return CreatedAtAction(nameof(PurUsuarioId), new { usuarioid = ((Persona)r.Payload).UsuarioId}, r.Payload);
            }

            return ActionFromCode(r.HttpCode, r.Error);
        }

        [HttpPut("{Id}", Name = "ActualizarPersona")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> Actualizar(string Id, [FromBody] Persona persona)
        {
            if (!ModelState.IsValid || persona.Id != Id) { return BadRequest(ModelState); }

            if (persona.Clientes == null || persona.Clientes.Count == 0)
            {
                persona.Clientes = new List<string>() { this.ClienteId };
            }
            else
            {
                if (!persona.Clientes.Any(x => x == ClienteId))
                {
                    persona.Clientes.Add(ClienteId);
                }
            }

            var r = await personas.Actualizar(persona);
            if (r.Ok)
            {
                int consecutivo = persona.Consecutivo.HasValue ? persona.Consecutivo.Value : 1;
                string nuevoNombre = $"{consecutivo.ToString().PadLeft(6, '0')} {persona.Nombre} {persona.Apellido1} {persona.Apellido2} ({persona.NombreArtistico})";
                await almacenamiento.RemameFolder(ClienteId, ((Persona)r.Payload).FolderContenidoId, nuevoNombre);
                return Ok();
            }

            return ActionFromCode(r.HttpCode, r.Error);
        }

        [HttpDelete("{Id}", Name = "ElmiminarPersona")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> Eliminar(string Id)
        {
            var r = await personas.Elmiminar(Id);
            if (r.Ok)
            {
                return Ok();
            }

            return ActionFromCode(r.HttpCode, r.Error);
        }


        [HttpGet("catalogo/{tipo}", Name = "ObtenerCatalogo")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<CatalogoBase>> Catalogo(string tipo)
        {

            if(string.IsNullOrEmpty(this.ClienteId))
            {
                return BadRequest();
            }

            var r = await catalogos.GetCatalogoCliente(tipo, this.ClienteId);

            if(r==null)
            {
                return NotFound();
            }

            return Ok(r);
        }


        [HttpGet("catalogo/perfil", Name = "ObtenerCatalogosPerfil")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<CatalogoBase>>> CatalogosPerfil()
        {

            if (string.IsNullOrEmpty(this.ClienteId))
            {
                return BadRequest();
            }

            var r = await catalogos.GetCatalogosPerfil(this.ClienteId);

            if (r == null)
            {
                return NotFound();
            }

            return Ok(r);
        }



        [HttpGet("perfilusuario", Name = "PerfilUsuario")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<InformacionPerfil>> PerfilUsuario()
        {

            if(string.IsNullOrEmpty(UsuarioId) || string.IsNullOrEmpty(ClienteId))
            {
                return BadRequest();
            }

            var p = await personas.PerfilCliente(UsuarioId, ClienteId);
            if(p== null)
            {
                return NotFound();
            }

            return Ok(p);
        }


        private async Task<string> CreaFolderAlmacenamiento(Persona persona)
        {
            int consecutivo = persona.Consecutivo.HasValue ? persona.Consecutivo.Value : 1;
            almacenamiento.NombreValidoFolder($"{consecutivo.ToString().PadLeft(6,'0')} {persona.Nombre} {persona.Apellido1} {persona.Apellido2} ({persona.NombreArtistico})");

            string nombre;
            if (string.IsNullOrEmpty( persona.Nombre))
            {
                nombre = persona.UsuarioId;
            } else
            {
                nombre = $"{persona.Nombre} {persona.Apellido1} {persona.Apellido2} ({persona.NombreArtistico})";
            }


            var f = await this.almacenamiento.FindFolder(ClienteId, nombre);
            if (f == null)
            {
                f = await this.almacenamiento.CreateFolder(ClienteId, nombre);
            }

            if (f == null)
            {
                throw new Exception("No fue posible crear el folder de almacenamiento");
            }

            return f.Id;
        }
    }
}