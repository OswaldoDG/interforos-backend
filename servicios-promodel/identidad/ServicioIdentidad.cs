using comunicaciones.email;
using CouchDB.Driver.Extensions;
using CouchDB.Driver.Query.Extensions;
using Flurl.Util;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using promodel.modelo;
using promodel.modelo.clientes;
using promodel.modelo.perfil;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace promodel.servicios;

public partial class ServicioIdentidad: IServicioIdentidad
{


    private readonly IdentidadCouchDbContext db;
    private readonly IDistributedCache cache;
    private readonly IConfiguration configuration;
    private readonly IServicioEmail servicioEmail;
    private readonly IWebHostEnvironment environment;
    private readonly IServicioClientes servicioClientes;

    public ServicioIdentidad(IdentidadCouchDbContext db, IDistributedCache cache, 
        IConfiguration configuration, IServicioEmail servicioEmail, 
        IWebHostEnvironment environment, IServicioClientes servicioClientes)
    {
        this.db = db;
        this.cache = cache;
        this.configuration = configuration;
        this.servicioEmail = servicioEmail;
        this.environment = environment;
        this.servicioClientes = servicioClientes;
    }

    public async Task<RespuestaLogin?> RefreshToken(string RefreshToken, string UsuarioId, string clienteId)
    {
        await EliminaTokensExcedidos();
        string tokenId = ExtensionesUsuario.Base64Decode(RefreshToken);
        Usuario? u = await db.Usuarios.Where(x => x.Id==UsuarioId)
            .UseIndex(new[] { "design_document", IdentidadCouchDbContext.IDX_USUARIO_X_ID }).FirstOrDefaultAsync();
        var roles = u.RolesCliente.Where(_ => _.ClienteId == clienteId).ToList();
        RefreshToken? r = await RefreshTokenPorId(tokenId);
        if (r != null)
        {
            if (r.Caducidad > DateTime.UtcNow)
            {

                DateTime tokenexpiration = DateTime.UtcNow.AddMinutes(configuration.GetValue<int>("Jwt:ttl_minutos"));
                var issuer = configuration["Jwt:Issuer"];
                var audience = configuration["Jwt:Audience"];
                var key = Encoding.UTF8.GetBytes(configuration["Jwt:Key"]);
                var signingCredentials = new SigningCredentials(
                                        new SymmetricSecurityKey(key),
                                        SecurityAlgorithms.HmacSha512Signature
                                    );
                var subject = new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, UsuarioId)
                });

