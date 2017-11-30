using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LotusRoot.RComm
{
    [Serializable]
    public class Root
    {
        private String _endpoint;
        private ushort[] _cports;
        private ushort _rport;
        private String _identifier;

        public Root(String endpoint, ushort[] cports, ushort rport, String identifier)
        {
            _endpoint = endpoint;
            _cports = cports;
            _rport = rport;
            _identifier = identifier;
        }

        public String Endpoint
        {
            get
            {
                return _endpoint;
            }
        }

        public ushort[] CPorts
        {
            get
            {
                return _cports;
            }
            set
            {
                _cports = value;
            }
        }

        public ushort RPort
        {
            get
            {
                return _rport;
            }
            set
            {
                _rport = value;
            }
        }

        public String Identifier
        {
            get
            {
                return _identifier;
            }
            set
            {
                _identifier = value;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is Root)
            {
                return (obj as Root).Identifier.Equals(Identifier);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Identifier.GetHashCode();
        }

        public override string ToString()
        {
            return _identifier;
        }
    }
}
