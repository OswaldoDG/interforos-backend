using CouchDB.Driver.Extensions;
using CouchDB.Driver.Query.Extensions;
using Org.BouncyCastle.Asn1.Ess;
using promodel.modelo.castings;
using promodel.modelo.perfil;


namespace promodel.servicios.personas;

public class ServicioPersonasUsuario : IServicioPersonasUsuario
{
    private readonly PersonasUsuarioCouchDbContext db;
    private readonly IServicioPersonas servicioPersonas;

    public ServicioPersonasUsuario(PersonasUsuarioCouchDbContext db, IServicioPersonas servicioPersonas)
    {
        this.db = db;
        this.servicioPersonas = servicioPersonas;
    }

    public async Task<RespuestaPayload<PersonasUsuario>> AdicionaPersona(string personaId, string clienteId, string usuarioId, string? agenciaId = null)
    {
        var r = new RespuestaPayload<PersonasUsuario>();
        var personaUsuario = await PersonaUsuarioPorId(clienteId, usuarioId, agenciaId);
        if (personaUsuario==null)
        {
            var persona = new PersonasUsuario()
            { 
                Id = Guid.NewGuid().ToString(),
                UsuarioId = usuarioId,
                ClienteId = clienteId,
                AgenciaId = null,
                IdPersonas = new List<string> { personaId }
            };
            await db.Personas.AddOrUpdateAsync(persona);
            r.Payload = persona;
          
        }
        else
        {
            if (!personaUsuario.IdPersonas.Any(_=>_.Contains(personaId)))
            {
                personaUsuario.IdPersonas.Add(personaId);
                await db.Personas.AddOrUpdateAsync(personaUsuario);
                r.Payload = personaUsuario;
               
            }
        }
        // trae el registro en la base de datos que coincide  con los parámetros  
        // string clienteId, string usuarioId, string? agenciaId = null
        //
        // Si el registro NO EXISTE lo crea con ua nuva entidad de PersonasUsuario y 
        // en la lista IdPersonas añade el valor del parametro personaId
        // salva el nuevo registro en la base de datos
        //
        // Si el registro existe verifica si el valor del parametrot personaId esta en la lista IdPersonas
        // Si no esta lo añade y actualiza el registro .
        //
        // devuelve 200 en toso los casos
        r.Ok = true;
        return r;
    }

    public async Task<RespuestaPayload<List<MapaUsuarioNombre>>> ObtienePersonasRegistradas(string clienteId, string usuarioId, string? agenciaId = null)
    {
        var r = new RespuestaPayload<List<MapaUsuarioNombre>>();
        var modelos = new List<MapaUsuarioNombre>();
        var personaUsuario = await PersonaUsuarioPorId(clienteId, usuarioId, agenciaId);
        if (personaUsuario != null)
        {
            personaUsuario.IdPersonas.ForEach( p =>
            {
                var personaR =  servicioPersonas.PorId(p).Result;

                if (personaR.Ok)
                {
                    var persona = (Persona)personaR.Payload;
                    modelos.Add(new MapaUsuarioNombre() { Id = persona.Id, Nombre = persona.NombreArtistico, Email = null });
                }
            });
            r.Payload = modelos;
        }
        r.Ok = true;
        return r;
        // trae el registro en la base de datos que coincide  con los parámetros  
        // string clienteId, string usuarioId, string? agenciaId = null
        //
        // Si el registro NO EXISTE devuelve una lista vacia
        //
        // si el registro exiete devuelve el contenido de la propiedad IdPersonas


    }

    public async Task<Respuesta> RemuevePersona(string personaId, string clienteId, string usuarioId, string? agenciaId = null)
    {
        var r = new Respuesta();
        var personaUsuario = await PersonaUsuarioPorId(clienteId, usuarioId, agenciaId);
       
        if (personaUsuario!=null && personaUsuario.IdPersonas.Any(_ => _.Contains(personaId)))
        {
            personaUsuario.IdPersonas.Remove(personaId);
            await db.Personas.AddOrUpdateAsync(personaUsuario);
            r.Ok = true;
            return r;
        }
            r.HttpCode = HttpCode.NotFound;
            r.Error = "Persona no encontrado";
            return r;            

       
        // trae el registro en la base de datos que coincide  con los parámetros  
        // string clienteId, string usuarioId, string? agenciaId = null
        //
        // Si el registro NO EXISTE devuelve 404
        //
        // Si el registro existe verifica si el valor del parametrot personaId esta en la lista IdPersonas
        // si el valor EXISTE lo elimina de la lista y actualiza el registro

    }
    public async Task<PersonasUsuario> PersonaUsuarioPorId(string clienteId, string usuarioId, string? agenciaId = null)
    {
        //return await db.Personas.FirstOrDefaultAsync(_ => _.ClienteId == clienteId && _.UsuarioId == usuarioId);
        return await db.Personas.Where(_ => _.ClienteId == clienteId && _.UsuarioId == usuarioId).UseIndex(new[] { "design_document", PersonasUsuarioCouchDbContext.IDX_PERSONAS_X_UID_CID }).FirstOrDefaultAsync();

    }
}  
