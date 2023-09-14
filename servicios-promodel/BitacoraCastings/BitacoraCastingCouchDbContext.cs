using CouchDB.Driver.Options;
using CouchDB.Driver;
using promodel.modelo;
using promodel.modelo.castings;

namespace promodel.servicios.BitacoraCastings;

public class BitacoraCastingCouchDbContext : CouchContext
{  

    public BitacoraCastingCouchDbContext(CouchOptions<CLientesCouchDbContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(CouchOptionsBuilder optionsBuilder)
    {
    }

    public CouchDatabase<HistorialCasting> Bitacoras { get; set; }
 

    protected override void OnDatabaseCreating(CouchDatabaseBuilder databaseBuilder)
    {
        databaseBuilder.Document<HistorialCasting>().ToDatabase("bitacoras");
    }

}