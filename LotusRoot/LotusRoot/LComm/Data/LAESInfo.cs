using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LotusRoot.LComm.Data
{
    public class LAESInfo
    {
        private String _iv;
        private String _key;

        public LAESInfo(String iv, String key)
        {
            _iv = iv;
            _key = key;
        }

        public String IV
        {
            get
            {
                return _iv;
            }
            set
            {
                _iv = value;
            }
        }

        public String Key
        {
            get
            {
                return _key;
            }
            set
            {
                _key = value;
            }
        }
    }
}
