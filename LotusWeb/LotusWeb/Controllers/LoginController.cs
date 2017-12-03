using Json;
using LotusWeb.Resources;
using SaneWeb.Resources;
using SaneWeb.Resources.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Data.Entity;
using System.Text;
using System.Threading.Tasks;

using static BCrypt.Net.BCrypt;
using LotusWeb.Data.Contexts;
using LotusWeb.Logic.Communication;
using LotusWeb.Logic.Crypto;

namespace LotusWeb.Controllers
{
    public static class LoginController
    {
        [DataBoundView("/Login")]
        public static Object Login(HttpListenerContext context)
        {
            return BaseResources.GetResources(context);
        }

        [Controller("/LoginUser/", APIType.POST, "application/json")]
        public static byte[] LoginUser(HttpListenerContext context, String body)
        {
            try
            {
                dynamic loginRequest = JsonParser.Deserialize(body);
                String email = loginRequest.Email.Trim();
                String password = loginRequest.Password;
                using (LotusContext db = new LotusContext())
                {
                    User user = db.Users.Where(x => x.Email.Equals(email, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                    if (user != null)
                    {
                        String salt = user.Salt;
                        String attempt = HashPassword(password, salt);
                        if (attempt.Equals(user.Hash))
                        {
                            String cookie = SessionHub.SpawnSession(db, user);
                            user.Attempts.Add(new User.UserLoginAttempt(true, context.Request.UserAgent, context.Request.RemoteEndPoint.Address.ToString()));
                            context.Response.SetCookie(new Cookie("LOTUS_SESSION_ID", cookie, "/"));
                            db.SaveChanges();
                            return new byte[] { };
                        }
                        else
                        {
                            context.Response.StatusCode = 400;
                            user.Attempts.Add(new User.UserLoginAttempt(false, context.Request.UserAgent, context.Request.RemoteEndPoint.Address.ToString()));
                            db.SaveChanges();
                            return new HTTPErrorMessage("Incorrect password!");
                        }
                    }
                }
                context.Response.StatusCode = 400;
                return new HTTPErrorMessage("No user registered with that email!");
            }
            catch (Exception e)
            {
                context.Response.StatusCode = 500;
                return new HTTPErrorMessage("Failed to login! General error.");
            }
        }
    }
}
