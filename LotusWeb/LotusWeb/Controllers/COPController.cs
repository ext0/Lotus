using LotusWeb.Resources;
using SaneWeb.Resources.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LotusWeb.Controllers
{
    public static class COPController
    {
        [DataBoundView("/COPSocket")]
        public static Object COPSocket(HttpListenerContext context)
        {
            return BaseResources.GetResources(context);
        }
    }
}
