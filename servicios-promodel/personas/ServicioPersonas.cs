using almacenamiento;
using Amazon.Auth.AccessControlPolicy;
using Bogus;
using CouchDB.Driver.Extensions;
using CouchDB.Driver.Query.Extensions;
using CouchDB.Driver.Views;
using MailKit.Search;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using promodel.modelo;
using promodel.modelo.castings;
using promodel.modelo.clientes;
using promodel.modelo.perfil;
using promodel.modelo.proyectos;
using promodel.servicios.comunes;
using promodel.servicios.perfil;
using promodel.servicios.personas;
using promodel.servicios.proyectos;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.PortableExecutable;

namespace promodel.servicios
{
    public class ServicioPersonas: IServicioPersonas
    {
        private readonly PersonasCouchDbContext db;
        private readonly IdentidadCouchDbContext dbidentity;
        private readonly IDistributedCache cache;
        private readonly IConfiguration configuration;
        private readonly IAlmacenamiento almacenamiento;
        private readonly IServicioCatalogos servicioCatalogos;
        private readonly IServicioClientes servicioClientes;
        public ServicioPersonas(PersonasCouchDbContext db,
            IServicioClientes servicioClientes,
            IServicioCatalogos servicioCatalogos,
            IdentidadCouchDbContext dbidentity, IDistributedCache cache, IConfiguration configuration, IAlmacenamiento almacenamiento)
        {
            this.db = db;
            this.dbidentity = dbidentity;
            this.cache = cache;
            this.configuration = configuration;
            this.almacenamiento = almacenamiento;
            this.servicioCatalogos = servicioCatalogos;
            this.servicioClientes = servicioClientes;
            // Demo();
        }

        public async Task<bool> EliminarLinkDocumento(string CLienteId, string UsuarioId, string DocumentoId)
        {
            var p = await db.Personas.Where(x => x.UsuarioId == UsuarioId).FirstOrDefaultAsync();
            if (p != null)
            {
                var link = p.Documentos.FirstOrDefault(d => d.Id == DocumentoId);
                if (link != null)
                {
                    p.Documentos.Remove(link);
                }

                p.DocumentacionCompleta = await DocumentacionCompleta(p, CLienteId);

                await db.Personas.AddOrUpdateAsync(p);
                return true;
            }
            return false;
        }

        private async Task<List<DocumentoModelo>> DocumentosCLiente(string ClienteId) {
            var c = await servicioClientes.ClientePorId(ClienteId);
            if(c!=null)
            {
                return c.Documentacion;
            }
            return new List<DocumentoModelo>();
        }

        private async Task<bool> DocumentacionCompleta (Persona p, string clienteId)
        {
            var documentos = await DocumentosCLiente(clienteId);
            
            foreach(var d in documentos.Where(x=>x.Obligatorio == true).ToList())
            {
                if(!p.Documentos.Any(x=>x.Id == d.Id))
                {
                    return false;
                }
            }

            return true;
        }

        public async Task<bool> UpsertLinkDocumento(string CLienteId, string UsuarioId, string DocumentoId, string AlmacenamientoId)
        {
            var p = await db.Personas.Where(x => x.UsuarioId == UsuarioId).FirstOrDefaultAsync();
            if (p != null)
            {
                var link = p.Documentos.FirstOrDefault(d => d.Id == DocumentoId);
                if(link!=null)
                {
                    link.IdAlmacenamiento = AlmacenamientoId;
                } else
                {
                    p.Documentos.Add(new Documento() { Id = DocumentoId, IdAlmacenamiento = AlmacenamientoId });
                }

                p.DocumentacionCompleta = await DocumentacionCompleta(p, CLienteId);

                await db.Personas.AddOrUpdateAsync(p);
                return true;
            }
            return false;
        }

        public async Task<bool> EstableceFotoPrincipal(string UsuarioId, string? ElementoId)
        {
            var p = await db.Personas.Where(x => x.UsuarioId == UsuarioId).FirstOrDefaultAsync();
            if (p != null)
            {
                p.ElementoMedioPrincipalId = ElementoId;
                await db.Personas.AddOrUpdateAsync(p);
                return true;
            }
            return false;
        }

