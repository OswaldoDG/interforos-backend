using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace promodel.modelo.castings;

public class VotoModeloCategoria
{

    /// <summary>
    /// Identificador único del usuario que realizó el voto, DEBE utilizarse como clave
    /// Solo puede haber un por usuario y el usuario debe ser un revisor los STAFF no votan
    /// </summary>
    [JsonProperty("u")]
    public string UsuarioId { get; set; }



    /// <summary>
    /// FEcha del comentario en Ticks
    /// </summary>
    [JsonProperty("f")]

    public DateTime Fecha { get; set; } = DateTime.UtcNow;


    /// <summary>
    /// Determina el grado de aceptación de un modelo en el casting
    /// Los valores ´validos son 
    /// 0 = no me gusta 
    /// 1 = no sé si me gusta
    /// 2 = me gusta
    /// 3 = me gusta mucho
    /// </summary>
    [JsonProperty("n")]
    public int NivelLike { get; set; }

}
