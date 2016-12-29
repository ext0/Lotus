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
        private bool _asyncCallback;
        private String _id;

        [JsonConstructor]
        public LRequest(String auth, String command, String id, bool asyncCallback, params String[] parameters)
        {
            _auth = auth;
            _command = command;
            _parameters = parameters;
            _id = id;
            _asyncCallback = asyncCallback;
        }

        public LRequest(String auth, String command, bool asyncCallback, params String[] parameters)
        {
            _auth = auth;
            _command = command;
            _parameters = parameters;
            _asyncCallback = asyncCallback;
            _id = Guid.NewGuid().ToString();
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

        public bool ASyncCallback
        {
            get
            {
                return _asyncCallback;
            }
        }

        public String ID
        {
            get
            {
                return _id;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is LRequest)
            {
                LRequest other = obj as LRequest;
                return (_id.Equals(other.ID));
            }
            return false;
        }

        public override int GetHashCode()
        {
            return _id.GetHashCode();
        }

        public override string ToString()
        {
            return _id + ":" + Command;
        }
    }
}
