using Amazon.Runtime.Internal.Util;
using Bogus.DataSets;
using CouchDB.Driver.Extensions;
using Flurl.Util;
using ImageMagick;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Org.BouncyCastle.Asn1.Cms;
using promodel.modelo;
using promodel.modelo.castings;
using promodel.modelo.clientes;
using promodel.modelo.perfil;
using promodel.modelo.proyectos;
using promodel.servicios.castings;
using SendGrid;
using System;
using System.Diagnostics.Contracts;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Reflection.Metadata;
using static System.Net.Mime.MediaTypeNames;

namespace promodel.servicios.proyectos;

public class CastingService : ICastingService
{

    private readonly CastingCouchDbContext db;
    private readonly IDistributedCache cache;
    private readonly IServicioIdentidad identidad;
    private readonly HttpClient httpClient;
    private readonly IConfiguration configuration;

    public CastingService(CastingCouchDbContext db, IDistributedCache cache, 
        IServicioIdentidad servicioIdentidad, HttpClient httpClient, IConfiguration configuration)
    {
        this.db = db;
        this.cache = cache;
        this.identidad = servicioIdentidad;
        this.httpClient = httpClient;
        this.configuration = configuration;
    }

    public async Task<Casting?> ObtieneCasting(string CLienteId, string CastingId, string UsuarioId)
    {
        return await db.Castings.FirstOrDefaultAsync(x => x.ClienteId == CLienteId && x.Id == CastingId);
    }

    #region Castings
    public async Task<RespuestaPayload<CastingListElement>> Casting(string ClienteId, string usuarioId, TipoRolCliente rol, bool incluirInactivos = false)
    {
        var r = new RespuestaPayload<CastingListElement>();
        var castings = new List<Casting>();
        var castingsResult = new List<CastingListElement>();
        switch (rol)
        {
            case TipoRolCliente.Administrador:
                castings = await CastingsAdministrador(ClienteId, incluirInactivos);
                break;
            case TipoRolCliente.Staff:
                castings = await CastingsStaffRevisor(usuarioId, incluirInactivos);
                break;
            case TipoRolCliente.RevisorExterno:
                castings = await CastingsStaffRevisor(usuarioId, incluirInactivos);
                break;
            default:
                castings = null;
                break;
        }

        if (castings != null)
        {
            castings.ForEach(casting =>
            {

                castingsResult.Add(casting.aCastingListElement(rol));
            });

            r.Payload = castingsResult;
        }
        r.Ok = true;
        return r;
    }

    protected async Task<List<Casting>> CastingsAdministrador(string ClienteId, Boolean incluirInactivos)

    {
        if (incluirInactivos)
        {
            return await db.Castings.Where(c => c.ClienteId == ClienteId).ToListAsync();
        }
        else
        {
            return await db.Castings.Where(c => c.ClienteId == ClienteId && c.Activo == true).ToListAsync();
        }
    }
    protected async Task<List<Casting>> CastingsStaffRevisor(string usuarioId, Boolean incluirInactivos)
    {
        if (incluirInactivos)
        {
            return await db.Castings.Where(c => c.Contactos.Any(x => x.UsuarioId == usuarioId)).ToListAsync();
        }
        else
        {
            return await db.Castings.Where(c => c.Contactos.Any(x => x.UsuarioId == usuarioId) && c.Activo == true).ToListAsync();
        }
    }


    public async Task<RespuestaPayload<Casting>> FullCasting(string ClienteId, string CastingId, string UsuarioId)
    {
        var r = new RespuestaPayload<Casting>();
        var casting = await ObtieneCasting(ClienteId, CastingId, UsuarioId);
        if (casting != null)
        {
            r.Payload = casting;
            r.Ok = true;
        }
        else
        {
            r.HttpCode = HttpCode.NotFound;
        }
        return r;
    }

