using almacenamiento;
using CouchDB.Driver.Extensions;
using ImageMagick;
using promodel.modelo.media;
using promodel.modelo.proyectos;
using promodel.modelo.webhooks;
using promodel.servicios.media;
using promodel.servicios.proyectos;
using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using promodel.modelo.perfil;
using Amazon.Runtime.Internal.Util;
using promodel.modelo.clientes;
using System.Reflection;

namespace promodel.servicios.webhooks;

public class ServicioGoogleDrivePushNotifications : IServicioGoogleDrivePushNotifications
{
    private readonly GoogleDriveDbContext db;
    private readonly IMedia media;
    private readonly IAlmacenamiento google;
    private readonly IServicioPersonas servicioPersonas;
    private readonly ICastingService castingService;
    private readonly ICacheAlmacenamiento cacheAlmacenamiento;
    private readonly IConfiguration configuration;
    private readonly CacheAlmacenamientoLocalConfig config;

    public ServicioGoogleDrivePushNotifications(GoogleDriveDbContext db, IMedia media, IAlmacenamiento google, IServicioPersonas servicioPersonas, ICastingService castingService, ICacheAlmacenamiento cacheAlmacenamiento, IConfiguration configuration, IOptions<CacheAlmacenamientoLocalConfig> options)
    {
        this.db = db;
        this.media = media;
        this.google = google;
        this.servicioPersonas = servicioPersonas;
        this.castingService = castingService;
        this.cacheAlmacenamiento = cacheAlmacenamiento;
        this.configuration = configuration;
        this.config = options.Value;
    }

    /// <summary>
    /// Inserta un evento del webhoo para proceso asincrono
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public async Task<Respuesta> InsertaEvento(string ClienteId,GoogleDrivePushNotification data)
    {
        Respuesta r = new();
      //  string fileName = data.ResourceUri!.ToString().Split("files/")[1].Split("?")[0];
      //  var medio = google.ObtienePadre(ClienteId, fileName);
        try
        {
            var notificaionEncontrada = await db.Notificaciones.AnyAsync(_ => _.ChannelId == data.ChannelId && _.MessageNumber == data.MessageNumber);
            if (!notificaionEncontrada)
            {
                data.Id = Guid.NewGuid().ToString();
                await db.Notificaciones.AddOrUpdateAsync(data);
            }
            r.Ok = true;
        }
        catch (Exception ex)
        {
            r.Ok = false;
            r.Error = ex.Message;
        }
        return r;

    }

    public async Task<RespuestaPayload<GoogleDrivePushNotification?>> ObtieneEventoPendiente()
    {
        RespuestaPayload<GoogleDrivePushNotification> res = new();
        var encontrada = await db.Notificaciones.FirstOrDefaultAsync(_ => _.Procesado == false);
        res.Ok = true;
        res.Payload = encontrada;
        return res;
    }


    public async Task<Respuesta> FinalizaProcesamientoEvento(string Id, bool Ok, string? error)
    {
        Respuesta res = new();
        var notificaionEncontrada = await db.Notificaciones.FirstOrDefaultAsync(_ => _.Id == Id);
        if (notificaionEncontrada != null)
        {
            if (Ok)
            {
                await db.Notificaciones.RemoveAsync(notificaionEncontrada);
            }
            else
            {
                notificaionEncontrada.Procesado = true;
                notificaionEncontrada.Error = error;
                await db.Notificaciones.AddOrUpdateAsync(notificaionEncontrada);
            }
            res.Ok = true;
        }
        else
        {
            res.Error = "Notificacion no Encontrada";
            res.HttpCode = HttpCode.NotFound;
        }
        return res;
    }

