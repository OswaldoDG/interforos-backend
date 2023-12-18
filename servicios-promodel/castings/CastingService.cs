using almacenamiento;
using almacenamiento.GoogleDrive;
using Bogus.DataSets;
using CouchDB.Driver.Extensions;
using EllipticCurve.Utils;
using Google.Apis.Drive.v3;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using promodel.modelo;
using promodel.modelo.castings;
using promodel.modelo.clientes;
using promodel.modelo.media;
using promodel.modelo.perfil;
using promodel.modelo.proyectos;
using promodel.servicios.castings;
using promodel.servicios.media;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Reflection;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using ImageMagick;
using static Bogus.DataSets.Name;
using promodel.servicios.perfil;
using DocumentFormat.OpenXml.Office2010.Excel;

namespace promodel.servicios.proyectos;

public class CastingService : ICastingService
{

    private readonly CastingCouchDbContext db;
    private readonly IDistributedCache cache;
    private readonly IServicioIdentidad identidad;
    private readonly HttpClient httpClient;
    private readonly IConfiguration configuration;
    private readonly IServicioPersonas servicioPersonas;
    private readonly IAlmacenamiento almacenamiento;
    private readonly IGoogleDriveConfigProvider provider;
    private readonly IMedia media;
    private readonly ICacheAlmacenamiento cacheAlmacenamiento;
    private readonly IServicioCatalogos servicioCatalogos;
    private readonly ICacheAlmacenamiento cacheAlmacenamiento;
    public CastingService(CastingCouchDbContext db, IDistributedCache cache,
        IServicioIdentidad servicioIdentidad, HttpClient httpClient, IConfiguration configuration, IServicioPersonas servicioPersonas, IAlmacenamiento almacenamiento, IGoogleDriveConfigProvider provider, IMedia media, ICacheAlmacenamiento cacheAlmacenamiento,IServicioCatalogos servicioCatalogos)
    {
        this.db = db;
        this.cache = cache;
        this.identidad = servicioIdentidad;
        this.httpClient = httpClient;
        this.configuration = configuration;
        this.servicioPersonas = servicioPersonas;
        this.almacenamiento = almacenamiento;
        this.provider = provider;
        this.media = media;
        this.servicioCatalogos = servicioCatalogos;
        this.cacheAlmacenamiento = cacheAlmacenamiento;
    }




    public async Task<Respuesta> ActualizaEventosCasting(string CLienteId, string UsuarioId, string CastingId, List<EventoCasting> eventos)
    {
        var r = new Respuesta();
        // trae el casting completo 
        var casting = await ObtieneCasting(CLienteId, CastingId, UsuarioId);

        if (casting == null)
        {
            r.HttpCode = HttpCode.NotFound;
            r.Error = "Casting no encontrado";
            return r;
        }

        // Solo actualiza evento lo dem[as queda intacto
        casting.Eventos = eventos;

        await ActualizaCasting(CLienteId, UsuarioId, CastingId, casting);

        r.Ok = true;
        return r;
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


        List<CategoriaCasting> fianles = new List<CategoriaCasting>();
        // Primero las nuevas 
        foreach(var cnueva in categorias)
        {
            CategoriaCasting exisntent = casting.Categorias.FirstOrDefault(x => x.Id == cnueva.Id);
            if (exisntent != null)
            {
                // Como se evaua por refrecnia el objeto en el casting ya esta actualziado
                exisntent.Descripcion = cnueva.Descripcion;
                exisntent.Nombre = cnueva.Nombre;

            } else
            {
                fianles.Add (cnueva);
            }

        }

        casting.Categorias.AddRange(fianles);

        fianles.Clear();
        foreach(var c in casting.Categorias) {
            var encontrada = categorias.FirstOrDefault(x => x.Id == c.Id);
            if(encontrada == null) {
                fianles.Add(c);
            }
        }

        fianles.ForEach(c =>
        {
            casting.Categorias.Remove(c);
        });
        
               

       
        await ActualizaCasting(CLienteId, UsuarioId, CastingId, casting);

        r.Ok = true;
        return r;
    }

