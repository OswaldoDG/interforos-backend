using Newtonsoft.Json;

namespace promodel.modelo.perfil
{
    public enum TipoTalla
    {
        MX =0 , US=1, UK=2
    }

    public class PropiedadesVestuario
    {
        [JsonProperty("pan")]
        public decimal Pantalon { get; set; } = 0;

        [JsonProperty("ply")]
        public decimal Playera { get; set; } = 0;

        [JsonProperty("cal")] 
        public decimal Calzado { get; set; } = 0;

        [JsonProperty("tt")]
        public string TipoTallaId { get; set; } = "";
    }
}
