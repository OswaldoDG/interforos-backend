namespace promodel.modelo.perfil
{
    public class PersonaView
    {
        public bool Exclusivo { get; set; }

        public string? UsuarioId { get; set; }

        public string? Nombre { get; set; }

        public string? NombreArtistico { get; set; }

        public string? Apellido1 { get; set; }

        public string? Apellido2 { get; set; }

        public string? GeneroId { get; set; }

        public DateTime? FechaNacimiento { get; set; }

        public TipoRelacionPersona Relacion { get; set; }

        public string? PaisOrigenId { get; set; }

        public string? EstadoPaisId { get; set; }

        public bool? Extranjero { get; set; }

        public bool? PermisoTrabajo { get; set; }

        public string? ZonaHorariaId { get; set; }

        public decimal? OffsetHorario { get; set; }

        public List<string> IdiomasIds { get; set; }

        public List<string> ActividadesIds { get; set; }

        public PropiedadesFisicas? PropiedadesFisicas { get; set; }

        public PropiedadesVestuario? PropiedadesVestuario { get; set; }

        public string? ElementoMedioPrincipalId { get; set; }

        public Contacto? Contacto { get; set; }
    }
}
