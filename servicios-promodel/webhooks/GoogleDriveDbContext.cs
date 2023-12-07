using CouchDB.Driver;
using CouchDB.Driver.Options;
using promodel.modelo.webhooks;

namespace promodel.servicios.webhooks;

public class GoogleDriveDbContext: CouchContext
{
    public GoogleDriveDbContext(CouchOptions<GoogleDriveDbContext> options) : base(options)
    {
    }
    protected override void OnConfiguring(CouchOptionsBuilder optionsBuilder)
    {

    }
    public CouchDatabase<GoogleDrivePushNotification> Notificaciones { get; set; }


    protected override void OnDatabaseCreating(CouchDatabaseBuilder databaseBuilder)
    {
        databaseBuilder.Document<GoogleDrivePushNotification>().ToDatabase("notificaciones");
        databaseBuilder.Document<GoogleDrivePushNotification>().HasIndex("notificacion", b => b.IndexBy(b => b.Id));
    }
}
