using CouchDB.Driver.Options;
using CouchDB.Driver;
using promodel.modelo;
using promodel.modelo.media;

namespace promodel.servicios.media
{
    public class MediaCouchDbContext : CouchContext
    {
        public const string IDX_MEDIO_USUARIO = "medio_x_usuario";
        public const string IDX_MEDIO_CADUCIDAD = "medio_x_caducidad";

        public MediaCouchDbContext(CouchOptions<MediaCouchDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(CouchOptionsBuilder optionsBuilder)
        {
        }

        public CouchDatabase<MediaModelo> Medios { get; set; }
        

        protected override void OnDatabaseCreating(CouchDatabaseBuilder databaseBuilder)
        {
            databaseBuilder.Document<MediaModelo>().ToDatabase("medios");
        
            databaseBuilder.Document<MediaModelo>()
          .HasIndex(IDX_MEDIO_USUARIO, b => b.IndexBy(b => b.UsuarioId));

            databaseBuilder.Document<MediaModelo>()
          .HasIndex(IDX_MEDIO_CADUCIDAD, b => b.IndexBy(b => b.EliminacionTicks));

        }

    }
}
