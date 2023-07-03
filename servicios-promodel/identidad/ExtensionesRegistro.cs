using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace promodel.servicios.identidad
{
    public static class ExtensionesRegistro
    {
        public static string LeeUrlBase(this IConfiguration _configuration)
        {
            return _configuration.GetValue<string>("emailing:url-base");
        }
        public static string LeeUrlBaseActivacion(this IConfiguration _configuration)
        {
            return _configuration.GetValue<string>("BaseConnectUrl:urlRegistro");
        }

        public static string LeeTemaRegistro(this IConfiguration _configuration)
        {
            return _configuration.GetValue<string>("emailing:tema-email-registro");
        }

        public static string LeePlantillaRegistro(this IConfiguration _configuration, IWebHostEnvironment _environment)
        {
            string plantilla = _configuration.GetValue<string>("emailing:plantilla-email-registro");
            string ruta = Path.Combine(_environment.ContentRootPath, plantilla);
            string contenido = "";
            if (System.IO.File.Exists(ruta))
            {
                contenido = System.IO.File.ReadAllText(ruta);
            }
            return contenido;
        }
    }
}
