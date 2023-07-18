using Bogus;
using CouchDB.Driver.Extensions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using promodel.modelo.clientes;
using promodel.modelo.perfil;

namespace promodel.servicios.perfil
{
    public class ServicioCatalogos : IServicioCatalogos
    {

        private readonly CatalogosCouchDbContext db;
        private readonly IDistributedCache cache;
        private readonly IConfiguration configuration;
        private readonly IServicioClientes DbClientes;

        public ServicioCatalogos(CatalogosCouchDbContext db, IDistributedCache cache, IConfiguration configuration, IServicioClientes servicioClientes)
        {
            this.db = db;
            this.cache = cache;
            this.configuration = configuration;
            this.DbClientes = servicioClientes;
        }

        public async Task<ElementoCatalogo> BuscaCrea(string ClienteId, string ClaveCatalogo, string Texto)
        {
            var c = db.Catalogos.FirstOrDefault(x => x.TipoPropiedad == ClaveCatalogo && x.ClienteId == ClienteId);
            if(c!=null)
            {
                ElementoCatalogo item = c.Elementos.FirstOrDefault(e => e.Texto.Equals(Texto, StringComparison.InvariantCultureIgnoreCase));
                if (item == null)
                {
                    item = new ElementoCatalogo() { Clave = Guid.NewGuid().ToString(), Texto = Texto, Idioma = "es" };
                    c.Elementos.Add(item);
                    await UpdateCatalogo(c);
                }
                return item;
            }
            return null;
        }

        public async Task UpdateCatalogo(CatalogoBase c)
        {

            await db.Catalogos.AddOrUpdateAsync(c);
            string idcache = $"{c.TipoPropiedad}-{c.ClienteId}";
            try
            {
                cache.Remove(idcache);
            }
            catch (Exception)
            {

            }
            await cache.SetStringAsync(idcache, JsonConvert.SerializeObject(c),
                new DistributedCacheEntryOptions() { AbsoluteExpirationRelativeToNow = new TimeSpan(0, 5, 0) });

        }

        public async Task<CatalogoBase> GetCatalogoCliente(string ClaveCatalogo, string ClienteId)
        {

            string idcache = $"{ClaveCatalogo}-{ClienteId}";
            string? cs = cache.GetString(idcache);
            CatalogoBase? c = null;

            if (cs != null)
            {
                return JsonConvert.DeserializeObject<CatalogoBase>(cs);
            }

            c = await db.Catalogos.Where(x => x.ClienteId == ClienteId && x.TipoPropiedad == ClaveCatalogo ).FirstOrDefaultAsync();
            if(c==null)
            {
                switch (ClaveCatalogo)
                {
                    case Perfil.CAT_AGENCIAS:
                        c = Perfil.NuevoCatalogo(ClienteId, ClaveCatalogo, Perfil.Agencias());
                        await db.Catalogos.AddAsync(c);
                        break;

                    case Perfil.CAT_TALLAS_VESTUARIO:
                        c = Perfil.NuevoCatalogo(ClienteId, ClaveCatalogo, Perfil.TallasVestuarioBase());
                        await db.Catalogos.AddAsync(c);
                        break;

                    case Perfil.CAT_IDIOMA:
                        c = Perfil.NuevoCatalogo(ClienteId, ClaveCatalogo, Perfil.IdiomaBase());
                        await db.Catalogos.AddAsync(c);
                        break;

                    case Perfil.CAT_ETNIA:
                        c = Perfil.NuevoCatalogo(ClienteId, ClaveCatalogo, Perfil.EtniaBase());
                        await db.Catalogos.AddAsync(c);
                        break;

                    case Perfil.CAT_TIPO_CABELLO:
                        c = Perfil.NuevoCatalogo(ClienteId, ClaveCatalogo, Perfil.TipoCabelloBase());
                        await db.Catalogos.AddAsync(c);
                        break;

                    case Perfil.CAT_COLOR_OJOS:
                        c = Perfil.NuevoCatalogo(ClienteId, ClaveCatalogo, Perfil.ColorOjosBase());
                        await db.Catalogos.AddAsync(c);
                        break;

                    case Perfil.CAT_COLOR_CABELLO:
                        c = Perfil.NuevoCatalogo(ClienteId, ClaveCatalogo, Perfil.ColorCabelloBase());
                        await db.Catalogos.AddAsync(c);
                        break;

                    case Perfil.CAT_GENERO:
                        c = Perfil.NuevoCatalogo(ClienteId, ClaveCatalogo, Perfil.GeneroBase());
                        await db.Catalogos.AddAsync(c); 
                        break;

                    case Perfil.CAT_ESTADOS_MX:
                        c = Perfil.NuevoCatalogo(ClienteId, ClaveCatalogo, Perfil.EstadosMX());
                        await db.Catalogos.AddAsync(c);
                        break;

                    case Perfil.CAT_ESTADOS_US:
                        c = Perfil.NuevoCatalogo(ClienteId, ClaveCatalogo, Perfil.EstadosUS());
                        await db.Catalogos.AddAsync(c);
                        break;

                    case Perfil.CAT_PAISES:

                        c = Perfil.NuevoCatalogo(ClienteId, ClaveCatalogo, Perfil.PaisesBase());
                        await db.Catalogos.AddAsync(c);
                        break;

                    case Perfil.CAT_ACTIVIDADES:
                        c = Perfil.NuevoCatalogo(ClienteId, ClaveCatalogo, Perfil.ActividadesBase());
                        await db.Catalogos.AddAsync(c);
                        break;
                }

                if (c != null) {
                    await cache.SetStringAsync(idcache, JsonConvert.SerializeObject(c) , 
                        new DistributedCacheEntryOptions() { AbsoluteExpirationRelativeToNow = new TimeSpan(0,5,0) });
                }
            }

            return c;
        }

        public async Task<List<CatalogoBase>> GetCatalogosPerfil(string ClienteId)
        {
            List<CatalogoBase> catalogos = new();
            List<string> l = new List<string>() { Perfil.CAT_GENERO, 
            Perfil.CAT_PAISES, Perfil.CAT_ESTADOS_US, Perfil.CAT_ESTADOS_MX,
            Perfil.CAT_COLOR_CABELLO, Perfil.CAT_COLOR_OJOS, Perfil.CAT_TIPO_CABELLO,
            Perfil.CAT_ETNIA, Perfil.CAT_IDIOMA, Perfil.CAT_ACTIVIDADES, Perfil.CAT_TALLAS_VESTUARIO, 
            Perfil.CAT_AGENCIAS};

            foreach(string clave in l)
            {
                var c = await GetCatalogoCliente(clave, ClienteId);
                c.Elementos=c.Elementos.OrderBy(x=>x.Texto).ToList();
                if (clave == "pais")
                {
                    var cliente = await DbClientes.ClientePorId(ClienteId);
                    var pais = c.Elementos.FirstOrDefault(_ => _.Clave == cliente.PaisDefault);
                    if (cliente != null && pais != null)
                    {
                        c.Elementos.Insert(0, pais);
                    }
                }
                catalogos.Add(c);
            }
            return catalogos;

        }
    }
}
