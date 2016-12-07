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
    public static class RegisterController
    {
        [DataBoundView("/Register")]
        public static Object Register(HttpListenerContext context)
        {
            return BaseResources.GetResources(context);
        }

        [Controller("/RegisterUser/", APIType.POST, "application/json")]
        public static byte[] RegisterUser(HttpListenerContext context, String body)
        {
            try
            {
                dynamic registrationRequest = JsonParser.Deserialize(body);
                String email = registrationRequest.Email.Trim();
                String password = registrationRequest.Password;
                String salt = GenerateSalt(4);
                String hash = HashPassword(password, salt);
                using (UserContext db = new UserContext())
                {
                    bool emailExists = db.Users.Where(x => x.Email.Equals(email, StringComparison.InvariantCultureIgnoreCase)).Count() != 0;
                    if (emailExists)
                    {
                        context.Response.StatusCode = 400;
                        return new HTTPErrorMessage("The email address \"" + email + "\" already has an account attached to it!");
                    }
                    User generated = new User(email, hash, salt, context.Request.UserAgent, context.Request.RemoteEndPoint.Address.ToString());
                    db.Users.Add(generated);
                    db.SaveChanges();
                    String cookie = SessionHub.SpawnSession(db, generated);
                    context.Response.SetCookie(new Cookie("LOTUS_SESSION_ID", cookie, "/"));
                }
                return new byte[] { };
            }
            catch (Exception e)
            {
                context.Response.StatusCode = 500;
                return new HTTPErrorMessage("Failed to register! General error.");
            }
        }
    }
}
