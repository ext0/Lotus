using SaneWeb.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LotusWeb.Data.Contexts
{
    public class User
    {
        public User() { }

        public User(String email, String hash, String salt, String userAgent, String IP)
        {
            this.Email = email;
            this.Hash = hash;
            this.Salt = salt;

            this.Sessions = new List<UserLoginSession>();

            this.Created = DateTime.Now;
            this.LastLogin = DateTime.Now;

            this.Attempts = new List<UserLoginAttempt>();
            this.Attempts.Add(new UserLoginAttempt(true, userAgent, IP));
        }

        public class UserLoginAttempt
        {
            public UserLoginAttempt() { }

            public UserLoginAttempt(bool success, String userAgent, String IP)
            {
                this.Success = success;
                this.UserAgent = userAgent;
                this.IP = IP;
                this.When = DateTime.Now;
            }

            public int Id { get; set; }

            public bool Success { get; set; }
            public DateTime When { get; set; }
            public String UserAgent { get; set; }
            public String IP { get; set; }
        }

        public class UserLoginSession
        {
            public UserLoginSession() { }

            public UserLoginSession(String cookie)
            {
                this.Cookie = cookie;
                this.Started = DateTime.Now;
            }

            public int Id { get; set; }
            public String Cookie { get; set; }
            public DateTime Started { get; set; }
        }

        public int Id { get; set; }
        public String Email { get; set; }
        public String Hash { get; set; }
        public String Salt { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastLogin { get; set; }

        public virtual IList<UserLoginAttempt> Attempts { get; set; }

        public virtual IList<UserLoginSession> Sessions { get; set; }
    }

    public class UserContext : DbContext
    {
        public UserContext() : base("Lotus")
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<User.UserLoginSession> Sessions { get; set; }
        public DbSet<User.UserLoginAttempt> Attempts { get; set; }
    }
}
