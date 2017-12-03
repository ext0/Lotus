using Newtonsoft.Json;
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

            this.EnabledPlugins = new HashSet<Plugin>();
            this.Sessions = new HashSet<UserLoginSession>();

            this.Created = DateTime.Now;
            this.LastLogin = DateTime.Now;

            this.Attempts = new HashSet<UserLoginAttempt>();
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

        [JsonIgnore]
        public virtual ICollection<Plugin> EnabledPlugins { get; set; }

        [JsonIgnore]
        public virtual ICollection<UserLoginAttempt> Attempts { get; set; }

        [JsonIgnore]
        public virtual ICollection<UserLoginSession> Sessions { get; set; }
    }

    public class Plugin
    {
        public Plugin() { }

        public Plugin(String name, String description, String uploader, int version, String absoluteClassPathName, byte[] classData, String controllerSource, String template, String tabHeader, String tabIcon)
        {
            this.Name = name;
            this.Description = description;
            this.Version = version;
            this.Uploader = uploader;
            this.AbsoluteClassPathName = absoluteClassPathName;
            this.ClassData = classData;
            this.Template = template;
            this.ControllerSource = controllerSource;
            this.TabHeader = tabHeader;
            this.TabIcon = tabIcon;
            this.Users = new HashSet<User>();
        }

        public int Id { get; set; }
        public String Name { get; set; }
        public String Description { get; set; }
        public String Uploader { get; set; }
        public int Version { get; set; }

        public String TabHeader { get; set; }
        public String TabIcon { get; set; }

        [JsonIgnore]
        public byte[] ClassData { get; set; }
        public String AbsoluteClassPathName { get; set; }

        public String ControllerSource { get; set; }
        public String Template { get; set; }

        [JsonIgnore]
        public virtual ICollection<User> Users { get; set; }
    }

    public class LotusContext : DbContext
    {
        public LotusContext() : base("Lotus")
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<User.UserLoginSession> Sessions { get; set; }
        public DbSet<User.UserLoginAttempt> Attempts { get; set; }
        public DbSet<Plugin> Plugins { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