    public async Task<RespuestaPayload<Casting>> CreaCasting(string ClienteId, string UsuarioId, Casting casting)
        {
        var r = new RespuestaPayload<Casting>();
        casting.Id = Guid.NewGuid().ToString();
        casting.FechaCreacionTicks = DateTime.UtcNow.Ticks;
        casting.ClienteId = ClienteId;
        casting.UsuarioId = UsuarioId;
        casting.Contactos = new List<ContactoCasting>();
        casting.Categorias = new List<CategoriaCasting>();
        await db.Castings.AddOrUpdateAsync(casting);
        r.Ok = true;
        r.Payload = casting;
        return r;
    }

    public async Task<Respuesta> ActualizaCasting(string ClienteId, string UsuarioId, string CastingId, Casting casting)
            
    {
        var r = new Respuesta();
        var tmpCasting = await ObtieneCasting(ClienteId, CastingId, UsuarioId);
        if (tmpCasting != null)
        {
            tmpCasting.Descripcion = casting.Descripcion;
            tmpCasting.Nombre = casting.Nombre;
            tmpCasting.NombreCliente = casting.NombreCliente;
            tmpCasting.FechaApertura = casting.FechaApertura;
            tmpCasting.FechaCierre = casting.FechaCierre;
            tmpCasting.AceptaAutoInscripcion = casting.AceptaAutoInscripcion;
            tmpCasting.Contactos = casting.Contactos;
            tmpCasting.Categorias=casting.Categorias;
            await db.Castings.AddOrUpdateAsync(tmpCasting);
            r.Ok = true;
        }
        else
        {
            r.HttpCode = HttpCode.NotFound;
        }

        return r;
    }

    public async Task<Respuesta> EliminarCasting(string ClienteId, string CastingId, string UsuarioId)
    {
        var r = new Respuesta();

        var casting = await ObtieneCasting(ClienteId, CastingId, UsuarioId);
        if (casting != null)
        {
            await db.Castings.RemoveAsync(casting);
            r.Ok = true;
        }
        else
        {
            r.HttpCode = HttpCode.NotFound;
        }

        return r;
    }

    public async Task<Respuesta> EstadoCasting(string ClienteId, string CastingId, string UsuarioId, bool Activo)
    {
        var r = new Respuesta();

        var casting = await ObtieneCasting(ClienteId, CastingId, UsuarioId);
        if (casting != null)
        {
            casting.Activo = Activo;
            await db.Castings.AddOrUpdateAsync(casting);
            r.Ok = true;
        }
        else
        {
            r.HttpCode = HttpCode.NotFound;
        }

        return r;
    }

    public async Task<Respuesta> AdicionarColaboradoresCasting(string ClienteId, string CastingId, string UsuarioId, List<string> ColaboradoresIds)
    {
        var r = new Respuesta();
        var casting = await ObtieneCasting(ClienteId, CastingId, UsuarioId);
        if (casting != null)
        {
            bool actualizar = false;
            foreach (string Id in ColaboradoresIds)
            {
                if (!casting.ColaboradoresIds.Any(x => x.Equals(Id)))
                {
                    casting.ColaboradoresIds.Add(Id);
                    actualizar = true;
                }
            }
            if (actualizar)
            {
                await db.Castings.AddOrUpdateAsync(casting);
                r.Ok = true;
            }
        }
        return r;
    }

    public async Task<Respuesta> RemoverColaboradoresCasting(string ClienteId, string CastingId, string UsuarioId, List<string> ColaboradoresIds)
    {
        var r = new Respuesta();
        var casting = await ObtieneCasting(ClienteId, CastingId, UsuarioId);
        if (casting != null)
        {
            bool actualizar = false;
            foreach (string Id in ColaboradoresIds)
            {
                if (casting.ColaboradoresIds.Any(x => x.Equals(Id)))
                {
                    casting.ColaboradoresIds.Remove(Id);
                    actualizar = true;
                }
            }
            if (actualizar)
            {
                await db.Castings.AddOrUpdateAsync(casting);
                r.Ok = true;
            }
        }
        return r;
    }

