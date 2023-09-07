using Humanizer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using promodel.modelo;
using promodel.modelo.registro;

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

        public static string LeeTemaRegistro(this IConfiguration _configuration, InvitacionRegistro inv)
        {

            switch (inv.Registro.Rol)
            {
                case TipoRolCliente.Staff:
                    return _configuration.GetValue<string>("emailing:tema-email-registro-staff");
                   
                case TipoRolCliente.Modelo:
                    return _configuration.GetValue<string>("emailing:tema-email-registro");
                   
                case TipoRolCliente.RevisorExterno:
                    return _configuration.GetValue<string>("emailing:tema-email-registro-revisor");
                case TipoRolCliente.Agencia:
                    return _configuration.GetValue<string>("emailing:tema-email-registro");

                default:
                    return null;

            }

            
        }

        public static string LeeTemaTipoServicio(this IConfiguration _configuration, SolicitudSoporteUsuario solicitudSoporteUsuario)
        {
            switch (solicitudSoporteUsuario.Tipo)
            {
                case TipoServicio.RecuperacionContrasena:
                    return _configuration.GetValue<string>("emailing:tema-email-restablecimiento-contraseña");

                default:
                    return null;

            }
        }

        public static string LeePlantillaRegistro(this IConfiguration _configuration, IWebHostEnvironment _environment, InvitacionRegistro inv)
        {
            string plantilla="";

            switch (inv.Registro.Rol)
            {
                case TipoRolCliente.Staff:
                    plantilla = _configuration.GetValue<string>("emailing:plantilla-email-registro-staff");
                    break;
                case TipoRolCliente.Modelo:

                    if (!string.IsNullOrEmpty(inv.Registro.Mensaje))
                    {
                        plantilla = _configuration.GetValue<string>("emailing:plantilla-email-invitacion-modelo");
                    }
                    else
                    {
                        plantilla = _configuration.GetValue<string>("emailing:plantilla-email-registro");
                    }
                    break;
                case TipoRolCliente.RevisorExterno:
                    plantilla = _configuration.GetValue<string>("emailing:plantilla-email-registro-revisor");
                    break;
                case TipoRolCliente.Agencia:
                    plantilla = _configuration.GetValue<string>("emailing:plantilla-email-registro");
                    break;

            }
            string ruta = Path.Combine(_environment.ContentRootPath, plantilla);
            string contenido = "";
            if (System.IO.File.Exists(ruta))
            {
                contenido = System.IO.File.ReadAllText(ruta);
            }
            return contenido;
        }

        public static string LeePlantillaTipoServcio(this IConfiguration _configuration, IWebHostEnvironment _environment, SolicitudSoporteUsuario solicitudSoporteUsuario)
        {
            string plantilla = "";
            switch (solicitudSoporteUsuario.Tipo)
            {
                case TipoServicio.RecuperacionContrasena:
                    plantilla = _configuration.GetValue<string>("emailing:plantilla-email-restablecer-password");
                    break;
            }

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
