using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LotusRoot.LComm.Data
{
    public interface ILCMDProcessor
    {
        void Process(LRequest request);
    }
}