    public async Task<RespuestaPayload<Casting>> ActualizaContactosCasting(string ClienteId, string CastingId, string UsuarioId, List<ContactoUsuario> Contactos)
    {
        var r = new RespuestaPayload<Casting>();
        var casting = await ObtieneCasting(ClienteId, CastingId, UsuarioId);

        if (casting == null)
        {
            r.HttpCode = HttpCode.NotFound;
            r.Error = "Casting no encontrado";
            return r;
        }
        casting.Contactos = new List<ContactoCasting>();
        foreach (var contacto in Contactos)
        {
            var user = await identidad.UsuarioPorEmail(contacto.Email);

            if (user == null)
            {
                casting.Contactos.Add(contacto.aContactoCasting(null));
            }

            else
            {
                casting.Contactos.Add(contacto.aContactoCasting(user.UltimoAcceso));
            }

        }
        await ActualizaCasting(ClienteId, UsuarioId, casting.Id, casting);
        r.Ok = true;
        r.Payload = casting;
        return r;
    }

    public async Task<RespuestaPayload<CastingListElement>> CastingsActuales(string CLienteId)
    {
        var r = new RespuestaPayload<CastingListElement>();
        var castingsResult = new List<CastingListElement>();
        var castings = await db.Castings
            .Where(_ => _.Activo == true && _.AceptaAutoInscripcion == true && _.FechaApertura >= DateTime.Now && _.FechaCierre > DateTime.Now).ToListAsync();

        if (castings != null)
        {
            castings.ForEach(casting =>
            {
                castingsResult.Add(casting.aCastingListElement());
            });

            r.Payload = castingsResult.OrderBy(_ => _.FechaApertura);
        }
        r.Ok = true;
        return r;
    }
    #endregion


    #region Cartegorias
    public async Task<RespuestaPayload<CategoriaCasting>> ActualizarCategoria(string ClienteId, string CastingId, string UsuarioId, string CategoriaId, CategoriaCasting categoria)
    {
        var r = new RespuestaPayload<CategoriaCasting>();

        var casting = await ObtieneCasting(ClienteId, CastingId, UsuarioId);
        if (casting == null)
        {
            r.HttpCode = HttpCode.NotFound;

        }
        else
        {
            var categoriaExistente = casting.Categorias.FirstOrDefault(x => x.Id == CategoriaId);
            if (categoriaExistente == null)
            {
                r.HttpCode = HttpCode.NotFound;
            }
            else
            {
                categoriaExistente.Nombre = categoria.Nombre;
                categoriaExistente.Descripcion = categoria.Descripcion;
                await db.Castings.AddOrUpdateAsync(casting);
                r.Ok = true;
            }
        }

        return r;
    }


    public async Task<RespuestaPayload<CategoriaCasting>> CrearCategoria(string ClienteId, string CastingId, string UsuarioId, CategoriaCasting categoria)
    {
        var r = new RespuestaPayload<CategoriaCasting>();

        var casting = await ObtieneCasting(ClienteId, CastingId, UsuarioId);
        if (casting == null)
        {
            r.HttpCode = HttpCode.NotFound;

        }
        else
        {
            categoria.Id = Guid.NewGuid().ToString();
            casting.Categorias.Add(categoria);
            await db.Castings.AddOrUpdateAsync(casting);
            r.Payload = categoria;
            r.Ok = true;
        }

        return r;
    }

    public async Task<Respuesta> EliminarCategoria(string ClienteId, string CastingId, string UsuarioId, string CategoríaId)
    {
        var r = new Respuesta();

        var casting = await ObtieneCasting(ClienteId, CastingId, UsuarioId);
        if (casting == null)
        {
            r.HttpCode = HttpCode.NotFound;
        }
        else
        {
            var categoria = casting.Categorias.FirstOrDefault(x => x.Id == CategoríaId);
            if (categoria == null)
            {
                r.HttpCode = HttpCode.NotFound;
            }
            else
            {
                casting.Categorias.Remove(categoria);
                await db.Castings.AddOrUpdateAsync(casting);
                r.Ok = true;
            }
        }

        return r;
    }

    #endregion

