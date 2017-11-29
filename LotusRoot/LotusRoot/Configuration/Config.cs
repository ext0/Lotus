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
            public ushort RPort { get; set; }
        }

        public RootEntry[] Roots { get; set; }
        public ushort LocalRPort { get; set; }
        public String LocalExternalIP { get; set; }
        public ushort[] LocalCPorts { get; set; }
        public String LocalIdentifier { get; set; }
        public String RemoteWebHost { get; set; }
        public ushort RemoteWebHostPort { get; set; }
    }
}
