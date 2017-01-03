using log4net;
using LotusRoot.Bson;
using LotusRoot.CComm.CData;
using LotusRoot.LComm.Data;
using LotusRoot.RComm;
using LotusWeb.Data;
using LotusWeb.Logic.RComm;
using MongoDB.Bson;
using SaneWeb.Resources;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LotusWeb.WebSockets
{
    public class COPProcessor
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(COPProcessor));
        private static readonly String[] BASIC_NOCACHE_COMMANDS =
        {
            "CLOGOFF",
            "CSHUTDOWN",
            "CRESTART"
        };

        private static readonly String[] BASIC_CACHE_COMMANDS =
        {

        };

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
                _server.SendLResponse(request, response);
                return;
            }
            else if (request.Command.Equals("CGETDRIVES"))
            {
                String cIdentifier = request.Parameters[0];
                RConnection connection = WClientStore.GetConnectionFromCIdentifier(cIdentifier);
                LRequest lRequest = new LRequest(authentication, request.Command, true, cIdentifier);
                connection.SendCallbackRequest(lRequest, LMetadata.NOTHING, (response) =>
                {
                    response.OverwriteData(Encoding.UTF8.GetString(Convert.FromBase64String(response.Data)));
                    _server.SendLResponse(request, response);
                });
                return;
            }
            foreach (String basic in BASIC_NOCACHE_COMMANDS)
            {
                if (request.Command.Equals(basic))
                {
                    String cIdentifier = request.Parameters[0];
                    RConnection connection = WClientStore.GetConnectionFromCIdentifier(cIdentifier);
                    LRequest lRequest = new LRequest(authentication, request.Command, true, cIdentifier);
                    connection.SendCallbackRequest(lRequest, LMetadata.NOTHING, (response) =>
                    {
                        _server.SendLResponse(request, response);
                    });
                    return;
                }
            }
        }
    }
}
