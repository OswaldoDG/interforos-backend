namespace comunicaciones.email
{
    public class SMTPConfig
    {
        public string Server { get; set; }
        public int Port { get; set; }
        public bool SSL { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public bool Authenticated { get; set; }
        public string From { get; set; }
        public string FromEmail { get; set; }
        public string SendgridKey { get; set; }
    }
}

