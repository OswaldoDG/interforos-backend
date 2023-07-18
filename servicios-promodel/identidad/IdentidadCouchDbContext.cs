using CouchDB.Driver;
using CouchDB.Driver.Options;
using promodel.modelo;


namespace promodel.servicios;

public class IdentidadCouchDbContext : CouchContext
{
    public const string IDX_USUARIO_X_EMAIL = "usuario_x_email";
    public const string IDX_USUARIO_X_ID = "usuario_x_id";
    public const string IDX_USUARIO_X_NOMBRE = "usuario_x_nombre";
    public const string IDX_REFRESHTOKEN_X_CADUCIDAD = "refreshtoken_x_caducidad";
    public const string IDX_INVITACION = "invitacion";
    public const string IDX_USUARIO_CLIENTES = "usuario_clientes";

    public IdentidadCouchDbContext(CouchOptions<CLientesCouchDbContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(CouchOptionsBuilder optionsBuilder)
    {
    }

    public CouchDatabase<InvitacionRegistro> Invitaciones { get; set; }
    public CouchDatabase<Usuario> Usuarios { get; set; }

    public CouchDatabase<RefreshToken> RefreshTokens { get; set; }

    protected override void OnDatabaseCreating(CouchDatabaseBuilder databaseBuilder)
    {
        databaseBuilder.Document<InvitacionRegistro>().ToDatabase("invitaciones");
        databaseBuilder.Document<Usuario>().ToDatabase("usuarios");
        databaseBuilder.Document<RefreshToken>().ToDatabase("refreshtokens");

        databaseBuilder.Document<Usuario>()
      .HasIndex("usuario_x_email", b => b.IndexBy(b => b.Email));

        databaseBuilder.Document<Usuario>()
      .HasIndex("usuario_clientes", b => b.IndexBy(b => b.Clientes));

        databaseBuilder.Document<Usuario>()
      .HasIndex("usuario_x_nombre", b => b.IndexBy(b => b.NombreAcceso));

        databaseBuilder.Document<RefreshToken>()
      .HasIndex("refreshtoken_x_caducidad", b => b.IndexBy(b => b.Caducidad));

        databaseBuilder.Document<InvitacionRegistro>()
      .HasIndex("invitacion", b => b.IndexBy(b => b.Registro.Email).ThenBy(c => c.Registro.ClienteId));

    }

}
