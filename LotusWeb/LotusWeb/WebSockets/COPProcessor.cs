using LotusRoot.CComm.CData;
using LotusRoot.LComm.Data;
using LotusRoot.RComm;
using LotusWeb.Data;
using LotusWeb.Logic.RComm;
using SaneWeb.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LotusWeb.WebSockets
{
    public class COPProcessor
    {
        private COPServer _server;

        public COPProcessor(COPServer server)
        {
            _server = server;
        }

        public void Process(LRequest request, String authentication)
        {
            if (request.Command.Equals("GETCTHUMBS"))
            {
                List<CThumbprint> thumbprints = WClientStore.GetThumbprintsFromAuth(authentication).ToList();
                LResponse response = new LResponse("GETCTHUMBS", Utility.serializeObjectToJSON(thumbprints));
                _server.SendLResponse(response);
            }
        }
    }
}
