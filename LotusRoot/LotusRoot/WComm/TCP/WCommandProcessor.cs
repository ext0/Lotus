using log4net;
using LotusRoot.CComm.TCP;
using LotusRoot.Datastore;
using LotusRoot.LComm.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LotusRoot.WComm.TCP
{
    public class WCommandProcessor : ILCMDProcessor
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(WCommandProcessor));

        private static readonly String[] BASIC_NOCACHE_COMMANDS =
        {
            "CLOGOFF",
            "CSHUTDOWN",
            "CRESTART"
        };

        private static readonly String[] BASIC_CACHE_COMMANDS =
        {
            "CGETDRIVES"
        };

        private WConnection _connection;

        public WCommandProcessor(WConnection connection)
        {
            _connection = connection;
        }

        public void ProcessRequest(LRequest request)
        {
            foreach (String basicCache in BASIC_CACHE_COMMANDS)
            {
                if (request.Command.Equals(basicCache))
                {
                    String cIdentifier = request.Parameters[0];
                    if (WCache.HasCachedValue(cIdentifier + request.Command))
                    {
                        LResponse cached = (LResponse)WCache.GetCachedValue(cIdentifier + request.Command);
                        _connection.SendResponse(cached, LMetadata.CACHE_HIT);
                    }
                    CConnection connection = RClientStore.GetConnectionFromCIdentifier(cIdentifier);
                    if (connection == null)
                    {
                        Logger.Error("CConnection does not exist for identifier " + cIdentifier + "!");
                        return;
                    }
                    connection.SendCallbackRequest(request, LMetadata.NOTHING, (response) =>
                    {
                        WCache.CacheValue(cIdentifier + request.Command, response, WCachePolicy.NORMAL);
                        _connection.SendResponse(response, LMetadata.NOTHING);
                    });
                    return;
                }
            }
            foreach (String basicNoCache in BASIC_NOCACHE_COMMANDS)
            {
                if (request.Command.Equals(basicNoCache))
                {
                    String cIdentifier = request.Parameters[0];
                    CConnection connection = RClientStore.GetConnectionFromCIdentifier(cIdentifier);
                    if (connection == null)
                    {
                        Logger.Error("CConnection does not exist for identifier " + cIdentifier + "!");
                        return;
                    }
                    connection.SendCallbackRequest(request, LMetadata.NOTHING, (response) =>
                    {
                        _connection.SendResponse(response, LMetadata.NOTHING);
                    });
                    return;
                }
            }
        }
        public void ProcessResponse(LResponse response)
        {
            _connection.Tracker.FulfillRequest(response);
        }
    }
}
