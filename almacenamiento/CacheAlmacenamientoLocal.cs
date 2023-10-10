using ImageMagick;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using static System.Net.Mime.MediaTypeNames;

namespace almacenamiento
{
    public class CacheAlmacenamientoLocal : ICacheAlmacenamiento
    {
        public const string TAMANO_FULL = "full";
        public const string TAMANO_CARD = "card";
        public const string TAMANO_THUMB = "thumb";

        private readonly CacheAlmacenamientoLocalConfig config;
        private readonly IAlmacenamiento almacenamiento;

        public CacheAlmacenamientoLocal(IAlmacenamiento almacenamiento, IOptions<CacheAlmacenamientoLocalConfig> options)
        {
            this.config = options.Value;
            this.almacenamiento = almacenamiento;
        }

        public async Task VerificaArchivo(string ClientId, string FileId)
        {
            await this.almacenamiento.DownloadFile(ClientId, FileId);
        }

        public async Task EliminaArchivo(string ClientId, string NombreArchivo, string Folder)
        {
            string Ruta = Path.Combine(config.Ruta, Folder, NombreArchivo);

            if (File.Exists(Ruta))
            {
                File.Delete(Ruta);
            }
        }
        public async Task<string?> FotoById(string ClientId, string usuarioid, string id, string tipo)
        {

            string dir = Path.Combine(config.Ruta, usuarioid);
           
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

                var archivo = Directory.GetFiles(dir, $"{id}-{tipo}.*").ToList().FirstOrDefault();
                if(archivo!= null)
                {
                    return archivo;
                } else
                {
                    var file = await almacenamiento.DownloadFile(ClientId, id);
                    await CreaArchivoImagen(file, $"{id}", usuarioid,true);
                    return Directory.GetFiles(dir, $"{id}-{tipo}.*").ToList().FirstOrDefault();
                }
               }

        public async Task CreaArchivoImagen(MemoryStream Archivo, string NuevoNombre, string Folder, bool EsImagen)
        {
            if (Archivo!=null)
            {
                string dir = Path.Combine(config.Ruta, Folder);
                var ImgGoogleDrive = $"{dir}\\{NuevoNombre}.jpg";
                FileStream file = new FileStream(ImgGoogleDrive, FileMode.Create, FileAccess.Write);
                Archivo.WriteTo(file);
                file.Close();
                Archivo.Close();
                                              
                FileInfo fi = new(ImgGoogleDrive);
                string Ruta = Path.Combine(dir, NuevoNombre);
                string FullFile = $"{Ruta.Replace(fi.Extension, "")}-{TAMANO_FULL}{fi.Extension}";
                File.Copy(ImgGoogleDrive, FullFile);
                File.Delete(ImgGoogleDrive);
                if (EsImagen && File.Exists(FullFile))
                {
                    await CreaImagen(FullFile, TAMANO_CARD, fi.Extension, config.Quality, config.TamanoCard);
                    await CreaImagen(FullFile, TAMANO_THUMB, fi.Extension, config.Quality, config.TamanoThumb);
                }
            }
        }
        public async Task CreaArchivoImagen(string Archivo, string NuevoNombre, string Folder, bool EsImagen)
        {
            if(File.Exists(Archivo))
            {
                string dir = Path.Combine(config.Ruta, Folder);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                
                FileInfo fi = new(Archivo);
                string Ruta = Path.Combine(dir, NuevoNombre);
                string FullFile = $"{Ruta.Replace(fi.Extension, "")}-{TAMANO_FULL}{fi.Extension}";
                File.Copy(Archivo, FullFile);

                if (EsImagen && File.Exists(FullFile))  
                {
                    await CreaImagen(FullFile, TAMANO_CARD, fi.Extension, config.Quality, config.TamanoCard);
                    await CreaImagen(FullFile, TAMANO_THUMB, fi.Extension, config.Quality, config.TamanoThumb);
                }
            }
        }

        private async Task<string> CreaImagen(string fullPath, string Tipo, string Extension, int quality, int MaxLen)
        {
            int ancho;
            int alto;
            using (var image = new MagickImage(fullPath))
            {
                image.AutoOrient();
                //Cambiando resolucion
            
                if (image.Width > image.Height)
                {
                    
                    ancho = MaxLen;
                    alto = (MaxLen * image.Height) / image.Width;

                }
                else
                {
                     ancho = (MaxLen * image.Width) / image.Height;
                     alto = MaxLen;
             
                   
                }
                var size = new MagickGeometry(ancho, alto);
                image.Resize(size);
                image.Quality = quality;

                string name = $"{fullPath.Replace($"-{TAMANO_FULL}","").Replace(Extension, "")}-{Tipo}{Extension}";

                await image.WriteAsync(name);

                return name;
            }
        }
    }
}
