using CouchDB.Driver.Options;
using CouchDB.Driver;
using promodel.modelo.registro;

namespace promodel.servicios.SolicitudSoporte;

public class SolicitudSoporteCouchDbContext : CouchContext
{
    public const string IDX_SOLICITUD = "solicitud";
    public SolicitudSoporteCouchDbContext(CouchOptions<CLientesCouchDbContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(CouchOptionsBuilder optionsBuilder)
    {

    }

    public CouchDatabase<SolicitudSoporteUsuario> SoporteUsuario { get; set; }


    protected override void OnDatabaseCreating(CouchDatabaseBuilder databaseBuilder)
    {
        databaseBuilder.Document<SolicitudSoporteUsuario>().ToDatabase("solicitudes");
        databaseBuilder.Document<SolicitudSoporteUsuario>().HasIndex("solicitud", b => b.IndexBy(b => b.Id));
    }

}