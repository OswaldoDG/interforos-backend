using almacenamiento.GoogleDrive;
using Microsoft.Extensions.Configuration;

namespace promodel.servicios.almacenamiento
{
    public class PromodelGDriveProvider : IGoogleDriveConfigProvider
    {

        private IConfiguration config;
        public PromodelGDriveProvider(IConfiguration config)
        {
            this.config = config;
        }

        public Task<GoogleDriveConfig> GetConfig(string ClientId)
        {
            var c = GetConfig();
            return Task.FromResult(c.Result);
        }

        public Task<GoogleDriveConfig> GetConfig()
        {
            GoogleDriveConfig c = new GoogleDriveConfig()
            {
                Account = config["GoogleDriveConfig:Account"],
                AuthJsonPath = config["GoogleDriveConfig:AuthJsonPath"],
                RootDirectory = config["GoogleDriveConfig:RootDirectory"]
            };

            return Task.FromResult(c);
        }
    }
}
