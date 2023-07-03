using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace almacenamiento.GoogleDrive
{
    public interface IGoogleDriveConfigProvider
    {
        Task<GoogleDriveConfig> GetConfig(string ClientId);
        Task<GoogleDriveConfig> GetConfig();
    }
}
