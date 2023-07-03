using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace comunicaciones.email
{
    public interface IServicioEmail
    {
        Task<bool> Enviar(MensajeEmail msg);
    }
}
