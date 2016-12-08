using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LotusRoot.LComm.Data
{
    public class LResponse
    {
        private String _response;
        private String _data;

        public LResponse(String response, String data)
        {
            _response = response;
            _data = data;
        }

        public String Response
        {
            get
            {
                return _response;
            }
        }

        public String Data
        {
            get
            {
                return _data;
            }
        }
    }
}
