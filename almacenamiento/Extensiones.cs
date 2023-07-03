using Microsoft.AspNetCore.StaticFiles;

namespace almacenamiento
{
    public static class Extensiones
    {
        public static string GetMimeTypeForFileExtension(this string filePath)
        {
            const string DefaultContentType = "application/octet-stream";

            var provider = new FileExtensionContentTypeProvider();

            if (!provider.TryGetContentType(filePath, out string contentType))
            {
                contentType = DefaultContentType;
            }

            return contentType;
        }
    }
}
