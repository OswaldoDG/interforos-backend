using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace almacenamiento.GoogleDrive
{
    public class GoogleDriveConfig
    {
        /// <summary>
        /// Ruta a archivo de OAuth2
        /// </summary>
        public string AuthJsonPath { get; set; }

        /// <summary>
        /// Account service with shared acces to the drive
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// Direcotio compartido a tomar como raíz, se puede obtener al navegar al folder en el navegador 
        /// </summary>
        public string RootDirectory { get; set; }
    }
}
