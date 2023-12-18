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

    public ServicioGoogleDrivePushNotifications(GoogleDriveDbContext db, IMedia media, IAlmacenamiento google, IServicioPersonas servicioPersonas, ICastingService castingService, ICacheAlmacenamiento cacheAlmacenamiento, IConfiguration configuration)
    {
        this.db = db;
        this.media = media;
        this.google = google;
        this.servicioPersonas = servicioPersonas;
        this.castingService = castingService;
        this.cacheAlmacenamiento = cacheAlmacenamiento;
        this.configuration = configuration;
    }

    /// <summary>
    /// Inserta un evento del webhoo para proceso asincrono
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public async Task<Respuesta> InsertaEvento(string ClienteId,GoogleDrivePushNotification data)
    {
        Respuesta r = new();
        string fileName = data.ResourceUri.ToString().Split("files/")[1].Split("?")[0];
        var medio = google.ObtienePadre(ClienteId, fileName);
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

    public async Task<Respuesta> ProcesaEventoEliminar(string ClienteId, string Id)
    {
        var respuesta = new Respuesta();
        Casting casting;
        ModeloCasting Modelo;
        var medio = await media.GetByElementoId(Id);

        if (medio != null)
        {
            var ElementoMedia = medio.Elementos.FirstOrDefault(_ => _.Id == Id);

            if (ElementoMedia != null)
            {
                media.DelElemento(ElementoMedia.Id, medio.UsuarioId);


                var respuestCasting = await castingService.FullCasting(ClienteId, ElementoMedia.CastingId, medio.UsuarioId);
                if (respuestCasting.Ok)
                {
                    casting = (Casting)respuestCasting.Payload;
                    var categoria = casting.Categorias.FirstOrDefault(x => x.Modelos.Any(y => y.PersonaId == medio.UsuarioId));
                    if (categoria != null)
                    {
                        Modelo = categoria.Modelos.FirstOrDefault(_ => _.PersonaId == medio.UsuarioId);
                        if (Modelo != null)
                        {

                            if (ElementoMedia.Imagen)
                            {
                                Modelo.ImagenPortadaId = null;
                            }
                            if (ElementoMedia.Video)
                            {
                                Modelo.VideoPortadaId = null;
                            }
                            respuesta = await castingService.ActualizarModeloCasting(ClienteId, casting.Id, categoria.Id, Modelo);

                        }
                    }
                }


            }
            else
            {
                respuesta.HttpCode = HttpCode.NotFound;
                respuesta.Error = "Elemento Media No encontrado";
            }
        }
        else
        {
            respuesta.HttpCode = HttpCode.NotFound;
            respuesta.Error = "Medio No existente";
        }
        return respuesta;
    }


    public async Task<Respuesta> ProcesaEventoCrear(string ClienteId, string Id)
    {
        var respuesta = new Respuesta();
        List<string> FolderModeloId = await google.ObtienePadre(ClienteId, Id);
        if (FolderModeloId.Any())
        {
            List<string> FolderCastingId = await google.ObtienePadre(ClienteId, FolderModeloId.First());
            Casting casting = null;
            ModeloCasting Modelo;
            if (FolderCastingId.Any())
            {
                var respuestCasting = await castingService.FullCastingByFolderId(ClienteId, FolderCastingId.First());
                if (respuestCasting.Ok)
                {
                    casting = (Casting)respuestCasting.Payload;
                    var categoria = casting.Categorias.FirstOrDefault(x => x.Modelos.Any(y => y.FolderId == FolderModeloId.First()));
                    if (categoria != null)
                    {
                        Modelo = categoria.Modelos.FirstOrDefault(_ => _.FolderId == FolderModeloId[0]);
                        if (Modelo != null)
                        {
                            var res = await ActualizarArchivo(ClienteId, Modelo.PersonaId, casting.Id, Id);
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
                            respuesta.Error = "No existe Modelo";
                        }
                    }
                    else
                    {
                        respuesta.HttpCode = HttpCode.NotFound;
                        respuesta.Error = "No existe Casting";
                    }
                }
                else
                {
                    respuesta.HttpCode = HttpCode.NotFound;
                    respuesta.Error = "la Carpeta no tiene Casting ID";
                }
            }
            else
            {
                respuesta.HttpCode = HttpCode.NotFound;
                respuesta.Error = "Archivo no tiene Carpeta ModeloId";
            }
        }
        else {
            respuesta.HttpCode = HttpCode.NotFound;
            respuesta.Error = "Archivo no existe";
        }

        return respuesta;
    }

    public async Task<RespuestaPayload<ElementoMedia>> ActualizarArchivo(string clienteId, string PersonaId, string castingId, string ArchivoId)
    {
        var respuesta = new RespuestaPayload<ElementoMedia>();
        var Archivo = await google.DownloadFile(clienteId, ArchivoId);
        var Metadatos = await google.getMetadatos(clienteId, ArchivoId);

        if (Archivo != null && Metadatos != null)
        {
            string usuarioFinal = PersonaId;

            string webRootPath = configuration["UploadTempDir"];
            string uploadsDir = Path.Combine(webRootPath, "uploads", usuarioFinal);

            if (!Directory.Exists(uploadsDir))
                Directory.CreateDirectory(uploadsDir);

            string fileName = Metadatos.Name;
            bool EsFoto = false;
            bool EsVideo = false;
            bool SinSoporte = true;
            bool EsAudio = false;
            bool Landscape = false;
            bool EsPDF = false;

            string FileId = fileName;
            string FullPath = Path.Combine(uploadsDir, FileId);
            string FileFrameId = $"{fileName.Split(".")[0]}.jpg";
            string FrameFullPath = Path.Combine(uploadsDir, FileFrameId);
            string Titulo = "";

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
                await cacheAlmacenamiento.CreaArchivoImagen(FullPath, fileName, uploadsDir, true);
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

                el = await AddElementoMedio(clienteId, usuarioFinal, ArchivoId,
                    TipoMedio.Galería, Metadatos.FileExtension, fileName.GetMimeTypeForFileExtension(),
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
        string usuarioId,
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

        el = await this.media.AddElemento(el, usuarioId);

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
     
    }
