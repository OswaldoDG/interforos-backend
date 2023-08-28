using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace promodel.servicios.personas
{
    public class ServicioPersonasUsuario : IServicioPersonasUsuario
    {
        public Task<Respuesta> AdicionaPersona(string personaId, string clienteId, string usuarioId, string? agenciaId = null)
        {
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
            return null;
        }

        public Task<RespuestaPayload<List<string>>> ObtienePersonasRegistradas(string clienteId, string usuarioId, string? agenciaId = null)
        {
            // trae el registro en la base de datos que coincide  con los parámetros  
            // string clienteId, string usuarioId, string? agenciaId = null
            //
            // Si el registro NO EXISTE devuelve una lista vacia
            //
            // si el registro exiete devuelve el contenido de la propiedad IdPersonas

            return null;
        }

        public Task<Respuesta> RemuevePersona(string personaId, string clienteId, string usuarioId, string? agenciaId = null)
        {
            // trae el registro en la base de datos que coincide  con los parámetros  
            // string clienteId, string usuarioId, string? agenciaId = null
            //
            // Si el registro NO EXISTE devuelve 404
            //
            // Si el registro existe verifica si el valor del parametrot personaId esta en la lista IdPersonas
            // si el valor EXISTE lo elimina de la lista y actualiza el registro
            return null;
        }
    }
}
