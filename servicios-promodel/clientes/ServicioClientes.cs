using CouchDB.Driver.Extensions;
using CouchDB.Driver.Query.Extensions;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using promodel.modelo.clientes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace promodel.servicios
{
    public class ServicioClientes: IServicioClientes
    {

        private readonly CLientesCouchDbContext db;
        private readonly IDistributedCache cache;
        public ServicioClientes(CLientesCouchDbContext db, IDistributedCache cache)
        {
            this.db = db;
            this.cache = cache;
        }

        public async Task<Cliente?> ClientePorId(string Id)
        {
            string k = $"cl-activos";
            string c = await cache.GetStringAsync(k);
            List<Cliente> cs = new();
            Cliente? cliente = null;

            if (c != null)
            {
                cs = JsonConvert.DeserializeObject<List<Cliente>>(c) ?? new List<Cliente>();
            }

            cliente = cs.FirstOrDefault(x => x.Id.Equals(Id, StringComparison.InvariantCultureIgnoreCase));

            if (cliente == null)
            {
                cliente = await db.Clientes.Where(x => x.Id == Id)
                    .UseIndex(new[] { "design_document", CLientesCouchDbContext.IDX_CLIENTE_X_URL }).FirstOrDefaultAsync();
                if (cliente != null)
                {
                    cs.Add(cliente);
                    await cache.SetStringAsync(k, JsonConvert.SerializeObject(cs),
                        new DistributedCacheEntryOptions() { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1) });
                }
            }

            return cliente;
        }

        public async Task<Cliente?> ClientePorUrl(string url)
        {
            url = url.ToLower();
            string k = $"cl-activos";
            string c = await cache.GetStringAsync(k);
            List<Cliente> cs = new();
            Cliente? cliente = null;

            if (c != null)
            {
                cs = JsonConvert.DeserializeObject<List<Cliente>>(c) ?? new List<Cliente>();
            }

            cliente = cs.FirstOrDefault(x => x.Url.Equals(url, StringComparison.InvariantCultureIgnoreCase));

            if (cliente == null)
            {
                cliente = await db.Clientes.Where(x => x.Url == url)
                    .UseIndex(new[] { "design_document", CLientesCouchDbContext.IDX_CLIENTE_X_URL }).FirstOrDefaultAsync();
                if (cliente != null)
                {
                    cs.Add (cliente);
                    await cache.SetStringAsync(k, JsonConvert.SerializeObject(cs), 
                        new DistributedCacheEntryOptions() {  AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1) });
                }
            }

            return cliente;
        }

        public Task<Cliente> Upsert(Cliente cliente)
        {
            return null;
        }
    }
}
