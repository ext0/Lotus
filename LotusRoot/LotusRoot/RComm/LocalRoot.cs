using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LotusRoot.RComm
{
    public static class LocalRoot
    {
        public static readonly IPAddress LOCAL_CAPTURE_ADDRESS = IPAddress.Parse("0.0.0.0");

        public static Root Local { get; set; }
    }
}
