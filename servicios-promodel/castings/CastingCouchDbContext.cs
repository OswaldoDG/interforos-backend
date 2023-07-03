using CouchDB.Driver.Options;
using CouchDB.Driver;
using promodel.modelo.proyectos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace promodel.servicios.proyectos
{
    public class CastingCouchDbContext: CouchContext
    {
        public const string IDX_CLIENTE_FECHA_X_UID = "client_x_uid_fecha";
    
        public CastingCouchDbContext(CouchOptions<PersonasCouchDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(CouchOptionsBuilder optionsBuilder)
        {

        }

        public CouchDatabase<Casting> Castings { get; set; }


        protected override void OnDatabaseCreating(CouchDatabaseBuilder databaseBuilder)
        {
            databaseBuilder.Document<Casting>().ToDatabase("proyectos");

            databaseBuilder.Document<Casting>()
              .HasIndex(IDX_CLIENTE_FECHA_X_UID, b => b.IndexBy(b => b.ClienteId)
              .ThenBy(i=>i.ColaboradoresIds)
              .ThenBy(i => i.FechaCreacionTicks)
              .ThenBy(i => i.Activo)
              .ThenBy(i => i.Nombre));
        }
    }
}
