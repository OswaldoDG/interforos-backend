using CouchDB.Driver.Extensions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using promodel.modelo.media;

namespace promodel.servicios.media
{
    public class MediaService: IMedia
    {
        private readonly MediaCouchDbContext db;
        private readonly IDistributedCache cache;
        private readonly IConfiguration configuration;
        public MediaService(MediaCouchDbContext db, IDistributedCache cache, IConfiguration configuration)
        {
            this.db = db;
            this.cache = cache;
            this.configuration = configuration;
        }


        public async Task<bool> EliminarElemento(string UsuarioId, string ElementoId)
        {

            string mm = await cache.GetStringAsync($"media-{UsuarioId}");
            if (!string.IsNullOrEmpty(mm))
            {
                await cache.RemoveAsync($"media-{UsuarioId}");
            }

            MediaModelo m = await db.Medios.Where(x => x.UsuarioId == UsuarioId).FirstOrDefaultAsync();
            if (m != null)
            {
                var e = m.Elementos.FirstOrDefault(x => x.Id == ElementoId);
                if (e != null)
                {
                    m.Elementos.Remove(e);
                    await db.Medios.AddOrUpdateAsync(m);
                }

                await cache.SetStringAsync($"media-{UsuarioId}", JsonConvert.SerializeObject(m), new DistributedCacheEntryOptions()
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                });

                return e != null;

            }
            return false;

        }

        public async Task<bool> EstablecerPrincipal(string UsuarioId, string ElementoId)
        {

            string mm = await cache.GetStringAsync($"media-{UsuarioId}");
            if (!string.IsNullOrEmpty(mm))
            {
                await cache.RemoveAsync($"media-{UsuarioId}");
            }

            MediaModelo m = await db.Medios.Where(x => x.UsuarioId == UsuarioId).FirstOrDefaultAsync();
            if (m != null)
            {
                foreach(var e in m.Elementos)
                {
                    if(e.Id == ElementoId)
                    {
                        e.Principal = true;
                        e.Permanente = true;
                    } else
                    {
                        if(e.Principal)
                        {
                            e.Permanente = false;
                        }
                        e.Principal = false;
                    }
                    
                }
                var done = m.Elementos.FirstOrDefault(x => x.Id == ElementoId);
                await db.Medios.AddOrUpdateAsync(m);

                await cache.SetStringAsync($"media-{UsuarioId}", JsonConvert.SerializeObject(m), new DistributedCacheEntryOptions()
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                });

                
                
                return done != null;

            }
            return false;
            
        }


        public async Task<bool> AlternarBloqueo(string UsuarioId, string ElementoId)
        {

            string mm = await cache.GetStringAsync($"media-{UsuarioId}");
            if (!string.IsNullOrEmpty(mm))
            {
                await cache.RemoveAsync($"media-{UsuarioId}");
            }

            MediaModelo m = await db.Medios.Where(x => x.UsuarioId == UsuarioId).FirstOrDefaultAsync();
            if (m != null)
            {
                var e = m.Elementos.FirstOrDefault(x => x.Id == ElementoId);
                if (e != null)
                {
                    e.Permanente = !e.Permanente;
                    await db.Medios.AddOrUpdateAsync(m);
                }

                await cache.SetStringAsync($"media-{UsuarioId}", JsonConvert.SerializeObject(m), new DistributedCacheEntryOptions()
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                });

                return e != null;

            }
            return false;

        }


        public async Task<MediaModelo> GetByUsuarioId(string UsuarioId)
        {

            MediaModelo m = null;
            string mm = await cache.GetStringAsync($"media-{UsuarioId}");
            if(!string.IsNullOrEmpty(mm))
            {
                m = JsonConvert.DeserializeObject<MediaModelo>(mm);
                return m;
            }

            m = await db.Medios.Where(x => x.UsuarioId == UsuarioId).FirstOrDefaultAsync();
            if (m == null)
            {
                m = new MediaModelo()
                {
                    Id = Guid.NewGuid().ToString(),
                    UsuarioId = UsuarioId,
                    EliminacionTicks = null,
                };
            } else
            {
                await cache.SetStringAsync($"media-{UsuarioId}", JsonConvert.SerializeObject(m), new DistributedCacheEntryOptions()
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                });
            }

            await db.Medios.AddOrUpdateAsync(m);

            return m;
        }


        public async Task<ElementoMedia> AddElemento(ElementoMedia el, string UsuarioId)
        {
            string mm = await cache.GetStringAsync($"media-{UsuarioId}");
            if (!string.IsNullOrEmpty(mm))
            {
                await cache.RemoveAsync($"media-{UsuarioId}");
            }

            MediaModelo m = await db.Medios.Where(x => x.UsuarioId == UsuarioId).FirstOrDefaultAsync();
            if (m == null)
            {
                m = new MediaModelo()
                {
                    Id = Guid.NewGuid().ToString(),
                    UsuarioId = UsuarioId,
                    EliminacionTicks = null,
                };
            }

            m.Elementos.Add(el);

            long? minTicks = null;
            if (m.Elementos.Any(x => x.EliminacionTicks.HasValue))
            {
                minTicks = m.Elementos.Select(x => x.EliminacionTicks).Min();
            }

            m.TamanoBytes = m.Elementos.Sum(x => x.TamanoBytes);
            m.EliminacionTicks = minTicks;

            await db.Medios.AddOrUpdateAsync(m);

            await cache.SetStringAsync($"media-{UsuarioId}", JsonConvert.SerializeObject(m), new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            });

            return el;
        }


        public async Task DelElemento(string Id, string UsuarioId)
        {

            string mm = await cache.GetStringAsync($"media-{UsuarioId}");
            if (!string.IsNullOrEmpty(mm))
            {
                await cache.RemoveAsync($"media-{UsuarioId}");
            }

            MediaModelo m = await db.Medios.Where(x => x.UsuarioId == UsuarioId).FirstOrDefaultAsync();
            if (m != null)
            {


                var el = m.Elementos.FirstOrDefault(x => x.Id == Id);
                if (el != null)
                {
                    m.Elementos.Remove(el);
                    long? minTicks = null;
                    if (m.Elementos.Any(x => x.EliminacionTicks.HasValue))
                    {
                        minTicks = m.Elementos.Select(x => x.EliminacionTicks).Min();
                    }

                    if (m.Elementos.Count > 0)
                    {
                        m.TamanoBytes = m.Elementos.Sum(x => x.TamanoBytes);
                    } else
                    {
                        m.TamanoBytes = 0;
                    }
                    
                    m.EliminacionTicks = minTicks;
                    await db.Medios.AddOrUpdateAsync(m);

                    await cache.SetStringAsync($"media-{UsuarioId}", JsonConvert.SerializeObject(m), new DistributedCacheEntryOptions()
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                    });

                }

            }

         
        }

    }
}
