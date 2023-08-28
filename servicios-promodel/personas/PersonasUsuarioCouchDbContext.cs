using CouchDB.Driver;
using CouchDB.Driver.Options;
using promodel.modelo.perfil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace promodel.servicios.personas
{
    public class PersonasUsuarioCouchDbContext : CouchContext
    {
        public const string IDX_PERSONAS_X_UID_CID = "p_uid_cid";

        public PersonasUsuarioCouchDbContext(CouchOptions<PersonasUsuarioCouchDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(CouchOptionsBuilder optionsBuilder)
        {

        }

        public CouchDatabase<Persona> Personas { get; set; }


        protected override void OnDatabaseCreating(CouchDatabaseBuilder databaseBuilder)
        {
            databaseBuilder.Document<PersonasUsuario>().ToDatabase("personasusuario");

            databaseBuilder.Document<PersonasUsuario>()
              .HasIndex(IDX_PERSONAS_X_UID_CID, b => b.IndexBy(b => b.UsuarioId).ThenBy(b=>b.ClienteId));
        }
    }
}