        public async Task<RespuestaPayload<Persona>> Actualizar(Persona persona)
        {
            persona.NombreBusqueda = NombreBusuqedaPersona(persona);
            RespuestaPayload<Persona> r = new();
            var p = await db.Personas.FindAsync(persona.Id);
            if (p != null)
            {
                foreach(string clId in persona.Clientes)
                {
                    if(!p.Clientes.Any(x=>x.Equals(clId, StringComparison.CurrentCultureIgnoreCase)))
                    {
                        p.Clientes.Add(clId);
                    }
                }

                foreach (string cliente in p.Clientes)
                {
                    await validaIds(persona, cliente);

                }

                if (persona.FechaNacimiento.HasValue)
                {
                    persona.TicksFechaNacimiento = persona.FechaNacimiento.Value.Ticks;
                }
                
                if (persona.PropiedadesFisicas != null)
                {
                    persona.PropiedadesFisicas.IMC = IMC(persona);
                }


                if(!p.Consecutivo.HasValue)
                {
                    persona.Consecutivo = this.SiguienteId();
                } else
                {
                    persona.Consecutivo = p.Consecutivo;
                }

                persona.FolderContenidoId = p.FolderContenidoId;
                persona.Clientes = p.Clientes;
                persona.Rev = p.Rev;
                persona.ElementoMedioPrincipalId = p.ElementoMedioPrincipalId;
                persona.Documentos = p.Documentos;
                persona.DocumentacionCompleta = p.DocumentacionCompleta;
                await db.Personas.AddOrUpdateAsync(persona);

                r = r.OK(persona);
            }
            else
            {
                r.HttpCode = HttpCode.NotFound;
            }
            return r;
        }

        public static decimal IMC(Persona persona)
        {
            decimal imc = 0;
            if (persona.PropiedadesFisicas?.Altura != null &&
                persona.PropiedadesFisicas?.Peso != null)
            {

                if (persona.PropiedadesFisicas.MKS)
                {
                    double peso = (double)persona.PropiedadesFisicas.Peso;
                    double altura2 = Math.Pow((double)persona.PropiedadesFisicas.Altura, (double)2);
                    imc = (decimal)(peso / altura2);
                }
                else
                {
                    double peso = (double)persona.PropiedadesFisicas.Peso;
                    string hs = persona.PropiedadesFisicas.Altura.ToString();
                    int pies = int.Parse(hs.Split(',')[0]);
                    int pulgs = int.Parse(hs.Split(',')[1]);
                    double altura2 = Math.Pow((double)((pies * 12) + pulgs), (double)2);
                    imc = (decimal)(peso / altura2) * 703;
                }
            }

            return imc;
        }

        public static string NombreBusuqedaPersona(Persona persona)
        {
            return $"{persona.Nombre}{persona.Apellido1}{persona.Apellido2}{persona.NombreArtistico}".ToUpper();
        }

        public async void Demo()
        {
           throw new Exception("No Llamar");
            var ps = PersonasDemo.CreaPersonas(1000);
            await db.Personas.AddOrUpdateRangeAsync(ps);
        }

        private int SiguienteId()
        {
            try
            {
                var c = db.Personas.Max(p => p.Consecutivo);
                if(c.HasValue)
                {
                    return c.Value + 1;
                }
            }
            catch (Exception)
            {

                
            }

            return 1;
        }

