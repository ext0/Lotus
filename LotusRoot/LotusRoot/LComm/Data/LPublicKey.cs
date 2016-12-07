using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LotusRoot.LComm.Data
{
    public class LPublicKey
    {
        private String _modulus;
        private String _exponent;

        public LPublicKey(String modulus, String exponent)
        {
            _modulus = modulus;
            _exponent = exponent;
        }

        public String Modulus
        {
            get
            {
                return _modulus;
            }
            set
            {
                _modulus = value;
            }
        }

        public String Exponent
        {
            get
            {
                return _exponent;
            }
            set
            {
                _exponent = value;
            }
        }
    }
}
