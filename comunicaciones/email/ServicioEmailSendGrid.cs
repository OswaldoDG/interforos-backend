using Microsoft.Extensions.Options;
using SendGrid.Helpers.Mail;
using SendGrid;
using Serilog;

namespace comunicaciones.email
{
    public class ServicioEmailSendGrid : IServicioEmail
    {
        private IMessageBuilder _messageBuilder;
        private SMTPConfig _SMTPConfig;

        public ServicioEmailSendGrid(IMessageBuilder messageBuilder, IOptions<SMTPConfig> SMTPConfig)
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

                var result = await EnviarCorreo(msg.PlantillaTema, msg.PlantillaCuerpo,
                    msg.DireccionPara, msg.NombrePara, _SMTPConfig.FromEmail, _SMTPConfig.From);
                return result;

            }
            catch (Exception ex)
            {
                File.AppendAllText("log.txt", ex.ToString() + "\r\n");
                Log.Error(ex.Message);
                return false;
            };
        }

        public async Task<bool> EnviarCorreo(string subject, string body, string email, string nombre, string emailDe, string nombreDe)
        {
            var apiKey = _SMTPConfig.SendgridKey;
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(emailDe, nombreDe);
            var to = new EmailAddress(email, nombre);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, null, body);
            var response = await client.SendEmailAsync(msg);

            if(!response.IsSuccessStatusCode)
            {
                File.AppendAllText("log.txt", response.Body + "\r\n");
            }

            return response.IsSuccessStatusCode;
        }

    }
}
