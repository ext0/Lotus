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
    public class Plugin
    {
        public Plugin() { }

        public Plugin(String name, String description, String uploader, int version, String absoluteClassPathName, byte[] classData, String controllerSource, String template)
        {
            this.Name = name;
            this.Description = description;
            this.Version = version;
            this.Uploader = uploader;
            this.AbsoluteClassPathName = absoluteClassPathName;
            this.ClassData = classData;
            this.Template = template;
            this.ControllerSource = controllerSource;
        }

        public int Id { get; set; }
        public String Name { get; set; }
        public String Description { get; set; }
        public String Uploader { get; set; }
        public int Version { get; set; }
        public byte[] ClassData { get; set; }
        public String AbsoluteClassPathName { get; set; }

        public String ControllerSource { get; set; }
        public String Template { get; set; }

        public void ModifyForPublicPluginView()
        {
            ClassData = null;
        }
    }

    public class PluginContext : DbContext
    {
        public PluginContext() : base("Lotus")
        {

        }

        public DbSet<Plugin> Plugins { get; set; }
    }
}
