using log4net;
using LotusRoot.CComm.CData;
using LotusRoot.RComm;
using LotusWeb.Logic.RComm;
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

            CThumbprint exists = _thumbprints[root].Where((x) => x.Equals(thumbprint)).FirstOrDefault();
            if (exists != null)
            {
                exists.Active = true;
                Logger.Debug("CThumbprint reactivated (" + thumbprint.CIdentifier + ")!");
            }
            else
            {
                _thumbprints[root].Add(thumbprint);
                Logger.Debug("CThumbprint added (" + thumbprint.CIdentifier + ")!");
            }
        }

        public static void DisableCThumbprint(Root root, CThumbprint thumbprint)
        {
            if (_thumbprints.ContainsKey(root))
            {
                CThumbprint disabling = _thumbprints[root].Where((x) => x.Equals(thumbprint)).FirstOrDefault();
                if (disabling != null)
                {
                    disabling.Active = false;
                    Logger.Debug("Disabled CThumbprint (" + thumbprint.CIdentifier + ")");
                }
                else
                {
                    Logger.Warn("Tried to disable nonexistant CThumbprint (" + thumbprint.CIdentifier + ")");
                }
            }
        }

        public static void DisableAllCThumbprints(Root root)
        {
            if (_thumbprints.ContainsKey(root))
            {
                foreach (CThumbprint thumbprint in _thumbprints[root])
                {
                    thumbprint.Active = false;
                }
            }
        }

        public static RConnection GetConnectionFromCIdentifier(String identifier)
        {
            Root root = FindRootFromCIdentifier(identifier);
            if (root != null)
            {
                return WRootStore.GetConnectionFromRoot(root);
            }
            return null;
        }

        public static Root FindRootFromCIdentifier(String identifier)
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
