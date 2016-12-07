using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LotusRoot.Configuration
{
    public class Config
    {
        public class RootEntry
        {
            public String IP { get; set; }
            public short RPort { get; set; }
        }

        public RootEntry[] Roots { get; set; }
        public short LocalRPort { get; set; }
        public String LocalExternalIP { get; set; }
        public short[] LocalCPorts { get; set; }
        public String LocalIdentifier { get; set; }
        public String RemoteWebHost { get; set; }
        public short RemoteWebHostPort { get; set; }
    }
}