    public async Task<RespuestaPayload<List<ContactoUsuario>>> ActualizaContactosCasting(string ClienteId, string CastingId, string UsuarioId, List<ContactoUsuario> Contactos)
    {
        var r = new RespuestaPayload<List<ContactoUsuario>>();
        var casting = await ObtieneCasting(ClienteId, CastingId, UsuarioId);

        if (casting == null)
        {
            r.HttpCode = HttpCode.NotFound;
            r.Error = "Casting no encontrado";
            return r;
        }


        List<ContactoCasting> contactosCasting = new List<ContactoCasting>();
        foreach (var contacto in Contactos)
        {
            var user = await identidad.UsuarioPorEmail(contacto.Email);

            if (user == null)
            {
                contactosCasting.Add(contacto.aContactoCasting(null));
            }

            else
            {
                contactosCasting.Add(contacto.aContactoCasting(user.UltimoAcceso));
            }
        }


        casting.Contactos = contactosCasting;
        await ActualizaCasting(ClienteId, UsuarioId, CastingId, casting);
        r.Ok = true;
        r.Payload = casting.Contactos;
        return r;
    }


    public async Task<Casting?> ObtieneCasting(string CLienteId, string CastingId, string UsuarioId)
    {
        return await db.Castings.FindAsync(CastingId);
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
                castings = await CastingsStaffRevisor(usuarioId, incluirInactivos,rol);
                break;
            case TipoRolCliente.RevisorExterno:
                castings = await CastingsStaffRevisor(usuarioId, incluirInactivos,rol);
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
    protected async Task<List<Casting>> CastingsStaffRevisor(string usuarioId, Boolean incluirInactivos, TipoRolCliente rol)
    {
        if (incluirInactivos)
        {
            return await db.Castings.Where(c => c.Contactos.Any(x => x.UsuarioId == usuarioId && x.Rol==rol)).ToListAsync();
        }
        else
        {
            return await db.Castings.Where(c => c.Contactos.Any(x => x.UsuarioId == usuarioId && x.Rol == rol) && c.Activo == true).ToListAsync();
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
    public async Task<RespuestaPayload<Casting>> FullCastingByFolderId(string ClienteId, string FolderId)
    {
        var r = new RespuestaPayload<Casting>();
        var casting = await db.Castings.FirstOrDefaultAsync(_ => _.ClienteId == ClienteId && _.FolderId == FolderId);
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
        var cfg = await provider.GetConfig(ClienteId);
        var f= await almacenamiento.CreateFolder(ClienteId, casting.Nombre,cfg.CastingDirectory);
        var r = new RespuestaPayload<Casting>();
        casting.Id = Guid.NewGuid().ToString();
        casting.FechaCreacionTicks = DateTime.UtcNow.Ticks;
        casting.ClienteId = ClienteId;
        casting.UsuarioId = UsuarioId;
        casting.Contactos = new List<ContactoCasting>();
        if(f!=null)
        {
          casting.FolderId = f.Id;
        }       
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
            if (!tmpCasting.Nombre.Equals(casting.Nombre))
            {
                 await almacenamiento.RemameFolder(ClienteId, tmpCasting.FolderId, casting.Nombre);
            }
            tmpCasting.Nombre = casting.Nombre;
            tmpCasting.NombreCliente = casting.NombreCliente;
            tmpCasting.FechaApertura = casting.FechaApertura;
            tmpCasting.FechaCierre = casting.FechaCierre;
            tmpCasting.AceptaAutoInscripcion = casting.AceptaAutoInscripcion;
            tmpCasting.Contactos = casting.Contactos;
            tmpCasting.Categorias=casting.Categorias;
            tmpCasting.Eventos = casting.Eventos;
            tmpCasting.AperturaAutomatica = casting.AperturaAutomatica;
            tmpCasting.CierreAutomatico = casting.CierreAutomatico;
            tmpCasting.AperturaAutomatica = casting.AperturaAutomatica;
            tmpCasting.PernisosEcternos = casting.PernisosEcternos;
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
            foreach (var categoria in casting.Categorias)
            {
                foreach (var modelo in categoria.Modelos)
                {
                    await servicioPersonas.RemoverCasting(ClienteId, modelo.PersonaId, CastingId);
                    
                }


            }
            await db.Castings.RemoveAsync(casting);
            await almacenamiento.DeleteFile(ClienteId, casting.FolderId);
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

    public async Task<RespuestaPayload<CastingListElement>> CastingsActuales(string CLienteId)
    {
        var r = new RespuestaPayload<CastingListElement>();
        var castingsResult = new List<CastingListElement>();
        var castings = await db.Castings
            .Where(_ => _.Activo == true && _.AceptaAutoInscripcion == true && _.Status == modelo.EstadoCasting.Abierto &&
                   (_.FechaApertura <= DateTime.UtcNow) && (_.FechaCierre > DateTime.UtcNow))
            .ToListAsync();

        if (castings != null)
        {
            castings.ForEach(casting =>
            {
                var temp = casting.aCastingListElement();
                temp.Logo = "data:image/jpeg;base64," + Convert.ToBase64String(ObtieneLogo(CLienteId,casting.Id).Result);
                castingsResult.Add(temp);
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

    public async Task<Respuesta> AdicionarModeloCategoria(string ClienteId, string CastingId,string CategoríaId, string PersonaId, OrigenInscripcion origen)
    {
        var r = new Respuesta();
        var casting = await db.Castings.FirstOrDefaultAsync(x => x.ClienteId == ClienteId && x.Id == CastingId);
        if (casting != null)
        {
            var categoria = casting.Categorias.FirstOrDefault(c => c.Id == CategoríaId);
            if (categoria != null && !categoria.Modelos.Any(x => x.PersonaId == PersonaId))
            {
               var res =  await servicioPersonas.AdicionarCasting(PersonaId, ClienteId, CastingId,casting.FolderId);
                if (res.Ok)
                {
                    CastingPersona p = (CastingPersona)res.Payload;
                    categoria.Modelos.Add(new ModeloCasting() { 
                       PersonaId = PersonaId,
                       Origen = origen,
                       FolderId=p.FolderId,
                       Consecutivo= SiguienteId(categoria),
                       FechaAdicion= DateTime.UtcNow
                    });
                    await db.Castings.AddOrUpdateAsync(casting);
                }
                r.Ok = true;
                return r;
            }
          
        }
        r.HttpCode = HttpCode.BadRequest;
        r.Error = "No se pudo agregar modelo";
        return r;
    }

    public async Task<RespuestaPayload<ModeloCastingReview>> AdicionarModeloCategoriaConsecutivo(string ClienteId, string CastingId,string CategoríaId, int Consecutivo, OrigenInscripcion origen)
    {
        var r = new RespuestaPayload<ModeloCastingReview>();
        var casting = await db.Castings.FirstOrDefaultAsync(x => x.ClienteId == ClienteId && x.Id == CastingId);
        var persona = await servicioPersonas.PorConsecutivo(ClienteId,Consecutivo);
        if (casting != null && persona!=null)
        {
            var categoria = casting.Categorias.FirstOrDefault(c => c.Id == CategoríaId);
            if (categoria != null && !categoria.Modelos.Any(x => x.PersonaId == persona.Id))
            {
                var res = await servicioPersonas.AdicionarCasting(persona.Id, ClienteId, CastingId, casting.FolderId);
                if (res.Ok)
                {
                    CastingPersona p = (CastingPersona)res.Payload;
                    var modelo = new ModeloCasting() { PersonaId = persona.Id, Origen = origen, FolderId = p.FolderId, Consecutivo = SiguienteId(categoria), FechaAdicion = DateTime.UtcNow };
                    categoria.Modelos.Add(modelo);
                    await db.Castings.AddOrUpdateAsync(casting);
                    r.Ok = true;
                    r.Payload = new ModeloCastingReview { Consecutivo = modelo.Consecutivo, PersonaId = modelo.PersonaId,FechaAdicion=modelo.FechaAdicion};
                    return r;
                }
                else
                {
                    r.HttpCode = HttpCode.BadRequest;
                    r.Error = "No se pudo agregar modelo";
                }
               
            }
            else
            {
                r.HttpCode = HttpCode.Conflict;
                r.Error = "Modelo ya existente";
            }

        }
        else
        {
            r.HttpCode = HttpCode.NotFound;
            r.Error = "No existe modelo";
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
                    await servicioPersonas.RemoverCasting(ClienteId,PersonaId, CastingId);
                    r.Ok = true;
                    return r;
                }
            }
        }
        r.HttpCode = HttpCode.BadRequest;
        r.Error = "No se pudo remover modelo";
        return r;
    }


    public async Task<RespuestaPayload<ModeloCasting>> GetVideoCastingModelo(string ClienteId, string CastingId, string UsuarioId, string CategoríaId, string PersonaId)
    {
        var respuesta =new RespuestaPayload<ModeloCasting>();
        var casting = await db.Castings.FirstOrDefaultAsync(_ => _.ClienteId == ClienteId && _.Id == CastingId);
        if (casting != null)
        {
            var categoria = casting.Categorias.FirstOrDefault(_ => _.Id == CategoríaId);
            if (categoria != null)
            {
                var modeloCasting = categoria.Modelos.FirstOrDefault(_ => _.PersonaId == PersonaId);
                if(modeloCasting!=null)
                {
                    respuesta.Ok = true;
                    respuesta.Payload = modeloCasting;
                }
                //var video = await almacenamiento.DownloadFile(ClienteId, modeloCasting.VideoPortadaId);
                //if (video != null)
                //{
                //    return video;
                //}
               
            }

        }
        return respuesta;
    }

    public async Task<string> GetFotoCastingModelo(string ClienteId, string CastingId, string UsuarioId, string CategoríaId, string PersonaId)
    {
        var casting = await db.Castings.FirstOrDefaultAsync(_ => _.ClienteId == ClienteId && _.Id == CastingId);
        if (casting != null)
        {
            var categoria = casting.Categorias.FirstOrDefault(_ => _.Id== CategoríaId);
            if (categoria != null)
            {
                var modeloCasting = categoria.Modelos.FirstOrDefault(_ => _.PersonaId == PersonaId);

                var foto = await cacheAlmacenamiento.FotoById(ClienteId,PersonaId,modeloCasting.ImagenPortadaId,"card");
                if (foto != null)
                {
                    return foto;
                }
            }

        }
        return null;
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


    public async Task<RespuestaPayload<ComentarioCategoriaModeloCasting>> AdicionarComentarioModeloCategoria(string ClienteId, string CastingId, string UsuarioId, string CategoriaId, string PersonaId, string Comentario)
    {
        var r = new RespuestaPayload<ComentarioCategoriaModeloCasting>();
        var casting = await ObtieneCasting(ClienteId, CastingId, UsuarioId);
        if (casting != null)
        {
            var categoria = casting.Categorias.FirstOrDefault(x => x.Id == CategoriaId);
            if (categoria != null)
            {
                var modelo = categoria.Modelos.FirstOrDefault(m => m.PersonaId.Equals(PersonaId));
                if (modelo != null)
                {
                    var comentario = new ComentarioCasting() { Comentario = Comentario, UsuarioId = UsuarioId };
                    modelo.Comentarios.Add(comentario);
                    await db.Castings.AddOrUpdateAsync(casting);
                    r.Payload = comentario.aComentarioCategoriaModeloCasting(CategoriaId,PersonaId);
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
            var fichero = System.IO.File.Create(patch);
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
            Directory.Delete(pahchFolder, true);
            return r;
        }
        r.HttpCode = HttpCode.BadRequest;
        r.Error = "No se pudo guardar logo";
        Directory.Delete(pahchFolder, true);
        return r;

    }


    public async Task<byte[]> ObtieneLogo(string ClienteId, string CastingId)
    {
        string URL = configuration.GetValue<string>("promodeldrivers:couchdb:endpoint");
        string User = configuration.GetValue<string>("promodeldrivers:couchdb:username");
        string Pass = configuration.GetValue<string>("promodeldrivers:couchdb:password");
        string url = URL + "/proyectos/" + CastingId + "/logo.jpg";

        var base64String = Convert.ToBase64String(
           System.Text.Encoding.ASCII.GetBytes($"{User}:{Pass}"));
        httpClient.DefaultRequestHeaders.Add("Accept", "application/octet-stream");
        httpClient.DefaultRequestHeaders.Authorization =
        new AuthenticationHeaderValue("Basic", base64String);
        var response = await httpClient.GetAsync(url);
        var result = await response.Content.ReadAsByteArrayAsync();
        return result;

    }

    public async Task<SelectorCastingCategoria> SelectorCastingCategoria(string ClienteId,string CastingId, string UsuarioId)
    {
        var casting = await ObtieneCasting(ClienteId,CastingId,UsuarioId);
        
        if (casting != null)
        {
           
            var castingSelector = casting.aSelectorCasting();


            foreach (var a in casting.Contactos)
            {
                var user = await identidad.UsuarioPorId(a.UsuarioId);
                if (user != null)
                {
                    MapaUsuarioNombre m = new MapaUsuarioNombre()
                    {
                        Id = user.Id,
                        Nombre = user.NombreUsuario,
                        Email = user.Email
                    };
                    castingSelector.Participantes.Add(m);
                }
            }
                return castingSelector;
            

        }
        return null;
    }

    public async Task<SelectorCastingCategoria> SelectorCastingCategoriaRevisor(string ClienteId, string CastingId, string UsuarioId,TipoRolCliente rol)
    {
        var casting = await ObtieneCasting(ClienteId, CastingId, UsuarioId);

        if (casting != null)
        {
            if (casting.Contactos.Any(_ => _.UsuarioId == UsuarioId  && _.Rol==rol)|| rol==TipoRolCliente.Administrador)
            {
                var castingSelector = casting.aSelectorCasting();


                foreach (var a in casting.Contactos)
                {
                    var user = await identidad.UsuarioPorId(a.UsuarioId);
                    if (user != null)
                    {
                        MapaUsuarioNombre m = new MapaUsuarioNombre()
                        {
                            Id = user.Id,
                            Nombre = user.NombreUsuario,
                            Email = user.Email
                        };
                        castingSelector.Participantes.Add(m);
                    }
                }
                return castingSelector;
            }

        }
        return null;
    }

    public async Task<RespuestaPayload<VotoModeloCategoria>> VotoModelo(string userId, string modeloId, string clienteId, string castingId, string categoriaId, string nivel)
    {
        var r = new RespuestaPayload<VotoModeloCategoria>();
        VotoModeloCategoria votoRetorno = new VotoModeloCategoria();
        var casting = await ObtieneCasting(clienteId, castingId, userId);
        var categoria = casting.Categorias.FirstOrDefault(c => c.Id == categoriaId);
        if (casting == null && categoria == null)
        {
            r.Error = "No se encontró el casting y la categoría";
            r.HttpCode = HttpCode.BadRequest;
            return r;
        }
        var modelo = categoria.Modelos.FirstOrDefault(m => m.PersonaId == modeloId);
        if (modelo == null && categoria == null)
        {
            r.Error = "No se encontró el modelo y la categoría";
            r.HttpCode = HttpCode.BadRequest;
            return r;
        }
        var revisor = casting.Contactos.FirstOrDefault(r => r.UsuarioId == userId && r.Rol == TipoRolCliente.RevisorExterno);
        if (revisor == null)
        {
            r.Error = "El usuario no forma parte como revisor";
            r.HttpCode = HttpCode.BadRequest;
            return r;
        }

        var votoRevisor = modelo.Votos.FirstOrDefault(v => v.UsuarioId == userId);
        if (votoRevisor != null)
        {
            votoRevisor.NivelLike = int.Parse(nivel);
            votoRetorno = votoRevisor;
        }
        else
        {
            VotoModeloCategoria voto = new VotoModeloCategoria()
            {
                UsuarioId = userId,
                NivelLike = int.Parse(nivel),
            };
            modelo.Votos.Add(voto);
            votoRetorno = voto;
        }
        await db.Castings.AddOrUpdateAsync(casting);
        r.Ok = true;
        r.Payload= votoRetorno;
        return r;
    }

    public async Task<Respuesta> InscripcionCasting(string ClienteId, string PersonaId, string CastingId, string CategoriaId, bool Abandonar, string UsuarioId)
    {
        var r = new Respuesta();

        var casting = await ObtieneCasting(ClienteId, CastingId, UsuarioId);
        if (casting.ClienteId == ClienteId && casting.AceptaAutoInscripcion == true)
        {
            var categoriaEnCasting = casting.Categorias.FirstOrDefault(c => c.Id == CategoriaId);
            if (categoriaEnCasting != null)
            {
                var persona = await servicioPersonas.PorId(PersonaId);
                var personaAsociadaCliente = ((Persona)persona.Payload).Clientes.FirstOrDefault(clienteAsociado => clienteAsociado == ClienteId);
                if (personaAsociadaCliente != null)
                {
                    var personaEnCategoria = categoriaEnCasting.Modelos.FirstOrDefault(p => p.PersonaId == PersonaId);
                    if (Abandonar == false && personaEnCategoria == null)
                    {
                        ModeloCasting modeloCasting = new ModeloCasting()
                        {
                            PersonaId = PersonaId,
                            Origen = OrigenInscripcion.publico,
                        };
                        categoriaEnCasting.Modelos.Add(modeloCasting);
                    }
                    else
                    {
                        if(personaEnCategoria != null)
                        {
                            categoriaEnCasting.Modelos.Remove(personaEnCategoria);
                        }
                    }
                }
                else
                {
                    r.Ok = false;
                    r.HttpCode = HttpCode.BadRequest;
                    return r;
                }
            }
            else
            {
                r.Ok = false;
                r.HttpCode = HttpCode.BadRequest;
                return r;
            }
        }
        else
        {
            r.Ok = false;
            r.HttpCode = HttpCode.BadRequest;
        }
        await db.Castings.AddOrUpdateAsync(casting);
        r.Ok = true;
        return r;
    }

    public async Task<RespuestaPayload<List<string>>> CategoriasModeloCasting(string ClienteId, string CastingId, string PersonaId, string UsuarioId)
    {
        var r = new RespuestaPayload<List<string>>();

        var casting = await ObtieneCasting(ClienteId, CastingId, UsuarioId);
        if (casting.ClienteId == ClienteId && casting.AceptaAutoInscripcion == true)
        {
            var categoriaEnCasting = casting.Categorias.Any();
            if (categoriaEnCasting == true)
            {
                var persona = await servicioPersonas.PorId(PersonaId);
                var personaAsociadaCliente = ((Persona)persona.Payload).Clientes.FirstOrDefault(clienteAsociado => clienteAsociado == ClienteId);
                if (personaAsociadaCliente != null)
                {
                    List<string> listaCategoriasPersona = new List<string>();
                    casting.Categorias.ForEach(c =>
                    {
                        var lista = c.Modelos.FirstOrDefault(m => m.PersonaId == PersonaId);
                        if(lista != null)
                        {
                            listaCategoriasPersona.Add(c.Id);

                        }
                    });
                    r.Payload = listaCategoriasPersona;
                }
                else
                {
                    r.Ok = false;
                    r.HttpCode = HttpCode.BadRequest;
                    return r;
                }
            }
            else
            {
                r.Ok = false;
                r.HttpCode = HttpCode.BadRequest;
                return r;
            }
        }
        else
        {
            r.Ok = false;
            r.HttpCode = HttpCode.BadRequest;
        }
        r.Ok = true;
        return r;
    }


    public async Task<Respuesta> EstablecerEstadoCasting(string usuarioId, string castingId, string clienteId, EstadoCasting estado,TipoRolCliente Rol)
    {
        //  Creacion de un o tipo Respuesta
        var r = new Respuesta();
        var casting = await ObtieneCasting(clienteId, castingId, usuarioId);
        //verifica que el parametro castingId de este metodo exixta en alguno de los castings
        if (casting != null)
        {
            //verifica que el usurioId de es  dentro de los contactos busca todo los contactos que esten agregados los busca por su id de usuario y verifica que esos 
            //usuarios tengan un rango de estaff o  si el usuario es administrador
          
            var puedeEditar = casting.Contactos.Any(x => x.UsuarioId == usuarioId && (x.Rol == TipoRolCliente.Staff)) || TipoRolCliente.Administrador==Rol;
            if (puedeEditar)
            {
                casting.Status = estado;
                await db.Castings.AddOrUpdateAsync(casting);
                r.Ok = true;
                return r;
            }
            else
            {
                r.HttpCode = HttpCode.Forbidden;
            }
        }
        else
        {
            r.HttpCode = HttpCode.Forbidden;
        }
        return r;
    }

    public async Task<string?> NombreActivo(string ClienteId, string UsuarioId, string castingId)
    {
        var rCasting = await FullCasting(ClienteId, castingId, UsuarioId);
        if (rCasting.Ok)
        {
            var casting = (Casting)rCasting.Payload;
            if (casting.Activo)
            {
                return casting.Nombre;
            }
        }

        return null;
    }

    public async Task<Respuesta> ActualizarModeloCasting(string ClienteId, string castingId,string categoriaId, ModeloCasting Modelo)
    {
        var respuesta = new Respuesta();
        var casting = db.Castings.FirstOrDefault(_ => _.Id == castingId && _.ClienteId==ClienteId);
        if(casting!=null)
        {
            var indexCategoria = casting.Categorias.FindIndex(_ => _.Id == categoriaId);
            if(indexCategoria>=0)
            {

                var indexModelo = casting.Categorias[indexCategoria].Modelos.FindIndex(_ => _.PersonaId == Modelo.PersonaId);

                if(indexModelo>=0)
                {
                    casting.Categorias[indexCategoria].Modelos[indexModelo] = Modelo;
                    await db.Castings.AddOrUpdateAsync(casting);
                    respuesta.Ok = true;
                }
            }
        }
        return respuesta;
    }


    public async Task<Respuesta> ActualizarFotoCastinPrincipal(string ClienteId, string castingId, string personaId, string? archivoId)
    {
        var respuesta = new Respuesta();
        var casting = db.Castings.FirstOrDefault(_ => _.Id == castingId && _.ClienteId == ClienteId);
        if (casting != null)
        {
            var Categoria = casting.Categorias.FirstOrDefault(_=>_.Modelos.Any(x=>x.PersonaId==personaId));
            if (Categoria!=null)
            {
                var modeloCasting = Categoria.Modelos.FirstOrDefault(_=>_.PersonaId==personaId);

                if(modeloCasting!=null)
                {
                    modeloCasting.ImagenPortadaId = archivoId;

                    return await this.ActualizarModeloCasting(ClienteId,castingId,Categoria.Id,modeloCasting);
                }
               
            }
        }
        return respuesta;

    }
    public async Task<Respuesta> ActualizarVideoCastinPrincipal(string ClienteId, string castingId, string personaId, string? archivoId)
    {
        var respuesta = new Respuesta();
        var casting = db.Castings.FirstOrDefault(_ => _.Id == castingId && _.ClienteId == ClienteId);
        if (casting != null)
        {
            var Categoria = casting.Categorias.FirstOrDefault(_ => _.Modelos.Any(x => x.PersonaId == personaId));
            if (Categoria != null)
            {
                var modeloCasting = Categoria.Modelos.FirstOrDefault(_ => _.PersonaId == personaId);

                if (modeloCasting != null)
                {
                    modeloCasting.VideoPortadaId = archivoId;

                    return await this.ActualizarModeloCasting(ClienteId, castingId, Categoria.Id, modeloCasting);
                }

            }
        }
        return respuesta;

    }


    #endregion
    #region Acceso
    #endregion

    private int SiguienteId(CategoriaCasting categoria)
    {
        try
        {
            var c = categoria.Modelos.Max(p => p.Consecutivo);
            if (c.HasValue)
            {
                return c.Value + 1;
            }
        }
        catch (Exception)
        {


        }

        return 1;
    }

    public async Task<RespuestaPayload<List<Persona>>> GetModelosCategoria(string ClienteId, string castingId, string categoriaId)
    {
        var modelos = new List<Persona>();
            var r = new RespuestaPayload<List<Persona>>();
            var casting = await db.Castings.FirstOrDefaultAsync(x => x.ClienteId == ClienteId && x.Id == castingId);
            if (casting != null)
            {
                var categoria = casting.Categorias.FirstOrDefault(c=>c.Id == categoriaId);
                if (categoria != null && categoria.Modelos.Any())
                {
                    foreach (var modelo in categoria.Modelos)
                    {
                        var res = await servicioPersonas.PorId(modelo.PersonaId);
                            if (res.Ok)
                            {
                            modelos.Add((Persona)res.Payload);
                            }    
                    }
                    r.Ok = true;
                    r.Payload = modelos;
                    return r;
                }

            }
            r.HttpCode = HttpCode.BadRequest;
            r.Error = "No hay modelos";
            return r;
     }
    
    #region Excel
    public async Task<RespuestaPayload<Casting>> CrearExcelOpenXml(string filepath, Casting casting)
    {
        var r = new RespuestaPayload<Casting>();
        using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Create(filepath, SpreadsheetDocumentType.Workbook))
        {
            WorkbookPart workbookPart = spreadsheetDocument.AddWorkbookPart();
            workbookPart.Workbook = new Workbook();

            Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());

            foreach (var categoria in casting.Categorias)
            {
                WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                worksheetPart.Worksheet = new Worksheet(new SheetData());

                Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = (uint)sheets.Count() + 1, Name = categoria.Nombre };
                sheets.Append(sheet);

                SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

                Row headerRow = new Row();
                headerRow.Append(
                    new Cell() { DataType = CellValues.String, CellValue = new CellValue("FotografiaPricipal") },
                    new Cell() { DataType = CellValues.String, CellValue = new CellValue("NombreArtistico") },
                    new Cell() { DataType = CellValues.String, CellValue = new CellValue("NombrePersona") },
                    new Cell() { DataType = CellValues.String, CellValue = new CellValue("Genero") },
                    new Cell() { DataType = CellValues.String, CellValue = new CellValue("Edad") },
                    new Cell() { DataType = CellValues.String, CellValue = new CellValue("Habilidades") }
                );
                sheetData.AppendChild(headerRow);

                foreach (var modelo in categoria.Modelos)
                {
                    var respuesta = await servicioPersonas.PorId(modelo.PersonaId);
                    if (respuesta.Payload is Persona persona)
                    {
                        CatalogoBase habilidades = await servicioCatalogos.GetCatalogoCliente("actividades",casting.ClienteId);
                        List<string> habilidadesModelo = new List<string>();
                        foreach (var habilidad in persona.ActividadesIds)
                        {
                            habilidades.Elementos.ForEach(e =>
                            {
                                if (e.Clave == habilidad)
                                {
                                    habilidadesModelo.Add(e.Texto);
                                }
                            });
                        }
                        string rutaIMG="";
                        FileStream fileStream;
                        if(!string.IsNullOrEmpty(persona.ElementoMedioPrincipalId))
                        {
                            rutaIMG = await cacheAlmacenamiento.FotoById(casting.ClienteId, persona.UsuarioId, persona.ElementoMedioPrincipalId, "thumb");
                            FileInfo fi = new FileInfo(rutaIMG);
                            fileStream = System.IO.File.OpenRead(rutaIMG);
                        }


                        Row row = new Row();
                        row.Append(

                            new Cell() { DataType = CellValues.String, CellValue = new CellValue(fileStream) },
                            new Cell() { DataType = CellValues.String, CellValue = new CellValue(persona.NombreArtistico) },
                            new Cell() { DataType = CellValues.String, CellValue = new CellValue(persona.Nombre) },
                            new Cell() { DataType = CellValues.String, CellValue = new CellValue(persona.GeneroId) },
                            new Cell() { DataType = CellValues.String, CellValue = new CellValue(persona.Edad) },
                            new Cell() { DataType = CellValues.String, CellValue = new CellValue(string.Join(", ", habilidadesModelo)) }
                        );
                        sheetData.AppendChild(row);
                    }
                }
            }

            workbookPart.Workbook.Save();
        }
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

    #endregion
}
