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
        /*
            Metadata flags cheat sheet
            0000 000X = encrypted
            0000 00X0 = heartbeat
            0000 0X00 = from-client
            0000 X000 = from-root
            000X 0000 = from-web
            00X0 0000 = to-client
            0X00 0000 = to-root
            X000 0000 = to-web
        */

        ENCRYPTED = 1,
        HEARTBEAT = 2,
        FCLIENT = 4,
        FROOT = 8,
        FWEB = 16,
        TCLIENT = 32,
        TROOT = 64,
        TWEB = 128
    }
}
