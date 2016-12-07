using log4net;
using LotusRoot.Bson;
using LotusRoot.CComm.CData;
using LotusRoot.Datastore;
using LotusRoot.LComm.Data;
using LotusWeb.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LotusWeb.Logic.RComm
{
    public class RCommandProcessor : ILCMDProcessor
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(RCommandProcessor));

        private RConnection _connection;

        public RCommandProcessor(RConnection connection)
        {
            _connection = connection;
        }

        public void Process(LRequest request)
        {
            Logger.Info("Received request (" + request.Command + ")");
            if (request.Command.Equals("ADDCTHUMB"))
            {
                byte[] data = Convert.FromBase64String(request.Parameters[0]);
                CThumbprint thumbprint = BsonConvert.DeserializeObject<CThumbprint>(data);
                WClientStore.AddCThumbprint(_connection.Root, thumbprint);
            }
            else if (request.Command.Equals("REMOVECTHUMB"))
            {
                byte[] data = Convert.FromBase64String(request.Parameters[0]);
                CThumbprint thumbprint = BsonConvert.DeserializeObject<CThumbprint>(data);
                WClientStore.RemoveCThumbprint(_connection.Root, thumbprint);
            }
        }
    }
}
