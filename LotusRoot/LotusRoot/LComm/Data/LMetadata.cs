using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LotusRoot.LComm.Data
{
    [Flags]
    public enum LMetadata : byte
    {
        ENCRYPTED = 1,
        HEARTBEAT = 2,
        REQUEST = 4,
        RESPONSE = 8,
        PLACEHOLDER0 = 16,
        PLACEHOLDER1 = 32,
        HANDSHAKE = 64,
        NOTHING = 128
    }
}
