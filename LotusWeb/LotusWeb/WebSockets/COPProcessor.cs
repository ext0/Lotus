using log4net;
using LotusRoot.Bson;
using LotusRoot.CComm.CData;
using LotusRoot.LComm.Data;
using LotusRoot.RComm;
using LotusWeb.Data;
using LotusWeb.Data.Contexts;
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
            else if (request.Command.Equals("INSTALLPLUGIN"))
            {
                String cIdentifier = request.Parameters[0];
                Plugin plugin = WPluginStore.GetPluginByName(request.Parameters[1]);

                LInstalledPlugin installedPluginDefinition = new LInstalledPlugin(plugin.Name, plugin.Description, plugin.Uploader, plugin.Version, plugin.AbsoluteClassPathName, plugin.ClassData, true);

                request.Parameters[1] = Convert.ToBase64String(BsonConvert.SerializeObject(installedPluginDefinition));

                RConnection connection = WClientStore.GetConnectionFromCIdentifier(cIdentifier);
                LRequest lRequest = new LRequest(authentication, request.Command, true, request.Parameters);
                connection.SendCallbackRequest(lRequest, LMetadata.NOTHING, (response) =>
                {
                    response.OverwriteData(Encoding.UTF8.GetString(Convert.FromBase64String(response.Data)));
                    _server.SendLResponse(request, response);
                });
            }
            else if (request.Command.Equals("DISABLEPLUGIN"))
            {
                String cIdentifier = request.Parameters[0];
                Plugin plugin = Utility.deserializeJSONToObject<Plugin>(request.Parameters[1]);

                LInstalledPlugin installedPluginDefinition = new LInstalledPlugin(plugin.Name, plugin.Description, plugin.Uploader, plugin.Version, plugin.AbsoluteClassPathName, plugin.ClassData, false);
                request.Parameters[1] = Convert.ToBase64String(BsonConvert.SerializeObject(installedPluginDefinition));

                RConnection connection = WClientStore.GetConnectionFromCIdentifier(cIdentifier);
                LRequest lRequest = new LRequest(authentication, request.Command, true, request.Parameters);
                connection.SendCallbackRequest(lRequest, LMetadata.NOTHING, (response) =>
                {
                    response.OverwriteData(Encoding.UTF8.GetString(Convert.FromBase64String(response.Data)));
                    _server.SendLResponse(request, response);
                });
            }
            else
            {
                String cIdentifier = request.Parameters[0];
                RConnection connection = WClientStore.GetConnectionFromCIdentifier(cIdentifier);
                LRequest lRequest = new LRequest(authentication, request.Command, true, request.Parameters);
                connection.SendCallbackRequest(lRequest, LMetadata.NOTHING, (response) =>
                {
                    response.OverwriteData(Encoding.UTF8.GetString(Convert.FromBase64String(response.Data)));
                    _server.SendLResponse(request, response);
                });
                return;
            }
        }
    }
}