        private async Task validaIds(Persona persona, string clienteId)
        {
            if (!string.IsNullOrEmpty(persona.OtroAgencia))
            {
                var item = await this.servicioCatalogos.BuscaCrea(clienteId, Perfil.CAT_AGENCIAS, persona.OtroAgencia);
                if (item != null)
                {
                    persona.AgenciasIds.Add(item.Clave);
                }
            }

            if (!string.IsNullOrEmpty(persona.OtroColorCabello) )
            {
                var item = await this.servicioCatalogos.BuscaCrea(clienteId, Perfil.CAT_COLOR_CABELLO, persona.OtroColorCabello);
                if(item!=null)
                {
                    persona.PropiedadesFisicas.ColorCabelloId = item.Clave;
                }
            }

            if (!string.IsNullOrEmpty(persona.OtroColorOjos))
            {
                var item = await this.servicioCatalogos.BuscaCrea(clienteId, Perfil.CAT_COLOR_OJOS, persona.OtroColorOjos);
                if (item != null)
                {
                    persona.PropiedadesFisicas.ColorOjosId = item.Clave;
                }
            }

            if (!string.IsNullOrEmpty(persona.OtroGrupoRacial))
            {
                var item = await this.servicioCatalogos.BuscaCrea(clienteId, Perfil.CAT_ETNIA, persona.OtroGrupoRacial);
                if (item != null)
                {
                    persona.PropiedadesFisicas.EtniaId = item.Clave;
                }
            }

            if (!string.IsNullOrEmpty(persona.OtroTipoCabello))
            {
                var item = await this.servicioCatalogos.BuscaCrea(clienteId, Perfil.CAT_TIPO_CABELLO, persona.OtroTipoCabello);
                if (item != null)
                {
                    persona.PropiedadesFisicas.TipoCabelloId = item.Clave;
                }
            }

            if (!string.IsNullOrEmpty(persona.OtroIdioma))
            {
                persona.IdiomasIds ??= new List<string>();

                var item = await this.servicioCatalogos.BuscaCrea(clienteId, Perfil.CAT_IDIOMA, persona.OtroIdioma);
                if (item != null)
                {
                    persona.IdiomasIds.Add(item.Clave);
                }
            }

            if (!string.IsNullOrEmpty(persona.OtroHabilidad))
            {
                persona.ActividadesIds ??= new List<string>();

                var item = await this.servicioCatalogos.BuscaCrea(clienteId, Perfil.CAT_ACTIVIDADES, persona.OtroHabilidad);
                if (item != null)
                {
                    persona.ActividadesIds.Add(item.Clave);
                }
            }

        }

        public async Task<RespuestaPayload<Persona>> Crear(Persona persona)
        {

            foreach(string cliente in persona.Clientes)
            {
                await validaIds(persona, cliente);

            }

            persona.NombreBusqueda = NombreBusuqedaPersona(persona);
          
            if (persona.PropiedadesFisicas!=null)
            {
                persona.PropiedadesFisicas.IMC = IMC(persona);
            }

            RespuestaPayload<Persona> r = new();
            var p = await db.Personas.Where(x=>x.UsuarioId == persona.UsuarioId).FirstOrDefaultAsync();
            if (p != null)
            {
                if (persona.FechaNacimiento.HasValue)
                {
                    persona.TicksFechaNacimiento = persona.FechaNacimiento.Value.Ticks;
                }
                persona.Rev = p.Rev;
                persona.Id = p.Id;
                persona.Consecutivo = p.Consecutivo.HasValue ? p.Consecutivo : SiguienteId();
                await db.Personas.AddOrUpdateAsync(persona);
                
            } else
            {
                persona.Consecutivo = this.SiguienteId();
                if (persona.FechaNacimiento.HasValue)
                {
                    persona.TicksFechaNacimiento = persona.FechaNacimiento.Value.Ticks;
                }
                persona.Castings = new List<CastingPersona>();
                persona.Id = Guid.NewGuid().ToString();
                await db.Personas.AddAsync(persona);
            }
            
            r = r.OK(persona);
            return r;
        }

        public async Task<RespuestaPayload<Persona>> CrearPersonaNew(Persona persona,string UsuarioId)
        {

            foreach (string cliente in persona.Clientes)
            {
                await validaIds(persona, cliente);

            }

            persona.NombreBusqueda = NombreBusuqedaPersona(persona);

            if (persona.PropiedadesFisicas != null)
            {
                persona.PropiedadesFisicas.IMC = IMC(persona);
            }

            RespuestaPayload<Persona> r = new();
            persona.Consecutivo = this.SiguienteId();
            if (persona.FechaNacimiento.HasValue)
            {
                persona.TicksFechaNacimiento = persona.FechaNacimiento.Value.Ticks;
            }
            persona.Id = Guid.NewGuid().ToString();
            persona.UsuarioRegistroId = UsuarioId;
            persona.FechaRegistro = DateTime.UtcNow.Ticks;
            await db.Personas.AddAsync(persona);

            r = r.OK(persona);
            return r;
        }
        public async Task<Respuesta> Elmiminar(string Id)
        {
            Respuesta r = new ();
            var p = await db.Personas.FindAsync(Id);
            if (p != null)
            {
                await db.Personas.RemoveAsync(p);
                r = r.OK();

            } else
            {
                r.HttpCode = HttpCode.NotFound;
            }
            return r;
        }

