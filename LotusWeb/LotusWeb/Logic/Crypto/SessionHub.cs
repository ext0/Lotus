using LotusWeb.Data.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LotusWeb.Logic.Crypto
{
    public static class SessionHub
    {
        public static String SpawnSession(UserContext context, User user)
        {
            using (RandomNumberGenerator rng = new RNGCryptoServiceProvider())
            {
                byte[] tokenData = new byte[32];
                rng.GetBytes(tokenData);
                String token = Convert.ToBase64String(tokenData);
                User.UserLoginSession session = new User.UserLoginSession(token);
                user.Sessions.Add(session);
                context.SaveChanges();
                return session.Cookie;
            }
        }

        public static bool InvalidateSession(String cookie)
        {
            bool deleted = false;
            using (UserContext db = new UserContext())
            {
                IQueryable<User.UserLoginSession> sessions = db.Sessions.Where(x => x.Cookie.Equals(cookie));
                foreach (User.UserLoginSession session in sessions)
                {
                    deleted = true;
                    db.Sessions.Remove(session);
                }
                db.SaveChanges();
            }
            return deleted;
        }

        public static User GetUserFromCookie(String cookie)
        {
            using (UserContext db = new UserContext())
            {
                User user = db.Users.Where(x => x.Sessions.Where(y => y.Cookie.Equals(cookie)).Count() != 0).FirstOrDefault();
                if (user != null)
                {
                    return user;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