    #region ModelosCategoria

    public async Task<Respuesta> AdicionarModeloCategoria(string ClienteId, string CastingId, string UsuarioId, string CategoríaId, string PersonaId, OrigenInscripcion origen)
    {
        var r = new Respuesta();
        var casting = await ObtieneCasting(ClienteId, CastingId, UsuarioId);
        if (casting != null)
        {
            var categoria = casting.Categorias.FirstOrDefault(c => c.Id == CategoríaId);
            if (categoria != null && !categoria.Modelos.Any(x => x.PersonaId == PersonaId))
            {
                categoria.Modelos.Add(new ModeloCasting() { PersonaId = PersonaId, Origen = origen });
                await db.Castings.AddOrUpdateAsync(casting);
                r.Ok = true;
            }
        }
        return r;
    }
    public async Task<Respuesta> EliminarModeloCategoria(string ClienteId, string CastingId, string UsuarioId, string CategoríaId, string PersonaId, OrigenInscripcion origen)
    {
        var r = new Respuesta();
        var casting = await ObtieneCasting(ClienteId, CastingId, UsuarioId);
        if (casting != null)
        {
            var categoria = casting.Categorias.FirstOrDefault(c => c.Id == CategoríaId);
            if (categoria != null)
            {
                var modelo = categoria.Modelos.FirstOrDefault(x => x.PersonaId == PersonaId);
                if (modelo != null)
                {
                    categoria.Modelos.Remove(modelo);
                    await db.Castings.AddOrUpdateAsync(casting);
                    r.Ok = true;
                }
            }
        }
        return r;
    }


    #endregion

    #region Comentarios
    public async Task<RespuestaPayload<ComentarioCasting>> AdicionarComentarioCasting(string ClienteId, string CastingId, string UsuarioId, string Comentario)
    {
        var r = new RespuestaPayload<ComentarioCasting>();
        var casting = await ObtieneCasting(ClienteId, CastingId, UsuarioId);
        if (casting != null)
        {
            var c = new ComentarioCasting() { Comentario = Comentario, UsuarioId = UsuarioId };
            casting.Comentarios.Add(c);
            await db.Castings.AddOrUpdateAsync(casting);
            r.Payload = c;
            r.Ok = true;

        }
        return r;
    }

    public async Task<Respuesta> EliminarComentarioCasting(string ClienteId, string CastingId, string UsuarioId, string ComentarioId)
    {
        var r = new Respuesta();
        var casting = await ObtieneCasting(ClienteId, CastingId, UsuarioId);
        if (casting != null)
        {
            var comentario = casting.Comentarios.FirstOrDefault(c => c.Id == ComentarioId && c.UsuarioId == UsuarioId);
            if (comentario != null)
            {
                casting.Comentarios.Remove(comentario);
                await db.Castings.AddOrUpdateAsync(casting);
                r.Ok = true;
            }
        }
        return r;
    }


    public async Task<RespuestaPayload<ComentarioCasting>> AdicionarComentarioModeloCategoria(string ClienteId, string CastingId, string UsuarioId, string CategoríaId, string PersonaId, string Comentario)
    {
        var r = new RespuestaPayload<ComentarioCasting>();
        var casting = await ObtieneCasting(ClienteId, CastingId, UsuarioId);
        if (casting != null)
        {
            var categoria = casting.Categorias.FirstOrDefault(x => x.Id == CategoríaId);
            if (categoria != null)
            {
                var modelo = categoria.Modelos.FirstOrDefault(m => m.PersonaId.Equals(PersonaId));
                if (modelo != null)
                {
                    var comentario = new ComentarioCasting() { Comentario = Comentario, UsuarioId = UsuarioId };
                    modelo.Comentarios.Add(comentario);
                    await db.Castings.AddOrUpdateAsync(casting);
                    r.Payload = comentario;
                    r.Ok = true;
                }
            }
        }
        return r;
    }