        public async Task<InformacionPerfil?> PerfilCliente(string UsuarioId, string ClienteId)
        {
            var usuario = await dbidentity.Usuarios.Where(x => x.Id == UsuarioId).FirstOrDefaultAsync();
            if(usuario!=null)
            {
                if(usuario.RolesCliente.Any(x=>x.ClienteId == ClienteId))
                {
                    var u = await db.Personas.Where(x => x.UsuarioId == UsuarioId).FirstOrDefaultAsync();
                    InformacionPerfil info = new InformacionPerfil()
                    {
                        AvatarBase64 = null,
                        Alias = usuario.Email,
                        UsuarioId = usuario.Id,
                        NombreCompleto = usuario.NombreUsuario,
                        TienePerfil = false,
                        RequirePerfil = usuario.RolesCliente.Any(x => x.ClienteId == ClienteId && x.Rol == TipoRolCliente.Modelo),
                        Roles = usuario.RolesCliente.Where(x=>x.ClienteId == ClienteId).Select(r=>r.Rol).ToList()
                    };

                    if (u != null)
                    {
                        info.TienePerfil = true;
                        info.AvatarBase64 = u.AvatarBase64;
                        info.NombreCompleto = u.Nombre;
                        info.Alias = u.NombreArtistico;
                        info.CosentimientosAceptados = usuario.AceptacionConsentimientos;
                    }
                    return info;
                }
            }

            return null;
        }

        public async Task<RespuestaPayload<Persona>> PorId(string Id)
        {
            RespuestaPayload<Persona> r = new();
            var p = await db.Personas.FindAsync(Id);
            if(p != null)
            {
                r = r.OK(p);
            } else
            {
                r.HttpCode = HttpCode.NotFound; 
            }

            return r;
        }

        public async Task<RespuestaPayload<Persona>> PorUsuarioId(string Id)
        {
            RespuestaPayload<Persona> r = new();
            var p = await db.Personas.Where(p=>p.UsuarioId == Id).FirstOrDefaultAsync();
            if (p != null)
            {
                r.Payload = p;
                r.Ok = true;
            }
            else
            {
                r.HttpCode = HttpCode.NotFound;
            }

            return r;
        }

