using CouchDB.Driver;
using CouchDB.Driver.Options;
using promodel.modelo.perfil;
using promodel.servicios.personas;

namespace promodel.servicios
{
    public class PersonasCouchDbContext : CouchContext
    {
        public const string IDX_PERSONA_X_UID = "persona_x_uid";
        public const string IDX_PERSONA_X_BUSCAR = "persona_buscar";
        public const string IDX_PERSONA_X_CONSEC = "persona_x_consecutivo";

        public PersonasCouchDbContext(CouchOptions<PersonasCouchDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(CouchOptionsBuilder optionsBuilder)
        {

        }

        public CouchDatabase<Persona> Personas { get; set; }


        protected override void OnDatabaseCreating(CouchDatabaseBuilder databaseBuilder)
        {
            databaseBuilder.Document<Persona>().ToDatabase("personas");

            databaseBuilder.Document<Persona>()
              .HasIndex(IDX_PERSONA_X_UID, b => b.IndexBy(b => b.UsuarioId));


            databaseBuilder.Document<Persona>()
              .HasIndex(IDX_PERSONA_X_CONSEC, b => b.IndexBy(b => b.Consecutivo));


            databaseBuilder.Document<Persona>()
              .HasIndex(IDX_PERSONA_X_BUSCAR, c => c.IndexBy(c => c.Clientes)
              .ThenBy(f => f.GeneroId)
              .ThenBy(f => f.TicksFechaNacimiento)
              .ThenBy(f => f.NombreBusqueda)
              .ThenBy(f => f.PropiedadesFisicas.EtniaId)
              .ThenBy(f => f.PropiedadesFisicas.IMC)
              .ThenBy(f => f.PropiedadesFisicas.Altura)
              .ThenBy(f => f.PropiedadesFisicas.ColorOjosId)
              .ThenBy(f => f.PropiedadesFisicas.TipoCabelloId)
              .ThenBy(f => f.PropiedadesFisicas.ColorCabelloId));
        }

    }
}
