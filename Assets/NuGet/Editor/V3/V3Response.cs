using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NugetForUnity.V3
{
    [Serializable]
    public class V3Response
    {
        public int totalHits;

        public List<V3ResponseData> data;
    }
}