                if (roles.Any())
                {
                    roles.ForEach(rol =>
                    {
                        subject.AddClaim(new Claim("role", rol.Rol.ToString().ToLower()));

                    });
                }

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = subject,
                    Expires = tokenexpiration,
                    Issuer = issuer,
                    Audience = audience,
                    SigningCredentials = signingCredentials
                };
                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var jwtToken = tokenHandler.WriteToken(token);

                await EliminaRefreshTokenPorId(tokenId);
                var rt = await CreaRefreshToken();
                RespuestaLogin refeshed = new() { RefreshToken = ExtensionesUsuario.Base64Encode(rt.Id), Token = jwtToken, UTCExpiration = tokenexpiration };

                return refeshed;
            }
        }

        return null;
    }

    public async Task<RespuestaLogin?> Login(string usuario, string contrasena,string clienteId)
    {
       
        Usuario? u = await db.Usuarios.Where(x => x.NombreAcceso == usuario.ToLower())
            .UseIndex(new[] { "design_document", IdentidadCouchDbContext.IDX_USUARIO_X_NOMBRE }).FirstOrDefaultAsync();
        if (u != null)
        {
            var roles = u.RolesCliente.Where(_=>_.ClienteId==clienteId).ToList();

            if (SecretHasher.Verify(contrasena, u.HashContrasena))
            {

                DateTime tokenexpiration = DateTime.UtcNow.AddMinutes(configuration.GetValue<int>("Jwt:ttl_minutos"));
                var issuer = configuration["Jwt:Issuer"];
                var audience = configuration["Jwt:Audience"];
                var key = Encoding.UTF8.GetBytes(configuration["Jwt:Key"]);
                var signingCredentials = new SigningCredentials(
                                        new SymmetricSecurityKey(key),
                                        SecurityAlgorithms.HmacSha512Signature
                                    );
                var subject = new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, u.Id)

                });

                if (roles.Any())
                {
                    roles.ForEach(rol =>
                    {
                       subject.AddClaim(new Claim("role",rol.Rol.ToString().ToLower()));

                    });
                }

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = subject,
                    Expires = tokenexpiration,
                    Issuer = issuer,
                    Audience = audience,
                    SigningCredentials = signingCredentials
                };
                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var jwtToken = tokenHandler.WriteToken(token);
                var rt = await CreaRefreshToken();
                RespuestaLogin r = new () {  RefreshToken = ExtensionesUsuario.Base64Encode(rt.Id) , Token = jwtToken , UTCExpiration = tokenexpiration };

                return r;
            }
        }
        return null;
    }

    private async Task EliminaTokensExcedidos()
    {
        var tokens = await db.RefreshTokens.Where(t => DateTime.UtcNow >t.Caducidad)
           .UseIndex(new[] { "design_document", IdentidadCouchDbContext.IDX_REFRESHTOKEN_X_CADUCIDAD }).ToListAsync();
        if(tokens.Count> 0)
        {
            await db.RefreshTokens.DeleteRangeAsync(tokens);
        }
    }

    private async Task<RefreshToken> CreaRefreshToken()
    {
        DateTime tokenexpiration = DateTime.UtcNow.AddMinutes(configuration.GetValue<int>("Jwt:ttl_refresh_minutos"));
        RefreshToken t = new() { Id = Guid.NewGuid().ToString(), Caducidad = tokenexpiration };
        await db.RefreshTokens.AddAsync(t);
        return t;
    }

    private async Task<RefreshToken?> RefreshTokenPorId(string Id)
    {
        return await db.RefreshTokens.FirstOrDefaultAsync(x => x.Id == Id);
    }

    private async Task EliminaRefreshTokenPorId(string Id)
    {
        var t=  await db.RefreshTokens.FirstOrDefaultAsync(x => x.Id == Id);
        if (t != null)
        {
            await db.RefreshTokens.RemoveAsync(t);
        }
    }

    public async Task<Usuario?> UsuarioPorEmail(string Email)
    {
        Usuario? usuario = await db.Usuarios.Where(x=>x.Email == Email.ToLower() )
            .UseIndex(new[] { "design_document", IdentidadCouchDbContext.IDX_USUARIO_X_EMAIL }).FirstOrDefaultAsync();
        return usuario;
    }
    public async Task<Usuario?> UsuarioPorId(string id)
    {
        Usuario? usuario = await db.Usuarios.Where(x => x.Id == id)
            .UseIndex(new[] { "design_document", IdentidadCouchDbContext.IDX_USUARIO_X_ID}).FirstOrDefaultAsync();
        return usuario;
    }

    public async  Task<Usuario?> ActualizaUsuario(Usuario usuario)
    {
        if(db.Usuarios.Any(x => x.Id == usuario.Id))
        {
            await db.Usuarios.AddOrUpdateAsync(usuario);
            return usuario;
        }
        return null;
    }

    public async Task<Usuario> CreaUsuario(Usuario usuario)
    {
       await db.Usuarios.AddOrUpdateAsync(usuario);
       return usuario;
    }

    public async Task<Respuesta> RestablecerPassword(string UsuarioId, string ContrasenaNueva)
    {
        var res = new Respuesta();

        var usuario = await UsuarioPorId(UsuarioId);

        if (usuario == null)
        {
            res.Error = "El usuario no existe";
            res.HttpCode = HttpCode.NotFound;
            return res;
        }

        usuario.HashContrasena = SecretHasher.Hash(ContrasenaNueva);
        await ActualizaUsuario(usuario);
        res.Ok = true;
        return res;
    }
    public async Task<Respuesta> CambiarPassword(string UsuarioId, string ContrasenaActual, string ContrasenaNueva)
    {
        var res = new Respuesta();

        var usuario = await UsuarioPorId(UsuarioId);

        if(usuario!=null && SecretHasher.Verify(ContrasenaActual, usuario.HashContrasena))
        {
            usuario.HashContrasena = SecretHasher.Hash(ContrasenaNueva);
            await ActualizaUsuario(usuario);
            res.Ok = true;
            return res;
        }
        else
        {
            res.Error = "E0";
            res.HttpCode = HttpCode.BadRequest;
            return res;

        }

    }
}