    public async Task<Respuesta> ProcesaEventoEliminar(string ClienteId, string FolderModeloId)
    {
        var respuesta = new Respuesta();

        List<string> FolderCastingId = await google.ObtienePadre(ClienteId, FolderModeloId);
        Casting casting = null;
        ModeloCasting Modelo;
        if (FolderCastingId.Any())
        {
            var respuestCasting = await castingService.FullCastingByFolderId(ClienteId, FolderCastingId.First());
            if (respuestCasting.Ok)
            {
                casting = (Casting)respuestCasting.Payload;
                var categoria = casting.Categorias.FirstOrDefault(x => x.Modelos.Any(y => y.FolderId == FolderModeloId));
                if (categoria != null)
                {
                    Modelo = categoria.Modelos.FirstOrDefault(_ => _.FolderId == FolderModeloId);
                    if (Modelo != null)
                    {
                        var ElementoRemovido = await RemoverArchivo(ClienteId, Modelo.PersonaId, casting.Id, FolderModeloId);
                        if (ElementoRemovido!=null)
                        {

                            if (ElementoRemovido.Imagen)
                            {
                                Modelo.ImagenPortadaId = null;
                            }
                            if (ElementoRemovido.Video)
                            {
                                Modelo.VideoPortadaId = null;
                            }
                            respuesta = await castingService.ActualizarModeloCasting(ClienteId,casting.Id, categoria.Id, Modelo);

                        }
                        else
                        {
                            respuesta.HttpCode = HttpCode.NotFound;
                            respuesta.Error = "No existe Archivo para procesar";
                        }
                    }
                    else
                    {
                        respuesta.HttpCode = HttpCode.NotFound;
                        respuesta.Error = "No existe Modelo en la categoria";
                    }
                }
                else
                {
                    respuesta.HttpCode = HttpCode.NotFound;
                    respuesta.Error = "El folder No pertenece a ninguna Categoria";
                }
            }
            else
            {
                respuesta.HttpCode = HttpCode.NotFound;
                respuesta.Error = "No existe el Casting";
            }
        }
        else
        {
            respuesta.HttpCode = HttpCode.NotFound;
            respuesta.Error = "El folder NO pertenece a un Casting";
        }
        return respuesta;

   
    }


        public async Task<Respuesta> ProcesaEventoCrear(string ClienteId, string FolderModeloId)
    {
        var respuesta = new Respuesta();

        List<string> FolderCastingId = await google.ObtienePadre(ClienteId, FolderModeloId);
        Casting casting = null;
        ModeloCasting Modelo;
        if (FolderCastingId.Any())
        {
            var respuestCasting = await castingService.FullCastingByFolderId(ClienteId, FolderCastingId.First());
            if (respuestCasting.Ok)
            {
                casting = (Casting)respuestCasting.Payload;
                var categoria = casting.Categorias.FirstOrDefault(x => x.Modelos.Any(y => y.FolderId == FolderModeloId));
                if (categoria != null)
                {
                    Modelo = categoria.Modelos.FirstOrDefault(_ => _.FolderId == FolderModeloId);
                    if (Modelo != null)
                    {
                        var archivoId = await GetArchivoIdAgregado(ClienteId, Modelo.PersonaId, casting.Id, FolderModeloId);
                        if (!string.IsNullOrEmpty(archivoId))
                        {
                            var res = await ActualizarArchivo(ClienteId, Modelo.PersonaId, casting.Id, archivoId, casting.Nombre);
                            if (res.Ok)
                            {
                                var medio = (ElementoMedia)res.Payload;

                                if (medio.Imagen)
                                {
                                    Modelo.ImagenPortadaId = medio.Id;
                                }
                                if (medio.Video)
                                {
                                    Modelo.VideoPortadaId = medio.Id;
                                }

                                respuesta = await castingService.ActualizarModeloCasting(ClienteId, casting.Id, categoria.Id, Modelo);

                            }
                            else
                            {
                                respuesta.HttpCode = res.HttpCode;
                                respuesta.Error = res.Error;
                            }
                        }
                        else
                        {
                            respuesta.HttpCode = HttpCode.NotFound;
                            respuesta.Error = "No existe Archivo para procesar";
                        }
                    }
                    else
                    {
                        respuesta.HttpCode = HttpCode.NotFound;
                        respuesta.Error = "No existe Modelo en la categoria";
                    }
                }
                else
                {
                    respuesta.HttpCode = HttpCode.NotFound;
                    respuesta.Error = "El folder No pertenece a ninguna Categoria";
                }
            }
            else
            {
                respuesta.HttpCode = HttpCode.NotFound;
                respuesta.Error = "No existe el Casting";
            }
        }
        else
        {
            respuesta.HttpCode = HttpCode.NotFound;
            respuesta.Error = "El folder NO pertenece a un Casting";
        }
        return respuesta;
    }

