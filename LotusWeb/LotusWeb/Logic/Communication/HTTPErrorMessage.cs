using SaneWeb.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LotusWeb.Logic.Communication
{
    public class HTTPErrorMessage
    {
        public String Error { get; set; }
        public HTTPErrorMessage(String message)
        {
            Error = message;
        }

        public static implicit operator String(HTTPErrorMessage errorMessage)
        {
            return errorMessage.GetJSON();
        }

        public static implicit operator byte[] (HTTPErrorMessage errorMessage)
        {
            return Encoding.UTF8.GetBytes(errorMessage.GetJSON());
        }

        public String GetJSON()
        {
            return Utility.serializeObjectToJSON(this);
        }
    }
}
