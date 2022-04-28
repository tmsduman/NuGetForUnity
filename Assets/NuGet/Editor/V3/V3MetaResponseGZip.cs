using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NugetForUnity.V3
{
    [Serializable]
    public class V3MetaResponseGZip
    {
        public string id;

        public V3ResponseCatalogEntryGzip catalogEntry;
    }
}