    public async Task<RespuestaPayload<ElementoMedia>> ActualizarArchivo(string clienteId, string PersonaId, string castingId, string ArchivoId,string nombreCasting)
    {
        var respuesta = new RespuestaPayload<ElementoMedia>();
        var Archivo = await google.DownloadFile(clienteId, ArchivoId);
        var Metadatos = await google.getMetadatos(clienteId, ArchivoId);

        if (Archivo != null && Metadatos != null)
        {
            string webRootPath = config.Ruta;
            string uploadsDir = Path.Combine(webRootPath, PersonaId);

            if (!Directory.Exists(uploadsDir))
                Directory.CreateDirectory(uploadsDir);

            string fileName = Metadatos.Name;
            bool EsFoto = false;
            bool EsVideo = false;
            bool SinSoporte = true;
            bool EsAudio = false;
            bool Landscape = false;
            bool EsPDF = false;

            string FileId = $"{ArchivoId}.{fileName.Split(".")[1]}";
            string FullPath = Path.Combine(uploadsDir, FileId);
            string FileFrameId = $"{ArchivoId}-thumb.jpg";
            string FrameFullPath = Path.Combine(uploadsDir, FileFrameId);
            string Titulo = nombreCasting;

            if ("jpg,jpeg,png,gif".Contains(Metadatos.FileExtension.ToLower(), StringComparison.CurrentCulture))
            {
                EsFoto = true;
                SinSoporte = false;
            }

            if ("mp4,mpeg,mpg,mov,wmv,ogg".Contains(Metadatos.FileExtension.ToLower(), StringComparison.CurrentCulture))
            {
                EsVideo = true;
                SinSoporte = false;
            }

            if ("mp3,ogg".Contains(Metadatos.FileExtension.ToLower(), StringComparison.CurrentCulture))
            {
                EsAudio = true;
                SinSoporte = false;
            }

            if (".pdf".Contains(Metadatos.FileExtension.ToLower(), StringComparison.CurrentCulture))
            {
                EsPDF = true;
                SinSoporte = false;
            }


            using (var stream = new FileStream(FullPath, FileMode.Create))
            {
                Archivo.WriteTo(stream);
            }

            if (EsFoto)
            {
                using (var image = new MagickImage(FullPath))
                {
                    image.AutoOrient();
                    if (image.Width > image.Height)
                    {
                        Landscape = true;
                    }
                }
                await cacheAlmacenamiento.CreaArchivoImagen(FullPath,FileId, uploadsDir, true);
            }

            if (EsVideo)
            {
                FrameFullPath = ObtieneFrame(FullPath, FrameFullPath);
                if (FrameFullPath == null)
                {
                    return respuesta;
                }                
            }
            ElementoMedia el = null;
            if (ArchivoId != null)
            {
                await google.AccesoPublico(clienteId, ArchivoId, true);
                // Añade el registro a la base de datos
                FileInfo FiSaved = new(FullPath);

                el = await AddElementoMedio(clienteId, PersonaId, ArchivoId,
                    TipoMedio.Galería, $".{ Metadatos.FileExtension}", fileName.GetMimeTypeForFileExtension(),
                    FiSaved.Length, EsFoto, EsVideo, EsAudio, SinSoporte, EsPDF, Landscape, ArchivoId, Titulo, castingId);
            }

            try
            {
                File.Delete(FullPath);
            }
            catch (Exception)
            {
            }

            if (el != null)
            {
                respuesta.Ok = true;
                respuesta.Payload = el;
            }
        }
        else
        {
            respuesta.HttpCode = HttpCode.NotFound;
            respuesta.Error = "Archivo no existente";
        }

        return respuesta;
    }


