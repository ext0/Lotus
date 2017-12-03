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

        [Controller("/GetMyPlugins/", APIType.POST, "application/json")]
        public static byte[] GetMyPlugins(HttpListenerContext context, String body)
        {
            try
            {
                Cookie cookie = context.Request.Cookies["LOTUS_SESSION_ID"];
                if (cookie != null)
                {
                    using (LotusContext db = new LotusContext())
                    {
                        User user = db.Users.Where(x => x.Sessions.Where(y => y.Cookie.Equals(cookie.Value)).Count() != 0).FirstOrDefault();
                        if (user != null)
                        {
                            Plugin[] plugins = user.EnabledPlugins.ToArray();
                            return Encoding.UTF8.GetBytes(Utility.serializeObjectToJSON(plugins));
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
                return new HTTPErrorMessage("Failed to enable! General error.");
            }
        }

        [Controller("/GetPlugins/", APIType.POST, "application/json")]
        public static byte[] GetPlugins(HttpListenerContext context, String body)
        {
            try
            {
                using (LotusContext db = new LotusContext())
                {
                    Plugin[] plugins = db.Plugins.ToArray();
                    return Encoding.UTF8.GetBytes(Utility.serializeObjectToJSON(plugins));
                }
            }
            catch (Exception e)
            {
                context.Response.StatusCode = 500;
                return new HTTPErrorMessage("Failed to login! General error.");
            }
        }

        [Controller("/TogglePlugin/", APIType.POST, "application/json")]
        public static byte[] TogglePlugin(HttpListenerContext context, String body)
        {
            try
            {
                Cookie cookie = context.Request.Cookies["LOTUS_SESSION_ID"];
                if (cookie != null)
                {
                    using (LotusContext db = new LotusContext())
                    {
                        User user = db.Users.Where(x => x.Sessions.Where(y => y.Cookie.Equals(cookie.Value)).Count() != 0).FirstOrDefault();
                        if (user != null)
                        {
                            dynamic togglePluginRequest = JsonParser.Deserialize(body);
                            String action = togglePluginRequest.Action;
                            dynamic plugin = togglePluginRequest.Plugin;
                            String pluginName = plugin.Name;
                            int pluginVersion = (int)plugin.Version;
                            Plugin match = db.Plugins.Where(x => x.Name.Equals(pluginName) && x.Version.Equals(pluginVersion)).FirstOrDefault();
                            if (match == null)
                            {
                                context.Response.StatusCode = 400;
                                return new HTTPErrorMessage("No plugin exists with this name and version!");
                            }
                            if (action.Equals("ENABLE"))
                            {
                                bool existsAlready = user.EnabledPlugins.Where(x => x.Name.Equals(pluginName) && x.Version.Equals(pluginVersion)).Count() != 0;
                                if (!existsAlready)
                                {
                                    user.EnabledPlugins.Add(match);
                                    db.SaveChanges();
                                    return new byte[] { };
                                }
                                else
                                {
                                    context.Response.StatusCode = 400;
                                    return new HTTPErrorMessage("Plugin already enabled!");
                                }
                            }
                            else if (action.Equals("DISABLE"))
                            {
                                bool existsAlready = user.EnabledPlugins.Where(x => x.Name.Equals(pluginName) && x.Version.Equals(pluginVersion)).Count() != 0;
                                if (existsAlready)
                                {
                                    user.EnabledPlugins.Remove(match);
                                    db.SaveChanges();
                                    return new byte[] { };
                                }
                                else
                                {
                                    context.Response.StatusCode = 400;
                                    return new HTTPErrorMessage("Plugin not enabled!");
                                }
                            }
                            else
                            {
                                context.Response.StatusCode = 400;
                                return new HTTPErrorMessage(action + " is not a valid action!");
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
                return new HTTPErrorMessage("Failed to enable! General error.");
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
                    using (LotusContext db = new LotusContext())
                    {
                        User user = db.Users.Where(x => x.Sessions.Where(y => y.Cookie.Equals(cookie.Value)).Count() != 0).FirstOrDefault();
                        if (user != null)
                        {
                            // the stupid names here are because for some reason Json.JsonObject freaks out with certain strings...
                            dynamic uploadPluginRequest = JsonParser.Deserialize(body);
                            String pluginName = uploadPluginRequest.Name;
                            String pluginDescription = uploadPluginRequest.Description;
                            int pluginVersion = (int)uploadPluginRequest.Version;
                            String absoluteClassPathName = uploadPluginRequest.Lass;
                            byte[] classData = Convert.FromBase64String(uploadPluginRequest.Dass);
                            String template = Encoding.UTF8.GetString(Convert.FromBase64String(uploadPluginRequest.Template));
                            String controllerSource = Encoding.UTF8.GetString(Convert.FromBase64String(uploadPluginRequest.Ontroller));
                            String tabIcon = uploadPluginRequest.Name3;
                            String tabHeader = uploadPluginRequest.Name2;
                            bool updating = uploadPluginRequest.Name4;
                            String pluginAuthor = user.Email;

                            if (pluginName == null || pluginName.Length == 0 || pluginDescription.Length == 0 || absoluteClassPathName == null || absoluteClassPathName.Length == 0 || classData.Length == 0)
                            {
                                context.Response.StatusCode = 400;
                                return new HTTPErrorMessage("Invalid post parameters!");
                            }

                            Plugin exists = db.Plugins.Where((x) => x.Name.Equals(pluginName)).FirstOrDefault();
                            if (exists != null && !updating)
                            {
                                context.Response.StatusCode = 400;
                                return new HTTPErrorMessage("Plugin already exists with that name!");
                            }
                            if (updating)
                            {
                                if (exists != null)
                                {
                                    if (!user.Email.Equals(exists.Uploader))
                                    {
                                        context.Response.StatusCode = 400;
                                        return new HTTPErrorMessage("Cannot update a plugin you don't own!");
                                    }
                                    exists.Description = uploadPluginRequest.Description;
                                    exists.ClassData = classData;
                                    exists.Template = template;
                                    exists.ControllerSource = controllerSource;
                                    exists.TabHeader = tabHeader;
                                    exists.TabIcon = tabIcon;

                                    db.SaveChanges();
                                    return new byte[] { };
                                }
                                else
                                {
                                    context.Response.StatusCode = 400;
                                    return new HTTPErrorMessage("No plugin exists by that name!");
                                }
                            }
                            Plugin plugin = new Plugin(pluginName, pluginDescription, user.Email, pluginVersion, absoluteClassPathName, classData, controllerSource, template, tabHeader, tabIcon);
                            db.Plugins.Add(plugin);
                            db.SaveChanges();
                            return new byte[] { };
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
