﻿
using Newtonsoft.Json;
using promodel.modelo.perfil;

namespace promodel.modelo.castings;

public class ModeloCastingReview
{
    /// <summary>
    /// modelo en el casting
    /// </summary>
    public string PersonaId { get; set; }
    /// <summary>
    /// consecutivo del modelo en el casting
    /// </summary>
    public int? Consecutivo { get; set; }
    /// <summary>
    /// Fecha de adición del modelo al casting
    /// </summary>
    public DateTime FechaAdicion { get; set; }
}
