using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NugetForUnity.V3
{
    [Serializable]
    public class V3ResponseCatalogEntryGzip : V3ResponseData
    {
        public string packageContent;
    }
}
