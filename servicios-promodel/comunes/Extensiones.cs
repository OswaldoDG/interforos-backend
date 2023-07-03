using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace promodel.servicios
{
    public static class Extensiones
    {
        public static RespuestaPayload<T> OK<T>(this RespuestaPayload<T> respuesta, object payload) where T : class 
        {
            respuesta.Payload = payload;
            respuesta.Ok = true;
            respuesta.HttpCode = HttpCode.Ok;
            return respuesta;
        }

        public static Respuesta OK(this Respuesta respuesta) 
        {
            respuesta.Ok = true;
            respuesta.HttpCode = HttpCode.Ok;
            return respuesta;
        }

    }
}
