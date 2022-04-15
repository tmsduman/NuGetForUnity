using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NugetForUnity.V3
{
    [Serializable]
    public class V3ResponseData
    {
        public string type;

        public string authors;

        public string id;

        public string title;

        public string summary;

        public int totalDownloads;

        public bool verified;

        public string version;

        public List<V3ResponseVersion> versions;

        public string tags;
    }
}
