using promodel.modelo.clientes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace promodel.servicios
{
    public interface IServicioClientes
    {
        Task<Cliente?> ClientePorId(string Id);
        Task<Cliente?> ClientePorUrl(string url);
        Task<Cliente> Upsert(Cliente cliente);
        
    }
}