    public async Task<Respuesta> EliminarComentarioModeloCategoria(string ClienteId, string CastingId, string UsuarioId, string CategoríaId, string PersonaId, string ComentarioId)
    {
        var r = new Respuesta();
        var casting = await ObtieneCasting(ClienteId, CastingId, UsuarioId);
        if (casting != null)
        {
            var categoria = casting.Categorias.FirstOrDefault(x => x.Id == CategoríaId);
            if (categoria != null)
            {
                var modelo = categoria.Modelos.FirstOrDefault(m => m.PersonaId.Equals(PersonaId));
                if (modelo != null)
                {
                    var comentario = modelo.Comentarios.FirstOrDefault(x => x.Id == ComentarioId && x.UsuarioId == UsuarioId);
                    if (comentario != null)
                    {
                        modelo.Comentarios.Remove(comentario);
                        await db.Castings.AddOrUpdateAsync(casting);
                        r.Ok = true;
                    }
                }
            }
        }
        return r;
    }

    public async Task<Respuesta> LogoCasting(string CLienteId, string UsuarioId, string CastingId, byte[] imagenByte)
    {
        string pahchFolder = @$".\LogoTemp\{Guid.NewGuid()}";
        string patch = @$"{pahchFolder}\logo.jpg";


        try
        {
            if (!Directory.Exists(pahchFolder))
            {
                Directory.CreateDirectory(pahchFolder);
            }
            var fichero = File.Create(patch);
            fichero.Write(imagenByte, 0, imagenByte.Length);
            fichero.Close();     
        }
        catch (Exception e)
        {
            Console.WriteLine("Exception: " + e.Message);
        }
        var r = new Respuesta();
        var casting = await db.Castings.FirstOrDefaultAsync(x => x.ClienteId == CLienteId && x.Id == CastingId);

        if (casting != null)
        {
            casting.Attachments.AddOrUpdate(patch, MediaTypeNames.Text.Plain);
            await db.Castings.AddOrUpdateAsync(casting);
            r.Ok = true;
            Directory.Delete(pahchFolder,true);
            return r;    
        }

        
        r.HttpCode = HttpCode.BadRequest;
        r.Error = "No se pudo guardar logo";
        Directory.Delete(pahchFolder,true);
        return r;

    }

    public Task ActualizaEventosCasting(string CLienteId, string UsuarioId, string CastingId, List<EventoCasting> eventos)
    {
        throw new NotImplementedException();
    }

    public async Task<Respuesta> ActualizaCategoríasCasting(string CLienteId, string UsuarioId, string CastingId, List<CategoriaCasting> categorias)
    {
        var r = new Respuesta();
        var casting = await ObtieneCasting(CLienteId, CastingId, UsuarioId);

        if (casting == null)
        {
            r.HttpCode = HttpCode.NotFound;
            r.Error = "Casting no encontrado";
            return r;
        }
        // se verifica si hay categorias a actualizar o nuevas , y elimina las removidas
        casting.ActulizarCategorias(categorias);
        await ActualizaCasting(CLienteId, UsuarioId, casting.Id, casting);
        r.Ok = true;
        return r;
    }

    public async Task<byte[]> ObtieneLogo(string ClienteId, string CastingId)
    {
        string URL = configuration.GetValue<string>("promodeldrivers:couchdb:endpoint");
        string User = configuration.GetValue<string>("promodeldrivers:couchdb:username");
        string Pass = configuration.GetValue<string>("promodeldrivers:couchdb:password");   
        string url = URL +"/proyectos/" + CastingId + "/logo.jpg";         

        var base64String = Convert.ToBase64String(
           System.Text.Encoding.ASCII.GetBytes($"{User}:{Pass}"));
        httpClient.DefaultRequestHeaders.Add("Accept", "application/octet-stream");
        httpClient.DefaultRequestHeaders.Add("Authorization", "Basic " + base64String);

        var response = await httpClient.GetAsync(url);
        if (response.IsSuccessStatusCode){
            var result = await response.Content.ReadAsByteArrayAsync();

            return result;
        }
        return null;      
    }
    #endregion
    #region Acceso
    #endregion
}
