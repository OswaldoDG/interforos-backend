namespace promodel.modelo
{
    public class RespuestaLogin
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public DateTime UTCExpiration { get; set; }
    }
}