    private async Task<ElementoMedia> AddElementoMedio(
        string clienteId,
        string personaId,
        string Id,
        TipoMedio Tipo,
        string Extension,
        string MimeType,
        long Totales,
        bool EsFoto = true,
        bool EsVideo = false,
        bool EsAudio = false,
        bool SinSoporte = true,
        bool Pdf = false,
        bool Landscape = true,
    string? FrameId = null,
        string? Titulo = null,
        string? castingId = null
        )
    {

        ElementoMedia el = new ElementoMedia()
        {
            Id = Id,
            ClienteIds = new List<string>() { clienteId },
            CreacionTicks = DateTime.UtcNow.Ticks,
            EliminacionTicks = DateTime.UtcNow.AddMonths(3).Ticks,
            Extension = Extension,
            Imagen = EsFoto,
            Video = EsVideo,
            MimeType = MimeType,
            Permanente = false,
            TamanoBytes = Totales,
            Principal = false,
            Landscape = Landscape,
            FrameVideoId = FrameId,
            Pdf = Pdf,
            Tipo = Tipo,
            Audio = EsAudio,
            SinSoporteWeb = SinSoporte,
            Titulo = Titulo,
            CastingId = castingId

        };

        el = await this.media.AddElemento(el, personaId);

        return el;
    }

    private string ObtieneFrame(string Archivo, string ArchivoSalida)
    {
        FileInfo fi = new FileInfo(Archivo);
        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.CreateNoWindow = false;
        startInfo.UseShellExecute = false;
        startInfo.FileName = configuration["ffmpegfullpath"];
        startInfo.Arguments = $"-i {Archivo} -vf \"select=eq(n\\,1)\" -frames:v 1 {ArchivoSalida}";
        startInfo.RedirectStandardOutput = true;


        try
        {
            using (Process process = Process.Start(startInfo))
            {
                while (!process.StandardOutput.EndOfStream)
                {
                    string line = process.StandardOutput.ReadLine();
                    Console.WriteLine(line);
                }

                process.WaitForExit();

                Console.WriteLine($"process.ExitCode {process.ExitCode}");
                if (process.ExitCode == 0)
                {
                    return ArchivoSalida;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error {ex.Message}");
        }
        return null; ;
    }
    public async Task<string> GetArchivoIdAgregado(string clienteId, string PersonaId, string castingId, string folderModeloId) 
    {       
        var medio = await media.GetByUsuarioId(PersonaId);

        if (medio != null)
        {
            var ElementosMedia = medio.Elementos.Where(_ => _.CastingId ==castingId).ToList();

            if (ElementosMedia != null)
            {
                var idsArchivosFolder = await google.getArchivosFolder(clienteId, folderModeloId);


                if (idsArchivosFolder.Any())
                {
                    foreach (var id in idsArchivosFolder)
                    {
                        var archivo = ElementosMedia.FirstOrDefault(_=> _.Id == id);
                        if (archivo == null)
                        {
                            return id;
                        }
                    };
                }            
            }
            else
            {
              return null;
            }
        }
        else
        {
            return null;
        }
        return null;
     
    }

    public async Task<ElementoMedia> RemoverArchivo(string clienteId, string PersonaId, string castingId, string folderModeloId)
    {
        var medio = await media.GetByUsuarioId(PersonaId);

        if (medio != null)
        {
            var ElementosMedia = medio.Elementos.Where(_ => _.CastingId == castingId).ToList();

            if (ElementosMedia.Count >0)
            {
                var idsArchivosFolder = await google.getArchivosFolder(clienteId, folderModeloId);               
                    foreach (var elemento in ElementosMedia)
                    {
                        if (!idsArchivosFolder.Contains(elemento.Id))
                        {
                            await  media.DelElemento(elemento.Id, medio.UsuarioId);
                            return elemento;
                        }
                    };
               
            }
        }
        else
        {
            return null;
        }
        return null;

    }

}
