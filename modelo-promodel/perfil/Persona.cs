using CouchDB.Driver.Types;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Principal;

namespace promodel.modelo.perfil
{

    /// <summary>
    /// Datos personales de un usuario de sistema
    /// </summary>
    public class Persona : CouchDocument
    {
        public Persona()
        {
            Clientes = new List<string>();
            IdiomasIds = new List<string>();
            ActividadesIds = new List<string>();
            Documentos = new List<Documento>();
        }

        /// <summary>
        /// Clientes con los que la persona es visible
        /// </summary>
        [JsonProperty("cls")]
        public List<string> Clientes { get; set; }

        [JsonProperty("consecutivo")]
        public int?  Consecutivo { get; set; }

        /// <summary>
        /// Imagen del avatar codificado base 64
        /// </summary>
        [JsonProperty("ava")]
        public string? AvatarBase64 { get; set; }

        /// <summary>
        /// Indica que la persona es exclusiva de la cartera de un cliente, cuando se encuentra marcado como verdarero
        /// la lista de clientes debe contener solo 1 elemnto                                                                                                                                                                                                                                  
        /// </summary>
        [JsonProperty("x")]
        [Required]
        public bool Exclusivo { get; set; }

        /// <summary>
        /// Usuario responsable de la persona
        /// </summary>
        [JsonProperty("uid")]
        public string? UsuarioId { get; set; }

        /// <summary>
        /// Nombre de la persona
        /// </summary>
        [JsonProperty("n")]
        
        public string Nombre { get; set; }

        /// <summary>
        /// Nombre artístico de la persona
        /// </summary>
        [JsonProperty("na")]
       
        public string NombreArtistico { get; set; }


        /// <summary>
        /// NOmbre utilziado para la búsqueda por nombre, no debe llenarse desde la UI
        /// </summary>
        [JsonProperty("nas")]
        public string NombreBusqueda { get; set; }

        /// <summary>
        /// 1er apellido de la persona
        /// </summary>
        [JsonProperty("a1")]
        public string Apellido1 { get; set; }

        /// <summary>
        /// 2o apellido de la persona
        /// </summary>
        [JsonProperty("a2")]
        public string Apellido2 { get; set; }

        /// <summary>
        /// Identificador de genero en base a PropiedadId del catálogo
        /// </summary>
        [JsonProperty("g")]
        public string GeneroId { get; set; }

        /// <summary>
        /// Fecha de nacimiento de la persona
        /// </summary>
        [JsonProperty("fn")]
        public DateTime? FechaNacimiento { get; set; }

        /// <summary>
        /// FEcha de nacimiento expresada en ticks para realizar bsuquedas
        /// </summary>
        [JsonProperty("tfn")]
        public long TicksFechaNacimiento { get; set; }

        /// <summary>
        /// Tipo de relación de la persona con el usuario registrado
        /// </summary>
        [JsonProperty("rel")]
        [Required]
        public TipoRelacionPersona Relacion { get; set; }

        /// <summary>
        /// Identificador de país de nacionaliad en base a PropiedadId del catálogo
        /// </summary>
        [JsonProperty("pa")]
        public string PaisOrigenId { get; set; }

        /// <summary>
        /// Identificador de país de residencia
        /// </summary>
        [JsonProperty("pac")]
        public string PaisActualId { get; set; }


        /// <summary>
        /// Identificador de país en base a PropiedadId del catálogo
        /// </summary>
        [JsonProperty("edo")]
        public string? EstadoPaisId { get; set; }

        /// <summary>
        /// Su bacionalidad es diferente a la del pais de la agencia
        /// </summary>
        [JsonProperty("ex")]
        public bool Extranjero { get; set; }


        /// <summary>
        /// Tiene permiso de trabajo en el pais de la agencia
        /// </summary>
        [JsonProperty("pt")]
        public bool PermisoTrabajo { get; set; }


        /// <summary>
        /// Identificador único de la zona horaria
        /// </summary>
        [JsonProperty("z")]
        public string? ZonaHorariaId { get; set; }

        /// <summary>
        /// Desviación horaria
        /// </summary>
        [JsonProperty("zo")]
        public decimal? OffsetHorario { get; set; }


        /// <summary>
        /// Idiomas hablados por la persona
        /// </summary>
        [JsonProperty("lang")]
        public List<string> IdiomasIds { get; set; }


        /// Agencias a las que pertenece la persona
        /// </summary>
        [JsonProperty("ag")]
        public List<string> AgenciasIds { get; set; }

        /// <summary>
        /// Id del folder de contenido por ejmeplo el id de ggogle para el folder
        /// </summary>
        [JsonProperty("fid")]
        public string? FolderContenidoId { get; set; }


        /// <summary>
        /// Lista de documentos solicitados por la gencia a la persona
        /// </summary>
        [JsonProperty("docs")]
        public List<Documento>? Documentos { get; set; }


        [JsonProperty("dok")]
        public bool? DocumentacionCompleta { get; set; } = false;

        /// <summary>
        /// Actividades realizadas por la persona
        /// </summary>
        [JsonProperty("act")]
        public List<string> ActividadesIds { get; set; }


        /// <summary>
        /// Detalle de las propiedades físicas del usuario
        /// </summary>
        [JsonProperty("pf")]
        public PropiedadesFisicas PropiedadesFisicas { get; set; }

        /// <summary>
        /// Detalle de las propiedades de vestuario del usuario
        /// </summary>
        [JsonProperty("pv")]
        public PropiedadesVestuario PropiedadesVestuario { get; set; }


        /// <summary>
        ///  Estas propeidades no se serializan en la base de datos sólo sirven para añadir nuevos tipos
        /// </summary>
        [JsonIgnore]
        public string? OtroIdioma { get; set; }

        [JsonIgnore]
        public string? OtroAgencia { get; set; }

        [JsonIgnore]
        public string? OtroColorOjos { get; set; }
        
        [JsonIgnore]
        public string? OtroColorCabello { get; set; }
        
        [JsonIgnore]
        public string? OtroTipoCabello { get; set; }
        
        [JsonIgnore]
        public string? OtroGrupoRacial { get; set; }

        [JsonIgnore] 
        public string? OtroHabilidad { get; set; }



        /// <summary>
        /// Identificador del elemento de medio para la foto principal
        /// </summary>
        [JsonProperty("epid")]
        public string? ElementoMedioPrincipalId { get; set; }

        [JsonProperty("cn")]
        public Contacto? Contacto { get; set; } = new Contacto();

        [NotMapped]
        public int Edad { get { return FechaNacimiento.DiferenciaAnos(); } }

        [NotMapped]
        public string EdadString
        {
            get
            {
                if (FechaNacimiento.HasValue)
                {
                    int a = FechaNacimiento.DiferenciaAnos();
                    if (a < 1)
                    {
                        return "<1";
                    }
                    else
                    {
                        return FechaNacimiento.DiferenciaAnos().ToString();
                    }
                }
                else { return ""; }
            }
        }
    }
}
