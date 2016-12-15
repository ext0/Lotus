using LotusRoot.RComm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LotusRoot.CComm.CData
{
    [Serializable]
    public class CThumbprint
    {
        private String _cidentifier;
        private int _cversion;
        private String _cip;
        private DateTime _lastheartbeat;
        private String _auth;
        private String _hostname;

        public CThumbprint(String cIdentifier, int cVersion, String cIP, String auth, String hostname)
        {
            _cidentifier = cIdentifier;
            _cversion = cVersion;
            _cip = cIP;
            _lastheartbeat = DateTime.MinValue;
            _auth = auth;
            _hostname = hostname;
        }

        public String Auth
        {
            get
            {
                return _auth;
            }
            set
            {
                _auth = value;
            }
        }

        public String CIdentifier
        {
            get
            {
                return _cidentifier;
            }
            set
            {
                _cidentifier = value;
            }
        }

        public int CVersion
        {
            get
            {
                return _cversion;
            }
            set
            {
                _cversion = value;
            }
        }

        public String CIP
        {
            get
            {
                return _cip;
            }
            set
            {
                _cip = value;
            }
        }

        public String Hostname
        {
            get
            {
                return _hostname;
            }
            set
            {
                _hostname = value;
            }
        }

        public void UpdateHeartbeat()
        {
            _lastheartbeat = DateTime.Now;
        }

        public override bool Equals(object obj)
        {
            if (obj is CThumbprint)
            {
                return (obj as CThumbprint).CIdentifier.Equals(CIdentifier);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return CIdentifier.GetHashCode();
        }
    }
}
