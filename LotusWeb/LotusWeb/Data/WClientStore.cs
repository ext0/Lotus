using log4net;
using LotusRoot.CComm.CData;
using LotusRoot.RComm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LotusWeb.Data
{
    public static class WClientStore
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(WClientStore));

        private static Dictionary<Root, List<CThumbprint>> _thumbprints = new Dictionary<Root, List<CThumbprint>>();

        public static void AddCThumbprint(Root root, CThumbprint thumbprint)
        {
            if (!_thumbprints.ContainsKey(root))
            {
                _thumbprints.Add(root, new List<CThumbprint>());
            }
            if (!_thumbprints[root].Contains(thumbprint))
            {
                Logger.Debug("CThumbprint added (" + thumbprint.CIdentifier + ")!");
                _thumbprints[root].Add(thumbprint);
            }
        }

        public static void RemoveCThumbprint(Root root, CThumbprint thumbprint)
        {
            if (_thumbprints.ContainsKey(root))
            {
                bool success = _thumbprints[root].Remove(thumbprint);
                if (success)
                {
                    Logger.Debug("Removed CThumbprint (" + thumbprint.CIdentifier + ")");
                }
                else
                {
                    Logger.Warn("Tried to remove nonexistant CThumbprint (" + thumbprint.CIdentifier + ")");
                }
            }
        }

        public static Root FindRootFromCThumbprint(String identifier)
        {
            foreach (KeyValuePair<Root, List<CThumbprint>> thumbprints in _thumbprints)
            {
                foreach (CThumbprint loop in thumbprints.Value)
                {
                    if (loop.CIdentifier.Equals(identifier))
                    {
                        return thumbprints.Key;
                    }
                }
            }
            return null;
        }

        public static IEnumerable<Root> FindRootFromAuth(String auth)
        {
            foreach (KeyValuePair<Root, List<CThumbprint>> thumbprints in _thumbprints)
            {
                foreach (CThumbprint loop in thumbprints.Value)
                {
                    if (loop.Auth.Equals(auth))
                    {
                        yield return thumbprints.Key;
                    }
                }
            }
        }

        public static IEnumerable<CThumbprint> GetThumbprintsFromAuth(String auth)
        {
            foreach (KeyValuePair<Root, List<CThumbprint>> thumbprints in _thumbprints)
            {
                foreach (CThumbprint loop in thumbprints.Value)
                {
                    if (loop.Auth.Equals(auth))
                    {
                        yield return loop;
                    }
                }
            }
        }
    }
}
