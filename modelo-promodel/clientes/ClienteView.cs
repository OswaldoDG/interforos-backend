namespace promodel.modelo.clientes
{
    public class ClienteView
    {
        public string Nombre { get; set; }

        public string Url { get; set; }

        public bool Activo { get; set; }

        public string? WebLogoBase64 { get; set; }

        public string? MailLogoURL { get; set; }

        public Contacto Contacto { get; set; }

        public List<DocumentoModelo> Documentacion { get; set; }

        /// <summary>
        /// Consentimientos del cliente
        /// </summary>
        public List<Consentimiento> Consentimientos { get; set; }
    }
}
