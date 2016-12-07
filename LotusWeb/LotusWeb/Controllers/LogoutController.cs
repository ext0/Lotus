using LotusWeb.Logic.Communication;
using LotusWeb.Logic.Crypto;
using SaneWeb.Resources.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LotusWeb.Controllers
{
    public static class LogoutController
    {
        [Controller("/Logout/", APIType.POST, "application/json")]
        public static byte[] Logout(HttpListenerContext context, String body)
        {
            Cookie cookie = context.Request.Cookies["LOTUS_SESSION_ID"];
            if (cookie != null)
            {
                bool result = SessionHub.InvalidateSession(cookie.Value);
                if (!result)
                {
                    context.Response.StatusCode = 400;
                    return new HTTPErrorMessage("No active session to logout from!");
                }
            }
            else
            {
                context.Response.StatusCode = 400;
                return new HTTPErrorMessage("No session cookie!");
            }
            return new byte[] { };
        }
    }
}
