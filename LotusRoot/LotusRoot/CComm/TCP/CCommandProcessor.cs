using log4net;
using LotusRoot.LComm.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LotusRoot.CComm.TCP
{
    public class CCommandProcessor : ILCMDProcessor
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(CCommandProcessor));

        private CConnection _connection;

        public CCommandProcessor(CConnection connection)
        {
            _connection = connection;
        }

        public void ProcessRequest(LRequest request)
        {

        }

        public void ProcessResponse(LResponse response)
        {
            _connection.Tracker.FulfillRequest(response);
        }
    }
}
