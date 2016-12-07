using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LotusRoot.LComm.Data
{
    public class LRequest
    {
        private String _auth;
        private String _command;
        private String[] _parameters;

        [JsonConstructor]
        public LRequest(String auth, String command, params String[] parameters)
        {
            _auth = auth;
            _command = command;
            _parameters = parameters;
        }

        public String Auth
        {
            get
            {
                return _auth;
            }
        }

        public String Command
        {
            get
            {
                return _command;
            }
        }

        public String[] Parameters
        {
            get
            {
                return _parameters;
            }
        }
    }
}
