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
        private CConnection _connection;

        public CCommandProcessor(CConnection connection)
        {
            _connection = connection;
        }

        public void Process(LRequest request)
        {

        }
    }
}
