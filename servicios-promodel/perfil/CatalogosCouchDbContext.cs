using CouchDB.Driver;
using CouchDB.Driver.Options;
using promodel.modelo;
using promodel.modelo.perfil;

namespace promodel.servicios
{
    public class CatalogosCouchDbContext : CouchContext
    {
        public const string IDX_CLIENTE_TIPO = "cat_x_cli_tipo";

        public CatalogosCouchDbContext(CouchOptions<CatalogosCouchDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(CouchOptionsBuilder optionsBuilder)
        {
        }

        public CouchDatabase<CatalogoBase> Catalogos { get;     set; }

        protected override void OnDatabaseCreating(CouchDatabaseBuilder databaseBuilder)
        {
            databaseBuilder.Document<CatalogoBase>().ToDatabase("catalogos");

            databaseBuilder.Document<CatalogoBase>()
            .HasIndex(IDX_CLIENTE_TIPO, b => b.IndexBy(b => b.ClienteId).ThenBy(c=>c.TipoPropiedad));

            

        }

    }
}
