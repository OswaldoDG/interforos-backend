using promodel.modelo.media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace promodel.servicios.media
{
    public interface IMedia
    {
        Task<bool> EliminarElemento(string UsuarioId, string ElementoId);
        Task<bool> EstablecerPrincipal(string UsuarioId, string ElementoId);
        Task<bool> AlternarBloqueo(string UsuarioId, string ElementoId);
        Task<MediaModelo> GetByUsuarioId(string UsuarioId);
        Task<ElementoMedia> AddElemento(ElementoMedia el, string UsuarioId);
        Task DelElemento(string Id, string UsuarioId);
    }
}
