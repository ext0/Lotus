using LotusRoot.CComm.CData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LotusRoot.RComm
{
    public interface RRemoteInvokable
    {
        List<Root> GetKnownRoots();

        String GetIdentifier();

        Root FindCConnection(String identifier);

        ushort[] GetCPorts();

        void NotifyCConnection(Root root, CThumbprint thumbprint);

        void NotifyCDrop(Root root, CThumbprint thumbprint);

        List<CThumbprint> GetLocalCThumbprints();
    }
}
