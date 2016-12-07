using LotusRoot.RComm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LotusWeb.Data
{
    public static class WRootStore
    {
        private static List<Root> _roots = new List<Root>();

        public static IList<Root> Roots
        {
            get
            {
                return _roots.AsReadOnly();
            }
        }

        public static void AddRoot(Root root)
        {
            if (!_roots.Contains(root))
            {
                _roots.Add(root);
            }
        }

        public static void RemoveRoot(Root root)
        {
            if (_roots.Contains(root))
            {
                _roots.Remove(root);
            }
        }

        public static Root GetRootByIdentifier(String identifier)
        {
            foreach (Root root in _roots)
            {
                if (root.Identifier.Equals(identifier))
                {
                    return root;
                }
            }
            return null;
        }
    }
}
