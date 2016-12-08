using LotusRoot.RComm;
using LotusWeb.Logic.RComm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LotusWeb.Data
{
    public static class WRootStore
    {
        private static Dictionary<Root, RConnection> _roots = new Dictionary<Root, RConnection>();

        public static IList<Root> Roots
        {
            get
            {
                return _roots.Keys.ToList();
            }
        }

        public static RConnection GetConnectionFromRoot(Root root)
        {
            return _roots[root];
        }

        public static void AddRoot(Root root, RConnection connection)
        {
            if (!_roots.ContainsKey(root))
            {
                _roots.Add(root, connection);
            }
        }

        public static void RemoveRoot(Root root)
        {
            if (_roots.ContainsKey(root))
            {
                _roots.Remove(root);
            }
        }

        public static Root GetRootByIdentifier(String identifier)
        {
            foreach (KeyValuePair<Root, RConnection> root in _roots)
            {
                if (root.Key.Identifier.Equals(identifier))
                {
                    return root.Key;
                }
            }
            return null;
        }
    }
}
