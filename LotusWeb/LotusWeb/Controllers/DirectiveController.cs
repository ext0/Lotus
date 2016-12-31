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
    public static class DirectiveController
    {
        [DataBoundView("/Templates/Overview")]
        public static Object ControlPanel(HttpListenerContext context)
        {
            return BaseResources.GetResources(context);
        }
    }
}
