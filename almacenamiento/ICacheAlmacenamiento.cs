using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace almacenamiento
{
    public interface ICacheAlmacenamiento
    {
        Task CreaArchivoImagen(string Archivo, string NuevoNombre, string Folder, bool EsImagen);
        string? FotoById(string usuarioid, string id, string tipo);
    }
}
