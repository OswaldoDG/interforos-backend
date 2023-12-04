using CouchDB.Driver;
using CouchDB.Driver.Options;

namespace promodel.servicios.webhooks;

public class GoogleDriveDbContext: CouchContext
{
    public GoogleDriveDbContext(CouchOptions<GoogleDriveDbContext> options) : base(options)
    {
    }
}
