using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using Serilog;

namespace comunicaciones.email
{
    public class ServicioEmailSMTP : IServicioEmail
    {
        private IMessageBuilder _messageBuilder;
        private SMTPConfig _SMTPConfig;
        public ServicioEmailSMTP(IMessageBuilder messageBuilder, IOptions<SMTPConfig> SMTPConfig)
        {
            _SMTPConfig = SMTPConfig.Value;
            _messageBuilder = messageBuilder;
        }

        public async Task<bool> Enviar(MensajeEmail msg)
        {
            try
            {
                msg.PlantillaCuerpo = _messageBuilder.FromTemplate(msg.PlantillaCuerpo, msg.JSONData);
                msg.PlantillaTema = _messageBuilder.FromTemplate(msg.PlantillaTema, msg.JSONData);
                
                var result  = await EnviarCorreo(msg.PlantillaTema, msg.PlantillaCuerpo, 
                    msg.DireccionPara, msg.NombrePara, _SMTPConfig.FromEmail, _SMTPConfig.From);
                return result;
            
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return false;
            };
        }

        public async Task<bool> EnviarCorreo(string subject, string body, string email, string nombre, string emailDe, string nombreDe)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(nombreDe, emailDe));
            message.To.Add(new MailboxAddress(nombre, email));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = body
            };

            message.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                client.Connect(_SMTPConfig.Server, _SMTPConfig.Port,  MailKit.Security.SecureSocketOptions.Auto);

                if (_SMTPConfig.Authenticated)
                {
                    client.Authenticate(_SMTPConfig.User, _SMTPConfig.Password);
                }
                await client.SendAsync(message);
                client.Disconnect(true);
            }

            return true;
        }


    }
}
