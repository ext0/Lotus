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
        private WConnection _connection;

        public WCommandProcessor(WConnection connection)
        {
            _connection = connection;
        }

        public void Process(LRequest request)
        {

        }
    }
}
