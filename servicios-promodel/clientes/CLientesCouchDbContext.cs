using CouchDB.Driver;
using CouchDB.Driver.Options;
using promodel.modelo.clientes;

namespace promodel.servicios
{
    public class CLientesCouchDbContext : CouchContext
    {
        public const string IDX_CLIENTE_X_URL = "cliente_x_url";
        public CLientesCouchDbContext(CouchOptions<CLientesCouchDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(CouchOptionsBuilder optionsBuilder)
        {

        }

        public CouchDatabase<Cliente> Clientes { get; set; }


        protected override void OnDatabaseCreating(CouchDatabaseBuilder databaseBuilder)
        {
            databaseBuilder.Document<Cliente>().ToDatabase("clientes");

            databaseBuilder.Document<Cliente>()
              .HasIndex("cliente_x_url", b => b.IndexBy(b => b.Url));
        }

    }
}