        public async Task<ResponsePaginado<Persona>> BuscarPersonas([FromBody] RequestPaginado<BusquedaPersonas> busqueda)
        {
            try
            {
                List<Expression<Func<Persona, bool>>> expressions = new List<Expression<Func<Persona, bool>>>();
                Expression<Func<Persona, bool>>? ex;

                expressions.Add(x => x.Clientes.Contains(busqueda.Request.ClienteId));

                ex = PorEdad(busqueda.Request.EdadMinima, busqueda.Request.EdadMaxima);
                if (ex != null)
                {
                    expressions.Add(ex);
                }

                ex = PorGenero(busqueda.Request.GenerosId);
                if (ex != null)
                {
                    expressions.Add(ex);
                }

                ex = PorTipoCuerpo(busqueda.Request.TipoCuerpos);
                if (ex != null)
                {
                    expressions.Add(ex);
                }

                ex = PorTipoNombre(busqueda.Request.Nombre);
                if (ex != null)
                {
                    expressions.Add(ex);
                }

                ex = PorEtnias(busqueda.Request.EtniasIds);
                if (ex != null)
                {
                    expressions.Add(ex);
                }

                ex = PorTipoCabello(busqueda.Request.TipoCabelloIds);
                if (ex != null)
                {
                    expressions.Add(ex);
                }

                ex = PorColorCabello(busqueda.Request.ColorCabelloIds);
                if (ex != null)
                {
                    expressions.Add(ex);
                }

                ex = PorIdiomas(busqueda.Request.IdiomasIds);
                if (ex != null)
                {
                    expressions.Add(ex);
                }

                ex = PorColorOjos(busqueda.Request.ColorOjosIds);
                if (ex != null)
                {
                    expressions.Add(ex);
                }

                ex = PorHabilidades(busqueda.Request.HabilidadesIds);
                if (ex != null)
                {
                    expressions.Add(ex);
                }


                Func<Persona, object> ordenar = null;
                string[] orden = null;
                if (busqueda.OrdernarASC != null)
                {

                    switch (busqueda.OrdenarPor)
                    {
                        case "consecutivo":
                            ordenar = OrdenarPorConsecutivo();                           
                            orden = new[] { "design_document", PersonasCouchDbContext.IDX_PERSONA_X_CONSEC };                        
                            break;
                        case "NombreArtistico":
                            ordenar = OrdenarPorNombreArtistico();
                            orden = new[] { "design_document", PersonasCouchDbContext.IDX_PERSONA_X_NOMBREARTISTICO};
                            break;
                        case "edad":
                            ordenar =OrdenarPorEdad();
                            orden = new[] { "design_document", PersonasCouchDbContext.IDX_PERSONA_X_EDAD };
                            break;
                        default:
                            ordenar = OrdenarPorNombre();
                            orden = new[] { "design_document", PersonasCouchDbContext.IDX_PERSONA_X_NOMBRE };
                            break;
                    }
                }
             

                int total = 0;
                List<Persona> personas = new List<Persona>();
                if (expressions.Count > 0)
                {
                    var expresion = expressions[0];

                    for (int i = 1; i < expressions.Count; i++)
                    {
                        expresion = expresion.And(expressions[i]);
                    }

                    if (busqueda.OrdernarASC != null && busqueda.OrdernarASC ==true)
                    {
                        var todos = db.Personas.Where(expresion).UseIndex(orden).OrderBy(ordenar).ToList();
                        total = todos.Count();
                        personas = todos.Skip((busqueda.Pagina - 1) * busqueda.Tamano).Take(busqueda.Tamano).ToList();
                    }
                    else
                    {
                        var todos = db.Personas.Where(expresion).UseIndex(orden).OrderByDescending(ordenar).ToList();
                        total = todos.Count();
                        personas = todos.Skip((busqueda.Pagina - 1) * busqueda.Tamano).Take(busqueda.Tamano).ToList();

                    }
                }


                return new ResponsePaginado<Persona>() { 
                    Elementos = personas,
                    Pagina = busqueda.Pagina, 
                    Tamano = busqueda.Tamano, 
                    Total = total };

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        private Func<Persona, object> OrdenarPorNombre()
        {
            return p => p.Nombre;
        }
        private Func<Persona, object> OrdenarPorNombreArtistico()
        {
            return p => p.NombreArtistico;
        }
        private Func<Persona, object> OrdenarPorConsecutivo()
        {
            return p => p.Consecutivo;
        }
        private Func<Persona, object> OrdenarPorEdad()
        {
            return p => p.Edad;
        }

        public async Task<ResponsePaginado<Persona>> BuscarPersonasId( RequestPaginado<BusquedaPersonasId> busqueda)
        {
            try
            {
                Func<Persona, object> ordenar = null;
                string[] orden = null;
                if (busqueda.OrdernarASC != null)
                {

                    switch (busqueda.OrdenarPor)
                    {
                        case "consecutivo":
                            ordenar = OrdenarPorConsecutivo();
                            orden = new[] { "design_document", PersonasCouchDbContext.IDX_PERSONA_X_CONSEC };
                            break;
                        case "NombreArtistico":
                            ordenar = OrdenarPorNombreArtistico();
                            orden = new[] { "design_document", PersonasCouchDbContext.IDX_PERSONA_X_NOMBREARTISTICO };
                            break;
                        case "edad":
                            ordenar = OrdenarPorEdad();
                            orden = new[] { "design_document", PersonasCouchDbContext.IDX_PERSONA_X_EDAD };
                            break;
                        default:
                            ordenar = OrdenarPorNombre();
                            orden = new[] { "design_document", PersonasCouchDbContext.IDX_PERSONA_X_NOMBRE };
                            break;
                    }
                }


                int total = 0;
                List<Persona> personas = new List<Persona>();
                List<Persona> todos = new List<Persona>();
                if (busqueda.Request.Ids.Count > 0)
                {
                    todos = await db.Personas.FindManyAsync(busqueda.Request.Ids);
                    if (busqueda.OrdernarASC==true)
                    {
                       todos= todos.OrderBy(ordenar).ToList();
                    }
                    else
                    {
                        todos = todos.OrderByDescending(ordenar).ToList();
                    }
                    total = todos.Count();
                    personas = todos.Skip((busqueda.Pagina - 1) * busqueda.Tamano).Take(busqueda.Tamano).ToList();
                  
                }

                return new ResponsePaginado<Persona>()
                {
                    Elementos = personas,
                    Pagina = busqueda.Pagina,
                    Tamano = busqueda.Tamano,
                    Total = total
                };

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }


        }

        private Expression<Func<Persona, bool>>? PorHabilidades(List<string>? Habilidades)
        {
            if (Habilidades != null && Habilidades.Count > 0)
            {
                return p => p.ActividadesIds.Contains(Habilidades);
            }
            return null;
        }

        private Expression<Func<Persona, bool>>? PorIdiomas(List<string>? Idiomas)
        {
            if (Idiomas != null && Idiomas.Count > 0)
            {
                return p => p.IdiomasIds.Contains(Idiomas);
            }
            return null;
        }


        /// <summary>
        ///  Obtiene la expresión para los colores de cabello 
        /// </summary>
        /// <param name="ColoresCabello"></param>
        /// <returns></returns>
        private Expression<Func<Persona, bool>>? PorColorCabello(List<string>? ColoresCabello)
        {
            if (ColoresCabello != null && ColoresCabello.Count > 0)
            {
                return p => p.PropiedadesFisicas.ColorCabelloId.In(ColoresCabello);
            }

            return null;
        }



        /// <summary>
        ///  Obtiene la expresión para los tipos de cabello 
        /// </summary>
        /// <param name="TiposCabello"></param>
        /// <returns></returns>
        private Expression<Func<Persona, bool>>? PorTipoCabello(List<string>? TiposCabello)
        {
            if (TiposCabello != null && TiposCabello.Count > 0)
            {
                return p => p.PropiedadesFisicas.TipoCabelloId.In(TiposCabello);
            }

            return null;
        }



        /// <summary>
        /// Realiza ala busqueda por nombre
        /// </summary>
        /// <param name="nombre"></param>
        /// <returns></returns>
        private Expression<Func<Persona, bool>>? PorTipoNombre(string nombre)
        {

            if (!string.IsNullOrEmpty(nombre))
            {
                Expression<Func<Persona, bool>> expresion = p => p.NombreBusqueda.IsMatch($"(?i)({nombre})");
                return expresion;
            }

            return null;
        }

        private Expression<Func<Persona, bool>>? PorColorOjos(List<string>? Colores)
        {
            if (Colores != null && Colores.Count > 0)
            {
                return p => p.PropiedadesFisicas.ColorOjosId.In(Colores);
            }

            return null;
        }


        /// <summary>
        ///  Obtiene la expresión para los géneros 
        /// </summary>
        /// <param name="Generos"></param>
        /// <returns></returns>
        private Expression<Func<Persona, bool>>? PorEtnias(List<string>? Etnias)
        {
            if (Etnias != null && Etnias.Count > 0)
            {
                return p => p.PropiedadesFisicas.EtniaId.In(Etnias);
            }

            return null;
        }



        /// <summary>
        ///  Obtiene la expresión para busqueda por tipo de cuaerpo
        /// </summary>
        /// <param name="Generos"></param>
        /// <returns></returns>
        private Expression<Func<Persona, bool>>? PorTipoCuerpo(List<TipoCuerpo>? Tipos)
        {
            if (Tipos != null && Tipos.Count > 0)
            {
                bool primera = true;
                Expression<Func<Persona, bool>> expresion = null;

                Tipos.ForEach(t =>
                {
                    Expression<Func<Persona, bool>> tmpExp = null;
                    switch (t) {
                        case TipoCuerpo.Normal:
                            tmpExp = p => p.PropiedadesFisicas.IMC >= (decimal)18.5 && p.PropiedadesFisicas.IMC < (decimal)25.0;
                            break;

                        case TipoCuerpo.Obeso:
                            tmpExp = p => p.PropiedadesFisicas.IMC >= (decimal)30.0;
                            break;

                        case TipoCuerpo.Bajo:
                            tmpExp = p => p.PropiedadesFisicas.IMC < (decimal)18.5;
                            break;

                        case TipoCuerpo.Sobrepeso:
                            tmpExp = p => p.PropiedadesFisicas.IMC >= (decimal)25.0 && p.PropiedadesFisicas.IMC < (decimal)30.0;
                            break;

                    }

                    if( tmpExp != null )
                    {
                        if(primera)
                        {
                            primera = false;
                            expresion = tmpExp;
                        } else
                        {
                            expresion = expresion.Or(tmpExp);
                        }
                    } 

                    
                });

                return expresion;
            }

            return null;
        }


        /// <summary>
        ///  Obtiene la expresión para los géneros 
        /// </summary>
        /// <param name="Generos"></param>
        /// <returns></returns>
        private Expression<Func<Persona, bool>>? PorGenero(List<string>? Generos)
        {
            if (Generos !=null && Generos.Count > 0) {
                return p => p.GeneroId.In(Generos); 
            }

            return null;
        }

        /// <summary>
        /// Ontiene la expresion para los fisltros de edad
        /// </summary>
        /// <param name="Min"></param>
        /// <param name="Max"></param>
        /// <returns></returns>
        private Expression<Func<Persona, bool>>? PorEdad (int? Min, int? Max)
        {

            if (!Min.HasValue && !Max.HasValue) return null;

            if(Min.HasValue)
            {
                long minimo = 0; 

                if (Max.HasValue)
                {
                    minimo = EdadATicks(Min.Value, false, false);
                    long maximo = EdadATicks(Max.Value, false, false);

                    // Intervalo
                    if(Min.Value == Max.Value)
                    {
                        minimo = EdadATicks(Min.Value, true, false);
                        maximo = EdadATicks(Max.Value, false, true);
                        return p => p.TicksFechaNacimiento <= maximo && p.TicksFechaNacimiento >= minimo;
                    }
                    return p => p.TicksFechaNacimiento <= minimo && p.TicksFechaNacimiento >= maximo;
                }
                else
                {
                    // >= min
                    minimo = EdadATicks(Min.Value, false, false);
                    return p => p.TicksFechaNacimiento <= minimo;

                }
                
            } else
            {
                if (Max.HasValue)
                {
                    long maximo = EdadATicks(Max.Value, false, false);

                    // <= max
                    return p => p.TicksFechaNacimiento >= maximo;

                }
            }

            return null;
        }


        /// <summary>
        ///  Ontiene la fecha en ticks en base a la edad
        /// </summary>
        /// <param name="Edad"></param>
        /// <returns></returns>
        private static long EdadATicks(int Edad, bool inicioAno, bool finAno)
        {
            DateTime? f = new DateTime();

            if (!inicioAno && !finAno)
            {
                f = DateTime.UtcNow.AddYears(((Edad + 1)*-1));
            }
            
            if(inicioAno)
            {
                f = DateTime.UtcNow.AddYears(((Edad + 1) * -1));
                f = new DateTime(f.Value.Year, 1, 1, 0, 0, 0,DateTimeKind.Utc);
            }

            if(finAno)
            {
                f = DateTime.UtcNow.AddYears(((Edad + 1) * -1));
                f = new DateTime(f.Value.Year, 12, 31, 23, 59, 59,DateTimeKind.Utc);
            }

            return f.Value.Ticks;
        }

        public async Task<RespuestaPayload<CastingPersona>> MisCastings(string usuarioId)
        {
            var r = new RespuestaPayload<CastingPersona>();
            var rPersona = await PorUsuarioId(usuarioId);
            if(rPersona.Ok)
            {
                var persona = (Persona)rPersona.Payload;
                r.Ok = true;
                r.Payload = persona.Castings;
            }
            return r;
        }

        public async Task<Respuesta> AdicionarCasting(string personaId, string clienteId, string castingId, string folderId)
        {
            var respuesta = new Respuesta();
            var r = await PorId(personaId);
            var persona = (Persona)r.Payload;
            if(r.Ok)
            {
                var personaCasting = new CastingPersona() { CastingId = castingId, ClienteId = clienteId, Declinado = false, FechaAdicion = DateTime.UtcNow };
                var nameFolder = $"{persona.Consecutivo}-{persona.NombreArtistico}-{persona.Nombre}{persona.Apellido1}{persona.Apellido2}";
                var f = await almacenamiento.CreateFolder(clienteId,nameFolder,folderId);
                if(f!=null)
                {
                    personaCasting.FolderId = f.Id;
                }
                persona.Castings.Add(personaCasting);
                await Actualizar(persona);
                respuesta.Ok = true;

            }
            return respuesta;
        }

        public async Task<Respuesta> RemoverCasting(string personaId, string clienteId, string castingId)
        {
            var respuesta = new Respuesta();
            var r = await PorId(personaId);
            var persona = (Persona)r.Payload;
            if (r.Ok)
            {
                persona.Castings.Remove(persona.Castings.FirstOrDefault(_=>_.CastingId==castingId));
                await Actualizar(persona);
                respuesta.Ok = true;

            }
            return respuesta;
        }
    }
}
