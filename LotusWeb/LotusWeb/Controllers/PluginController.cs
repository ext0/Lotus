using Json;
using log4net;
using LotusWeb.Data.Contexts;
using LotusWeb.Logic.Communication;
using LotusWeb.Logic.Crypto;
using SaneWeb.Resources;
using SaneWeb.Resources.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LotusWeb.Controllers
{
    public static class PluginController
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(PluginController));

        [Controller("/GetPlugins/", APIType.POST, "application/json")]
        public static byte[] GetPlugins(HttpListenerContext context, String body)
        {
            try
            {
                using (PluginContext db = new PluginContext())
                {
                    Plugin[] plugins = db.Plugins.ToArray();
                    foreach (Plugin plugin in plugins)
                    {
                        plugin.ClassData = null;
                    }
                    return Encoding.UTF8.GetBytes(Utility.serializeObjectToJSON(plugins));
                }
            }
            catch (Exception e)
            {
                context.Response.StatusCode = 500;
                return new HTTPErrorMessage("Failed to login! General error.");
            }
        }

        [Controller("/UploadPlugin/", APIType.POST, "application/json")]
        public static byte[] UploadPlugin(HttpListenerContext context, String body)
        {
            try
            {
                Cookie cookie = context.Request.Cookies["LOTUS_SESSION_ID"];
                if (cookie != null)
                {
                    using (UserContext userDb = new UserContext())
                    {
                        User user = userDb.Users.Where(x => x.Sessions.Where(y => y.Cookie.Equals(cookie.Value)).Count() != 0).FirstOrDefault();
                        if (user != null)
                        {
                            dynamic uploadPluginRequest = JsonParser.Deserialize(body);
                            String pluginName = uploadPluginRequest.Name;
                            String pluginDescription = uploadPluginRequest.Description;
                            int pluginVersion = (int)uploadPluginRequest.Version;
                            String absoluteClassPathName = uploadPluginRequest.Lass;
                            byte[] classData = Convert.FromBase64String(uploadPluginRequest.Dass);
                            String template = uploadPluginRequest.Template;
                            String controllerSource = uploadPluginRequest.Ontroller;

                            String pluginAuthor = user.Email;

                            if (pluginName == null || pluginName.Length == 0 || pluginDescription.Length == 0 || absoluteClassPathName == null || absoluteClassPathName.Length == 0 || classData.Length == 0)
                            {
                                context.Response.StatusCode = 400;
                                return new HTTPErrorMessage("Invalid post parameters!");
                            }

                            using (PluginContext db = new PluginContext())
                            {
                                Plugin plugin = new Plugin(pluginName, pluginDescription, user.Email, pluginVersion, absoluteClassPathName, classData, controllerSource, template);
                                db.Plugins.Add(plugin);
                                db.SaveChanges();
                                return new byte[] { };
                            }
                        }
                        else
                        {
                            context.Response.StatusCode = 400;
                            return new HTTPErrorMessage("No user associated with this session!");
                        }
                    }
                }
                else
                {
                    context.Response.StatusCode = 400;
                    return new HTTPErrorMessage("No session cookie!");
                }
            }
            catch (Exception e)
            {
                context.Response.StatusCode = 500;
                Logger.Error(e.Message + " - " + e.StackTrace);
                return new HTTPErrorMessage("Failed to upload plugin! General error.");
            }
        }
    }
}
