﻿using almacenamiento;
using api_promodel.Controllers.publico;
using ImageMagick;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using promodel.modelo.media;
using promodel.modelo.perfil;
using promodel.servicios;
using promodel.servicios.media;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Reflection;
using System.Reflection.PortableExecutable;

namespace api_promodel.Controllers.personas
{
    [Route("contenido")]
    [Authorize]
    [ApiController]
    public class ContenidoController : ControllerPublico
    {
        public class Medio
        {
            public string? Id { get; set; }
            public IFormFile formFile { get; set; }

            public string? Titulo { get; set; }
        }

        private readonly IServicioPersonas personas;
        private readonly IConfiguration configuration;
        private readonly IAlmacenamiento almacenamiento;
        private readonly IMedia media;
        private readonly ICacheAlmacenamiento cacheAlmacenamiento;

        public ContenidoController(
            IServicioPersonas personas,
            ICacheAlmacenamiento cacheAlmacenamiento,
            IConfiguration configuration, 
            IAlmacenamiento almacenamiento,
            IMedia media, IServicioClientes servicioClientes) : base(servicioClientes)
        {
            this.cacheAlmacenamiento = cacheAlmacenamiento;
            this.configuration = configuration;
            this.almacenamiento = almacenamiento;
            this.media = media;
            this.personas = personas;
        }


