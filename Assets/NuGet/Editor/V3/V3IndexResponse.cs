using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NugetForUnity.V3
{
    [Serializable]
    public class V3IndexResponse
    {
        public string version;

        public List<V3IndexResource> resources;
    }
}
