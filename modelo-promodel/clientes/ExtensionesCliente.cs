namespace promodel.modelo.clientes
{
    public static class ExtensionesCliente
    {

        public static ClienteView ToClienteView(this Cliente cliente)
        {
            ClienteView c = new ClienteView()
            {
                Activo = cliente.Activo,
                MailLogoURL = cliente.MailLogoURL,
                Nombre = cliente.Nombre,
                Url = cliente.Url,
                WebLogoBase64 = cliente.WebLogoBase64,
                MostrarConsentimientos = cliente.MostrarConsentimientos
                
                
                
            };

            c.Contacto = cliente.Contacto;
            c.Documentacion = cliente.Documentacion;
            c.Consentimientos= cliente.Consentimientos;

            return c;
        }


        
    }
}
