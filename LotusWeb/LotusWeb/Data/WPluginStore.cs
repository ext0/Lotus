using LotusWeb.Data.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LotusWeb.Data
{
    public static class WPluginStore
    {
        public static Plugin GetPluginByName(String name)
        {
            using (PluginContext db = new PluginContext())
            {
                return db.Plugins.Where((x) => x.Name.Equals(name)).FirstOrDefault();
            }
        }
    }
}
