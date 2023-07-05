using almacenamiento;
using almacenamiento.GoogleDrive;
using api_promodel.middlewares;
using comunicaciones.email;
using CouchDB.Driver.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using promodel.servicios;
using promodel.servicios.almacenamiento;
using promodel.servicios.castings.Mock;
using promodel.servicios.media;
using promodel.servicios.perfil;
using promodel.servicios.proyectos;
using Serilog;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

//builder.Host.UseSerilog((HostBuilderContext ctx, LoggerConfiguration loggerConfiguration) =>
//    {
//        loggerConfiguration.ReadFrom.Configuration(ctx.Configuration);
//    });



builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(p => p.AddPolicy("corsapp", builder =>
{
    builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(x =>
    {
        x.SaveToken = true;
        x.RequireHttpsMetadata = false;

        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"])),
            ValidIssuer = configuration["Jwt:Issuer"],
            ValidAudience = configuration["Jwt:Audience"],
            ClockSkew = TimeSpan.FromMinutes(5)
        };
    });



builder.Services.AddCouchContext<IdentidadCouchDbContext>(builder => builder
    .EnsureDatabaseExists()
    .UseEndpoint(configuration.GetValue<string>("promodeldrivers:couchdb:endpoint"))
    .UseBasicAuthentication(username: configuration.GetValue<string>("promodeldrivers:couchdb:username"),
    password: configuration.GetValue<string>("promodeldrivers:couchdb:password")));

builder.Services.AddCouchContext<CLientesCouchDbContext>(builder => builder
    .EnsureDatabaseExists()
    .UseEndpoint(configuration.GetValue<string>("promodeldrivers:couchdb:endpoint"))
    .UseBasicAuthentication(username: configuration.GetValue<string>("promodeldrivers:couchdb:username"),
    password: configuration.GetValue<string>("promodeldrivers:couchdb:password")));

builder.Services.AddCouchContext<PersonasCouchDbContext>(builder => builder
    .EnsureDatabaseExists()
    .UseEndpoint(configuration.GetValue<string>("promodeldrivers:couchdb:endpoint"))
    .UseBasicAuthentication(username: configuration.GetValue<string>("promodeldrivers:couchdb:username"),
    password: configuration.GetValue<string>("promodeldrivers:couchdb:password")));

builder.Services.AddCouchContext<CatalogosCouchDbContext>(builder => builder
    .EnsureDatabaseExists()
    .UseEndpoint(configuration.GetValue<string>("promodeldrivers:couchdb:endpoint"))
    .UseBasicAuthentication(username: configuration.GetValue<string>("promodeldrivers:couchdb:username"),
    password: configuration.GetValue<string>("promodeldrivers:couchdb:password")));

builder.Services.AddCouchContext<MediaCouchDbContext>(builder => builder
    .EnsureDatabaseExists()
    .UseEndpoint(configuration.GetValue<string>("promodeldrivers:couchdb:endpoint"))
    .UseBasicAuthentication(username: configuration.GetValue<string>("promodeldrivers:couchdb:username"),
    password: configuration.GetValue<string>("promodeldrivers:couchdb:password")));

builder.Services.AddCouchContext<CastingCouchDbContext>(builder => builder
    .EnsureDatabaseExists()
    .UseEndpoint(configuration.GetValue<string>("promodeldrivers:couchdb:endpoint"))
    .UseBasicAuthentication(username: configuration.GetValue<string>("promodeldrivers:couchdb:username"),
    password: configuration.GetValue<string>("promodeldrivers:couchdb:password")));

builder.Services.Configure<SMTPConfig>(builder.Configuration.GetSection("SMTPConfig"));
builder.Services.Configure<CacheAlmacenamientoLocalConfig>(builder.Configuration.GetSection("CacheAlmacenamientoLocalConfig"));

builder.Services.AddScoped<ControladorAutenticadoFilter>();
builder.Services.AddTransient<ICastingService, CastingService>();
builder.Services.AddTransient<IServicioClientes, ServicioClientes>();
builder.Services.AddTransient<IServicioIdentidad, ServicioIdentidad>();
builder.Services.AddTransient<IServicioPersonas, ServicioPersonas>();
builder.Services.AddTransient<IServicioCatalogos, ServicioCatalogos>();
builder.Services.AddTransient<IServicioEmail, ServicioEmailSendGrid>();
builder.Services.AddTransient<IMessageBuilder, JSONMessageBuilder>();
builder.Services.AddTransient<IGoogleDriveConfigProvider, PromodelGDriveProvider>();
builder.Services.AddTransient<IAlmacenamiento, GoogleDriveDriver>();
builder.Services.AddTransient<ICacheAlmacenamiento, CacheAlmacenamientoLocal>();
builder.Services.AddTransient<IMedia, MediaService>();
builder.Services.AddTransient<IBogusService, BogusService>();

// Default Policy for dynamic policy provider
builder.Services.AddCors(options =>
{
    options.AddPolicy(options.DefaultPolicyName, policy =>
    {
        policy.AllowAnyHeader()
        .AllowCredentials()
        .AllowAnyMethod();
    });
});

builder.Services.AddDistributedMemoryCache();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("corsapp");

app.UseHostDetector();

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.UseFileServer(new FileServerOptions
{
    
    FileProvider = new PhysicalFileProvider(configuration["CacheDir"]),
    RequestPath = "/videos",
    EnableDirectoryBrowsing = true
});


app.MapControllers();

app.Run();
