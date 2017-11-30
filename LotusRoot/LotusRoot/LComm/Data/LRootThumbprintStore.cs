using LotusRoot.CComm.CData;
using LotusRoot.RComm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LotusRoot.LComm.Data
{
    [Serializable()]
    public class LRootThumbprintStore
    {
        private List<LRootThumbprintKeyPair> _thumbprints = new List<LRootThumbprintKeyPair>();

        public bool ThumbprintStoreContains(Root root)
        {
            return GetKeyPair(root) != null;
        }

        public LRootThumbprintKeyPair GetKeyPair(Root root)
        {
            return _thumbprints.Where((x) => x.Root.Equals(root)).FirstOrDefault();
        }

        public void AddRootThumbprint(Root root)
        {
            _thumbprints.Add(new LRootThumbprintKeyPair(root));
        }

        public Root GetRootFromIdentifier(String identifier)
        {
            foreach (LRootThumbprintKeyPair keyPair in _thumbprints)
            {
                foreach (CThumbprint loop in keyPair.Thumbprints)
                {
                    if (loop.CIdentifier.Equals(identifier))
                    {
                        return keyPair.Root;
                    }
                }
            }
            return null;
        }

        internal List<LRootThumbprintKeyPair> Thumbprints
        {
            get
            {
                return _thumbprints;
            }
        }
    }
}
