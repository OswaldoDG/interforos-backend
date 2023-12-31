﻿using CouchDB.Driver.Types;
using Newtonsoft.Json;
using promodel.modelo.castings;

namespace promodel.modelo.proyectos
{
    /// <summary>
    /// Proyecto para la evaluación de usurios para casting
    /// </summary>
    public class Casting : CouchDocument
    {

        [JsonProperty("cl")]
        public string? ClienteId { get; set; }

        /// <summary>
        /// Nombre del proyecto
        /// </summary>

        [JsonProperty("n")]
        public string Nombre { get; set; }


        [JsonProperty("stt")]
        public EstadoCasting Status { get; set; } = EstadoCasting.EnEdicion;


        /// <summary>
        /// NOmbre del cliente para quien va el proyecto por ejemplo la emrpesa
        /// </summary>
        [JsonProperty("ncl")]
        public string NombreCliente { get; set; }

        /// <summary>
        /// fecha de creación del registro
        /// </summary>
        [JsonProperty("f")]
        public long? FechaCreacionTicks { get; set; }


        /// <summary>
        /// Identificador del usuario que cre el proyectp
        /// </summary>
        [JsonProperty("u")]
        public string? UsuarioId { get; set; }

        /// <summary>
        /// FEcha opcional de apertura a los candidatos y revisores del proyecto
        /// </summary>
        [JsonProperty("fa")]
        public DateTime? FechaApertura { get; set; }

        /// <summary>
        /// Fecha opcional de cierre del proyecto 
        /// </summary>
        [JsonProperty("fc")]
        public DateTime? FechaCierre { get; set; }


        [JsonProperty("autoi")]
        public bool AceptaAutoInscripcion { get; set; } = false;

        /// <summary>
        /// Lista de staff o externos participantes en el proyecto
        /// </summary>
        [JsonProperty("ctc")]
        public List<ContactoCasting>? Contactos { get; set; }

        [JsonProperty("ds")]
        public string? Descripcion { get; set; }

        /// <summary>
        /// Comentarios relacionados al proyecto
        /// </summary>
        [JsonProperty("cm")]
        public List<ComentarioCasting>? Comentarios { get; set; } = new List<ComentarioCasting>();


        /// <summary>
        /// Identificadores de los colaboradores para el filtro de busqueda de Castings en los que participa
        /// </summary>
        [JsonProperty("colabs")]
        public List<string>? ColaboradoresIds { get; set; }

        /// <summary>
        /// Indica si el proyecto se encuentra activo
        /// </summary>
        [JsonProperty("a")]
        public bool Activo { get; set; } = true;


        /// <summary>
        /// Indica si el proyecto se marca como activo en base a su fecha de apertura
        /// </summary>
        [JsonProperty("autoa")]
        public bool AperturaAutomatica { get; set; } = true;

        /// <summary>
        /// Especifica si el proyecto debe marcarse como inactivo al superarse la fecha de cierro
        /// </summary>
        [JsonProperty("c")]
        public bool CierreAutomatico { get; set; } = true;

        /// <summary>
        /// Categorías para la selección de modelos
        /// </summary>
        [JsonProperty("ca")]
        public List<CategoriaCasting>? Categorias { get; set; } = new List<CategoriaCasting>();

        /// <summary>
        /// Imagen para el casting en formato base 64 
        /// </summary>
        [JsonProperty("logo")]
        public string? LogoCasting { get; set; }

        [JsonProperty("pext")]
        public PermisosCasting PernisosEcternos { get; set;  } =new PermisosCasting();  

        /// <summary>
        /// Eventos asociados al casting
        /// </summary>
        public List<EventoCasting> Eventos { get; set; } = new List<EventoCasting>();

        /// <summary>
        /// Identificador único del folder de almacenamiento en google drive del castins
        /// </summary>
        [JsonProperty("fid")]
        public string? FolderId { get; set; }
    }
}
