using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace almacenamiento
{
    public class StorageObjectDescriptor
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public DateTime? Created  { get; set; }
    }
}
