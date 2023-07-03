using CouchDB.Driver.Types;

namespace promodel.modelo
{
    public class RefreshToken: CouchDocument
    {

        public DateTime Caducidad { get; set; }

    }
}
