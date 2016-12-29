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

        private WConnection _connection;

        public WCommandProcessor(WConnection connection)
        {
            _connection = connection;
        }

        public void ProcessRequest(LRequest request)
        {
            if (request.Command.Equals("CGETDRIVES"))
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
            }
        }
        public void ProcessResponse(LResponse response)
        {
            _connection.Tracker.FulfillRequest(response);
        }
    }
}
