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
    public class LRootThumbprintKeyPair
    {
        private Root _root;
        private List<CThumbprint> _thumbprints;

        public LRootThumbprintKeyPair(Root root)
        {
            _root = root;
            _thumbprints = new List<CThumbprint>();
        }

        public List<CThumbprint> Thumbprints
        {
            get
            {
                return _thumbprints;
            }
        }

        public Root Root
        {
            get
            {
                return _root;
            }
        }
    }
}
