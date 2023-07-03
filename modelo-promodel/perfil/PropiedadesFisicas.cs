using Newtonsoft.Json;

namespace promodel.modelo.perfil
{
    public class PropiedadesFisicas
    {
        public PropiedadesFisicas ()
        {

        }

        /// <summary>
        /// Indica si las unidades estan en MKS o el sistema ingles
        /// </summary>
        [JsonProperty("mks")]
        public bool MKS { get; set; } = true;

        /// <summary>
        /// Altura metros para MKS, Foot.Inch para ingles
        /// </summary>
        [JsonProperty("h")]
        public decimal Altura { get; set; } = 0;

        /// <summary>
        /// PEso en Kg para mks o libras para Inglés
        /// </summary>
        [JsonProperty("w")]
        public decimal Peso { get; set; } = 0;

        /// <summary>
        /// COlore de ojos de acuerdo al catálogo
        /// </summary>
        [JsonProperty("oj")]
        public string? ColorOjosId { get; set; } = "";

        /// <summary>
        /// Color de cabello de acuerdo al catálogo
        /// </summary>
        [JsonProperty("cb")]
        public string? ColorCabelloId { get; set; } = "";

        /// <summary>
        /// Identificador único del tipo de cabello
        /// </summary>
        [JsonProperty("tcb")]
        public string? TipoCabelloId { get; set; } = "";

        /// <summary>
        /// Etnia de acuerdo al catalogo
        /// </summary>
        [JsonProperty("et")]
        public string? EtniaId { get; set; } = "";

        /// <summary>
        /// Indice de masa corporal, se calcula en el backend al añadir o actualizar
        /// </summary>
        [JsonProperty("imc")]
        public decimal IMC { get; set; } = 0;


    }
}
