using LotusRoot.CComm.CData;
using LotusRoot.Datastore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LotusRoot.RComm
{
    public class RemoteRoot : MarshalByRefObject, RRemoteInvokable
    {
        public List<Root> GetKnownRoots()
        {
            return RHandler.RegisteredRoots;
        }

        public String GetIdentifier()
        {
            return LocalRoot.Local.Identifier;
        }

        public short[] GetCPorts()
        {
            return LocalRoot.Local.CPorts;
        }

        public Root FindCConnection(String identifier)
        {
            return RClientStore.FindRootFromCThumbprint(identifier);
        }

        public void NotifyCConnection(Root root, CThumbprint thumbprint)
        {
            RClientStore.AddRemoteCThumbprint(root, thumbprint);
        }

        public List<CThumbprint> GetLocalCThumbprints()
        {
            return RClientStore.LocalThumbprints;
        }

        public void NotifyCDrop(Root root, CThumbprint thumbprint)
        {
            RClientStore.RemoveRemoteCThumbprint(root, thumbprint);
        }
    }
}
