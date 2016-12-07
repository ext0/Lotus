using LotusRoot.RComm;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LotusRoot.Configuration
{
    public static class ConfigLoader
    {
        private static Config _config;

        public static Config Load(String path = @"C:\Config\config.json")
        {
            String text = File.ReadAllText(path);
            _config = JsonConvert.DeserializeObject<Config>(text);
            return Config;
        }

        public static Config Config
        {
            get
            {
                return _config;
            }
        }
    }
}
