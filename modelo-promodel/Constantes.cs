using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace promodel.modelo
{

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TipoRolCliente
    {
        Administrador = 0, Staff = 1, Modelo=2, RevisorExterno=3, Agencia = 4, Ninguno = -1
    }



    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum EstadoCasting
    {
        EnEdicion = 0, Abierto = 1, Cerrado = 2, Cancelado = 3
    }


    /// <summary>
    /// Giro de la empresa que hospeda el dominio
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum GiroDominio
    {
        Agencia=0, Castinera=1
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TipoRelacionContacto
    {
        Residencia =0, 
        Trabajo = 1, 
        Notificacion = 2
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TipoTelefono
    {
        Fijo = 0,
        Movil = 1
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TipoRelacionPersona
    {
        /// <summary>
        /// LA persona registrada soy yo
        /// </summary>
        Yo=0, 
        /// <summary>
        /// Soy padre, madre o tutor de la persona
        /// </summary>
        PadreTutor=1,
        /// <summary>
        /// Soy apoderado legal de la persona
        /// </summary>
        ApoderadoLegal = 2, 
        /// <summary>
        /// Es un contacto de mi catálogo de agencia
        /// </summary>
        Agencia=100
    }
}
