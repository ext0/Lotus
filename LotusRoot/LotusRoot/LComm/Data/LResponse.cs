using Newtonsoft.Json;
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
        private String _id;

        [JsonConstructor]
        public LResponse(String response, String data, String id)
        {
            _response = response;
            _data = data;
            _id = id;
        }

        public LResponse(String response, String data)
        {
            _response = response;
            _data = data;
            _id = Guid.NewGuid().ToString();
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

        public String ID
        {
            get
            {
                return _id;
            }
        }

        public void OverwriteData(String data)
        {
            _data = data;
        }

        public void OverwriteID(String id)
        {
            _id = id;
        }
    }
}
