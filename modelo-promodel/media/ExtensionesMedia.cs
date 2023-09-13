using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace promodel.modelo.media
{
    public static class ExtensionesMedia
    {

        public static MediaCliente ToMediaClienteFotosYVideo(this MediaModelo media, string ClienteId, string? CastingId)
        {
            var m = new MediaCliente() { UsuarioId = media.UsuarioId };
            // && x.ClienteIds.Contains(ClienteId)))
            if(!string.IsNullOrEmpty(CastingId))
            {
                foreach (var item in media.Elementos.Where(x => (x.Imagen == true || x.Video == true)&& x.CastingId==CastingId))
                {
                    m.Elementos.Add(item.ToElementoMediaCliente());
                }
            }
            else
            {
                foreach (var item in media.Elementos.Where(x => (x.Imagen == true || x.Video == true)))
                {
                    m.Elementos.Add(item.ToElementoMediaCliente());
                }
            }

            return m;
        }
        

        public static ElementoMediaCliente ToElementoMediaCliente(this ElementoMedia item)
        {
            return new ElementoMediaCliente()
            {
                Extension = item.Extension,
                Id = item.Id,
                Imagen = item.Imagen,
                MimeType = item.MimeType,
                Permanente = item.Permanente,
                Video = item.Video,
                Landscape = item.Landscape,
                Principal = item.Principal,
                FrameVideoId = item.FrameVideoId,
                Pdf = item.Pdf,
                Tipo = item.Tipo,
                Titulo = item.Titulo,
                CastingId = item.CastingId,
            };
        }
    }
}
