using System;
using System.Collections.Generic;
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
        private short[] _cports;
        private short _rport;
        private String _identifier;

        public Root(String endpoint, short[] cports, short rport, String identifier)
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

        public short[] CPorts
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

        public short RPort
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