        [HttpGet("mi", Name = "MisMedios")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<MediaCliente>> MisFotos()
        {
            var mm=  await media.GetByUsuarioId(UsuarioId);
            if(mm!=null)
            {
                return Ok(mm.ToMediaClienteFotosYVideo(ClienteId));
            }
            return NotFound();
        }


        [HttpGet("medios/{id}", Name = "MediosModelo")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<MediaCliente>> MediosModelo(string id)
        {
            var mm = await media.GetByUsuarioId(id);
            if (mm != null)
            {
                return Ok(mm.ToMediaClienteFotosYVideo(id));
            }
            return NotFound();
        }

        [HttpGet("principal/{id}", Name = "FotoPrincipal")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<MediaCliente>> FotoPrincipal(string id)
        {
            var mm = await media.EstablecerPrincipal(UsuarioId, id);
            if (mm)
            {
                var pp = await personas.EstableceFotoPrincipal(UsuarioId, id);
                if (pp)
                {
                    return Ok();
                }
            }
            return NotFound();
        }


        [HttpGet("bloqueo/{id}", Name = "MedioPermanente")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<MediaCliente>> FotoPermanente(string id)
        {
            var mm = await media.AlternarBloqueo(UsuarioId, id);
            if (mm)
            {
                return Ok();
            }
            return NotFound();
        }


        [HttpDelete("{id}", Name = "MedioEliminar")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<MediaCliente>> FotoEliminar(string id)
        {
            var mm = await media.EliminarElemento(UsuarioId, id);
            if (mm)
            {
                return Ok();
            }
            return NotFound();
        }


        [HttpGet("stream/{usuarioid}/{id}/{tipo}", Name = "StreamDeId")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<FileStreamResult> StreamById(string usuarioid, string id, string tipo)
        {
            var a = cacheAlmacenamiento.FotoById(usuarioid, id, tipo);
            Console.WriteLine(a);
            var stream = System.IO.File.OpenRead(a);
            return new FileStreamResult(stream, "video/mp4");


            //FileInfo fi = new FileInfo(a);
            //string type = a.GetMimeTypeForFileExtension();
            //var stream = System.IO.File.OpenRead(a);
            //return stream;
        }


        [HttpGet("{usuarioid}/{id}/{tipo}",  Name = "MedioDeId")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult FotoById(string usuarioid, string id, string tipo)
        {
   
            if (Guid.Empty.ToString() == id)
            {
                string ruta = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "archivos", "demo", "fotos");
                var files = Directory.GetFiles(ruta, "*.jpg");
                Random r = new Random(DateTime.Now.Millisecond);
                int n = r.Next(1, files.Length);
                var image = System.IO.File.OpenRead(files[n]);
                return File(image, files[n].GetMimeTypeForFileExtension());
            }
            else
            {
                var a = cacheAlmacenamiento.FotoById(usuarioid, id, tipo);
                if(a!=null)
                {
                    FileInfo fi = new FileInfo(a);
                    var stream = System.IO.File.OpenRead(a);

                    return new FileStreamResult(stream, a.GetMimeTypeForFileExtension())
                    {
                        EnableRangeProcessing = false, FileDownloadName = fi.Name
                    };
                } else
                {
                    return NotFound();
                }


            }
        }


        [HttpPost("documentacion", Name = "UploadDocumentacion")]
        [DisableRequestSizeLimit]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ElementoMediaCliente>> UploadDocumentacion([FromForm] Medio documento)
        {
            if (string.IsNullOrEmpty(UsuarioId) || string.IsNullOrEmpty(ClienteId))
                return BadRequest();

            if (!Request.Form.Files.Any())
                return BadRequest("No files found in the request");

            if (Request.Form.Files.Count > 1)
                return BadRequest("Cannot upload more than one file at a time");

            if (Request.Form.Files[0].Length <= 0)
                return BadRequest("Invalid file length, seems to be empty");

            bool documentoValido = false;
            var cliente = await this.Cliente();

            if (cliente != null)
            {
                if (cliente.Documentacion.Any(x => x.Id == documento.Id))
                {
                    documentoValido = true;
                }
            }

            if (!documentoValido)
            {
                return BadRequest("Documento no válido");
            }

            var queryPersona = await personas.PorUsuarioId(this.UsuarioId);
            Persona p = null;
            if (queryPersona.Ok)
            {
                p = (Persona)queryPersona.Payload;
            }
            else
            {
                return BadRequest("Persona inexistente");
            }
            


            if (!string.IsNullOrEmpty(p.FolderContenidoId))
            {

                // Elimina el contenido anterior si ya existe para el elemento
                var plantilla = cliente.Documentacion.FirstOrDefault(d => d.Id == documento.Id);
                var doc = p.Documentos.FirstOrDefault(d => d.Id == documento.Id);
                if(doc!=null)
                {
                    await almacenamiento.DeleteFile(ClienteId, doc.IdAlmacenamiento);
                    await this.media.DelElemento(doc.IdAlmacenamiento, UsuarioId);
                }

                string FolderId = p.FolderContenidoId;
                string webRootPath = configuration["UploadTempDir"];
                string uploadsDir = Path.Combine(webRootPath, "uploads", UsuarioId);

                StorageObjectDescriptor folderDocumentacion = await almacenamiento.CreateFolder(ClienteId, "Documentos", FolderId);
     
                if (!Directory.Exists(uploadsDir))
                    Directory.CreateDirectory(uploadsDir);

                IFormFile file = Request.Form.Files[0];
                string fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"').ToLower();
                FileInfo fi = new FileInfo(fileName);
                bool Landscape = false;
                bool ExtensionValida = false;
                string tempId = Guid.NewGuid().ToString();
                string FileId = $"{tempId}{fi.Extension.ToLower()}";
                string FullPath = Path.Combine(uploadsDir, FileId);
                bool esImagen = false;

                if (".jpg,.jpeg,.pdf".Contains(fi.Extension.ToLower(), StringComparison.CurrentCulture))
                {
                    ExtensionValida = true;
                    if (".jpg,.jpeg".Contains(fi.Extension.ToLower(), StringComparison.CurrentCulture))
                    {
                        esImagen = true;
                    }
                }

                if (!ExtensionValida)
                {
                    return BadRequest("Tipo de medio no válido");
                }

                using (var stream = new FileStream(FullPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                if(esImagen)
                {
                    using (var image = new MagickImage(FullPath))
                    {
                        image.AutoOrient();
                        if (image.Width > image.Height)
                        {
                            Landscape = true;
                        }
                    }
                }
                

                StorageObjectDescriptor archivoAlmacenado = null;

                if (System.IO.File.Exists(FullPath))
                {
                    ///Crea el archvo en el almacenamiento para obtener su Id
                    archivoAlmacenado = await almacenamiento.CreateFile(ClienteId, FullPath, plantilla.Nombre, folderDocumentacion.Id);
                }
                else
                {
                    throw new Exception("No fue posible crear el archivo");
                }


                ElementoMedia el = null;
                if (archivoAlmacenado != null)
                {
                    // Añade el registro a la base de datos
                    FileInfo FiSaved = new(FullPath);
                    el = await AddElementoMedio(archivoAlmacenado.Id,
                         TipoMedio.Documento, fi.Extension, fileName.GetMimeTypeForFileExtension(),
                         FiSaved.Length, false, false, true, Landscape);

                    await personas.UpsertLinkDocumento(ClienteId, UsuarioId, documento.Id, archivoAlmacenado.Id);
                    await cacheAlmacenamiento.CreaArchivoImagen(FullPath, $"{archivoAlmacenado.Id}{fi.Extension}", UsuarioId, esImagen);
                }

                try
                {
                    System.IO.File.Delete(FullPath);
                }
                catch (Exception)
                {
                }

                if (el != null)
                {
                    return Ok(el.ToElementoMediaCliente());
                }
            }

            // Algo anda mal con el request
            throw new Exception("No fue posible añadir el medio");
        }


        [HttpPost("carga", Name = "UploadMedio")]
        [DisableRequestSizeLimit]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ElementoMediaCliente>> UploadFoto([FromForm] Medio photo)
        {
            if (string.IsNullOrEmpty(UsuarioId))
                return BadRequest();

            if (!Request.Form.Files.Any())
                return BadRequest("No files found in the request");

            if (Request.Form.Files.Count > 1)
                return BadRequest("Cannot upload more than one file at a time");

            if (Request.Form.Files[0].Length <= 0)
                return BadRequest("Invalid file length, seems to be empty");

            var queryPersona = await personas.PorUsuarioId(this.UsuarioId);
            Persona p = null;
            if (queryPersona.Ok)
            {
                p = (Persona)queryPersona.Payload;
            } else
            {
                return BadRequest("Persona inexistente");
            }


            if (!string.IsNullOrEmpty(p.FolderContenidoId))
            {
                string FolderId = p.FolderContenidoId;
                string webRootPath = configuration["UploadTempDir"];
                string uploadsDir = Path.Combine(webRootPath, "uploads", UsuarioId);

                if (!Directory.Exists(uploadsDir))
                    Directory.CreateDirectory(uploadsDir);

                IFormFile file = Request.Form.Files[0];
                string fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"').ToLower();
                FileInfo fi = new FileInfo(fileName);
                bool EsFoto = false;
                bool EsVideo = false;
                bool SinSoporte = true;
                bool EsAudio = false;
                bool Landscape = false;
                bool EsPDF = false;
                string tempId = Guid.NewGuid().ToString();
                string FileId = $"{tempId}{fi.Extension.ToLower()}";
                string FullPath = Path.Combine(uploadsDir, FileId);
                string FileFrameId = $"{tempId}.jpg";
                string FrameFullPath = Path.Combine(uploadsDir, FileFrameId);
                string Titulo = "";

                if (".jpg,.jpeg,.png,.gif".Contains(fi.Extension.ToLower(), StringComparison.CurrentCulture))
                {
                    EsFoto = true;
                    SinSoporte = false;
                }

                if (".mp4,.mpeg,.mpg,.mov,.wmv,.ogg".Contains(fi.Extension.ToLower(), StringComparison.CurrentCulture))
                {
                    EsVideo = true;
                    SinSoporte = false;
                }

                if (".mp3,.ogg".Contains(fi.Extension.ToLower(), StringComparison.CurrentCulture))
                {
                    EsAudio = true;
                    SinSoporte = false;
                }

                if (".pdf".Contains(fi.Extension.ToLower(), StringComparison.CurrentCulture))
                {
                    EsPDF = true;
                    SinSoporte = false;
                }

                //if (!ExtensionValida)
                //{
                //    return BadRequest("Tipo de medio no válido");
                //}

                using (var stream = new FileStream(FullPath, FileMode.Create))
                {
                    file.CopyTo(stream);
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
                }

                if (EsVideo)
                {
                    FrameFullPath = ObtieneFrame(FullPath, FrameFullPath);
                    if (FrameFullPath == null)
                    {
                        return BadRequest("No fue posible obtener un frame del video");
                    }
                }


                StorageObjectDescriptor archivoAlmacenado = null;
                StorageObjectDescriptor frameAlmacenado = null;

                if (System.IO.File.Exists(FullPath))
                {
                    ///Crea el archvo en el almacenamiento para obtener su Id
                    archivoAlmacenado = await almacenamiento.CreateFile(ClienteId, FullPath, FileId, FolderId);
                }
                else
                {
                    throw new Exception("No fue posible crear el archivo");
                }

                if (EsVideo)
                {
                    frameAlmacenado = await almacenamiento.CreateFile(ClienteId, FrameFullPath, FileFrameId, FolderId);
                }


                ElementoMedia el = null;
                if (archivoAlmacenado != null)
                {
                    // Añade el registro a la base de datos
                    FileInfo FiSaved = new(FullPath);

                    el = await AddElementoMedio(archivoAlmacenado.Id, 
                        TipoMedio.Galería , fi.Extension, fileName.GetMimeTypeForFileExtension(),
                        FiSaved.Length, EsFoto, EsVideo, EsAudio, SinSoporte, EsPDF, Landscape, frameAlmacenado?.Id, photo.Titulo);

                    await cacheAlmacenamiento.CreaArchivoImagen(FullPath, $"{archivoAlmacenado.Id}{fi.Extension}", UsuarioId, EsFoto);
                    if (EsVideo)
                    {
                        await cacheAlmacenamiento.CreaArchivoImagen(FrameFullPath, $"{frameAlmacenado.Id}.jpg", UsuarioId, true);
                    }
                }

                try
                {
                    System.IO.File.Delete(FullPath);
                }
                catch (Exception)
                {
                }

                if (el != null)
                {
                    return Ok(el.ToElementoMediaCliente());
                }
            }

            // Algo anda mal con el request
            throw new Exception("No fue posible añadir el medio");
        }


        private async Task<ElementoMedia> AddElementoMedio(
            string Id,
            TipoMedio Tipo,
            string Extension, 
            string MimeType, 
            long Totales, 
            bool EsFoto=true, 
            bool EsVideo=false,
            bool EsAudio = false,
            bool SinSoporte = true,
            bool Pdf = false,
            bool Landscape = true, 
            string? FrameId = null, 
            string? Titulo = null
            )
        {

            ElementoMedia el = new ElementoMedia()
            {
                Id = Id,
                ClienteIds = new List<string>() { ClienteId },
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
                Tipo = Tipo ,
                Audio = EsAudio,
                SinSoporteWeb = SinSoporte,
                Titulo = Titulo
            };

            el = await this.media.AddElemento(el, UsuarioId);

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
            //startInfo.RedirectStandardError = true;

            //Console.WriteLine(string.Format(
            //    "Executing \"{0}\" with arguments \"{1}\".\r\n",
            //    startInfo.FileName,
            //    startInfo.Arguments));

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

}
