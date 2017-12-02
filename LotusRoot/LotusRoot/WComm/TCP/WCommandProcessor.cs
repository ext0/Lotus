using log4net;
using LotusRoot.Bson;
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

        private WConnection _connection;

        public WCommandProcessor(WConnection connection)
        {
            _connection = connection;
        }

        public void ProcessRequest(LRequest request)
        {
            // come back to this caching shit at some point
            /*
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
            */
            String cIdentifier = request.Parameters[0];
            CConnection connection = RClientStore.GetConnectionFromCIdentifier(cIdentifier);
            if (connection == null)
            {
                Logger.Error("CConnection does not exist for identifier " + cIdentifier + "!");
                return;
            }
            if (request.Command.Equals("INSTALLPLUGIN"))
            {
                LInstalledPlugin installedPlugin = BsonConvert.DeserializeObject<LInstalledPlugin>(Convert.FromBase64String(request.Parameters[1]));
                bool success = RClientStore.AddInstalledPluginFromCIdentifier(cIdentifier, installedPlugin);
                if (success)
                {
                    connection.SendCallbackRequest(request, LMetadata.NOTHING, (response) =>
                    {
                        _connection.SendResponse(response, LMetadata.NOTHING);
                    });
                }
            }
            else if (request.Command.Equals("DISABLEPLUGIN"))
            {
                LInstalledPlugin installedPlugin = BsonConvert.DeserializeObject<LInstalledPlugin>(Convert.FromBase64String(request.Parameters[1]));
                bool success = RClientStore.DisableInstalledPluginFromCIdentifier(cIdentifier, installedPlugin);
                if (success)
                {
                    connection.SendCallbackRequest(request, LMetadata.NOTHING, (response) =>
                    {
                        _connection.SendResponse(response, LMetadata.NOTHING);
                    });
                }
            }
            else
            {
                connection.SendCallbackRequest(request, LMetadata.NOTHING, (response) =>
                {
                    _connection.SendResponse(response, LMetadata.NOTHING);
                });
            }
        }
        public void ProcessResponse(LResponse response)
        {
            _connection.Tracker.FulfillRequest(response);
        }
    }
}
