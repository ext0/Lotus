using log4net;
using LotusRoot.CComm.CData;
using LotusRoot.RComm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LotusRoot.Datastore
{
    public static class RClientStore
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(RClientStore));

        private static Dictionary<Root, List<CThumbprint>> _thumbprints = new Dictionary<Root, List<CThumbprint>>();

        public static void AddLocalCThumbprint(CThumbprint thumbprint)
        {
            if (!_thumbprints.ContainsKey(LocalRoot.Local))
            {
                _thumbprints.Add(LocalRoot.Local, new List<CThumbprint>());
            }
            if (!_thumbprints[LocalRoot.Local].Contains(thumbprint))
            {
                Logger.Debug("Local CThumbprint added (" + thumbprint.CIdentifier + ")!");
                _thumbprints[LocalRoot.Local].Add(thumbprint);
            }
        }

        public static void AddRemoteCThumbprint(Root root, CThumbprint thumbprint)
        {
            if (!_thumbprints.ContainsKey(root))
            {
                _thumbprints.Add(root, new List<CThumbprint>());
            }
            if (!_thumbprints[root].Contains(thumbprint))
            {
                Logger.Debug("Remote CThumbprint added (" + thumbprint.CIdentifier + ")!");
                _thumbprints[root].Add(thumbprint);
            }
        }

        public static void RemoveLocalCThumbprint(CThumbprint thumbprint)
        {
            if (_thumbprints.ContainsKey(LocalRoot.Local))
            {
                bool success = _thumbprints[LocalRoot.Local].Remove(thumbprint);
                if (success)
                {
                    Logger.Debug("Removed local CThumbprint (" + thumbprint.CIdentifier + ")");
                }
                else
                {
                    Logger.Warn("Tried to remove nonexistant local CThumbprint (" + thumbprint.CIdentifier + ")");
                }
            }
        }

        public static void RemoveRemoteCThumbprint(Root root, CThumbprint thumbprint)
        {
            if (_thumbprints.ContainsKey(root))
            {
                bool success = _thumbprints[root].Remove(thumbprint);
                if (success)
                {
                    Logger.Debug("Removed remote CThumbprint (" + thumbprint.CIdentifier + ")");
                }
                else
                {
                    Logger.Warn("Tried to remove nonexistant remote CThumbprint (" + thumbprint.CIdentifier + ")");
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

        public static List<CThumbprint> LocalThumbprints
        {
            get
            {
                if (_thumbprints.ContainsKey(LocalRoot.Local))
                {
                    return _thumbprints[LocalRoot.Local];
                }
                else
                {
                    return new List<CThumbprint>();
                }
            }
        }
    }
}
