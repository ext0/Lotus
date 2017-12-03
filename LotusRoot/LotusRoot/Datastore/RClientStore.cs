using log4net;
using LotusRoot.Bson;
using LotusRoot.CComm.CData;
using LotusRoot.CComm.TCP;
using LotusRoot.Configuration;
using LotusRoot.LComm.Data;
using LotusRoot.RComm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LotusRoot.Datastore
{
    public static class RClientStore
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(RClientStore));

        private static LRootThumbprintStore _thumbprints = new LRootThumbprintStore();

        private static List<CConnection> _localConnections = new List<CConnection>();

        public static void SaveCThumbprintStore()
        {
            LRootThumbprintStore copy = BsonConvert.DeserializeObject<LRootThumbprintStore>(BsonConvert.SerializeObject(_thumbprints));
            foreach (LRootThumbprintKeyPair keyPair in copy.Thumbprints)
            {
                foreach (CThumbprint thumbprint in keyPair.Thumbprints)
                {
                    thumbprint.Active = false;
                }
            }
            File.WriteAllBytes(ConfigLoader.Config.CThumbprintStoreFilePath, BsonConvert.SerializeObject(copy));
            Logger.Debug("Saved CThumbprintStore successfully!");
        }

        public static void LoadCThumbprintStore()
        {
            if (!File.Exists(ConfigLoader.Config.CThumbprintStoreFilePath))
            {
                Logger.Warn("No CThumbprintStore file found at " + ConfigLoader.Config.CThumbprintStoreFilePath + " (if this is a new root, ignore this warning)");
                return;
            }
            byte[] data = File.ReadAllBytes(ConfigLoader.Config.CThumbprintStoreFilePath);
            _thumbprints = BsonConvert.DeserializeObject<LRootThumbprintStore>(data);
            Logger.Debug("Loaded CThumbprintStore successfully!");
        }

        public static void AddLocalCThumbprint(CConnection connection)
        {
            if (!_thumbprints.ThumbprintStoreContains(LocalRoot.Local))
            {
                _thumbprints.AddRootThumbprint(LocalRoot.Local);
            }
            CThumbprint exists = _thumbprints.GetKeyPair(LocalRoot.Local).Thumbprints.Where((x) => x.Equals(connection.Thumbprint)).FirstOrDefault();

            if (exists != null)
            {
                Logger.Debug("Local CThumbprint reactivated (" + connection.Thumbprint.CIdentifier + ")!");
                // remove/replace old "equal" thumbprint. new one may have new IP, plugins, etc
                _thumbprints.GetKeyPair(LocalRoot.Local).Thumbprints.Remove(exists);
                _thumbprints.GetKeyPair(LocalRoot.Local).Thumbprints.Add(connection.Thumbprint);
                Logger.Info(connection.Thumbprint.InstalledPlugins.Length);
            }
            else
            {
                Logger.Debug("Local CThumbprint added (" + connection.Thumbprint.CIdentifier + ")!");
                _thumbprints.GetKeyPair(LocalRoot.Local).Thumbprints.Add(connection.Thumbprint);
            }
            if (!_localConnections.Contains(connection))
            {
                _localConnections.Add(connection);
            }

            SaveCThumbprintStore();
        }

        public static void AddRemoteCThumbprint(Root root, CThumbprint thumbprint)
        {
            if (!_thumbprints.ThumbprintStoreContains(root))
            {
                _thumbprints.AddRootThumbprint(root);
            }
            CThumbprint exists = _thumbprints.GetKeyPair(root).Thumbprints.Where((x) => x.Equals(thumbprint)).FirstOrDefault();

            if (exists != null && !exists.Active)
            {
                Logger.Debug("Remote CThumbprint reactivated (" + thumbprint.CIdentifier + ")!");
                // remove/replace old "equal" thumbprint. new one may have new IP, plugins, etc
                _thumbprints.GetKeyPair(root).Thumbprints.Remove(exists);
                _thumbprints.GetKeyPair(root).Thumbprints.Add(thumbprint);
            }
            else if (exists == null)
            {
                Logger.Debug("Remote CThumbprint added (" + thumbprint.CIdentifier + ")!");
                _thumbprints.GetKeyPair(root).Thumbprints.Add(thumbprint);
            }

            SaveCThumbprintStore();
        }

        public static void DisableLocalCThumbprint(CThumbprint thumbprint)
        {
            CConnection connection = _localConnections.Where((x) => { return x.Thumbprint.Equals(thumbprint); }).FirstOrDefault();
            if (connection == null)
            {
                Logger.Warn("Tried to remove nonexistant local CConnection!");
            }
            else
            {
                _localConnections.Remove(connection);
            }

            if (_thumbprints.ThumbprintStoreContains(LocalRoot.Local))
            {
                CThumbprint disabling = _thumbprints.GetKeyPair(LocalRoot.Local).Thumbprints.Where((x) => x.Equals(thumbprint)).FirstOrDefault();
                if (disabling != null)
                {
                    disabling.Active = false;
                    Logger.Debug("Disabled local CThumbprint (" + thumbprint.CIdentifier + ")");
                }
                else
                {
                    Logger.Warn("Tried to disable nonexistant local CThumbprint (" + thumbprint.CIdentifier + ")");
                }
            }
        }

        public static CConnection GetConnectionFromCIdentifier(String identifier)
        {
            return _localConnections.Where((x) => { return x.Thumbprint.CIdentifier.Equals(identifier); }).FirstOrDefault();
        }

        public static bool AddInstalledPluginFromCIdentifier(String identifier, LInstalledPlugin installedPlugin)
        {
            CConnection connection = GetConnectionFromCIdentifier(identifier);

            if (connection == null)
            {
                Logger.Warn("Tried to add installed plugin to nonexistant CThumbprint (" + identifier + ")");
                return false;
            }

            List<LInstalledPlugin> installedPlugins = connection.Thumbprint.InstalledPlugins.ToList();
            LInstalledPlugin plugin = installedPlugins.Where(x => x.Equals(installedPlugin)).FirstOrDefault();
            if (plugin != null)
            {
                plugin.Enabled = true;
                SaveCThumbprintStore();
                return true;
            }
            else
            {
                installedPlugins.Add(installedPlugin);
                connection.Thumbprint.InstalledPlugins = installedPlugins.ToArray();
                SaveCThumbprintStore();
                return true;
            }
        }

        public static bool DisableInstalledPluginFromCIdentifier(String identifier, LInstalledPlugin installedPlugin)
        {
            CConnection connection = GetConnectionFromCIdentifier(identifier);

            if (connection == null)
            {
                Logger.Warn("Tried to add installed plugin to nonexistant CThumbprint (" + identifier + ")");
                return false;
            }

            List<LInstalledPlugin> installedPlugins = connection.Thumbprint.InstalledPlugins.ToList();
            LInstalledPlugin plugin = installedPlugins.Where(x => x.Equals(installedPlugin)).FirstOrDefault();
            if (plugin != null)
            {
                installedPlugins.Remove(plugin);
                connection.Thumbprint.InstalledPlugins = installedPlugins.ToArray();
                SaveCThumbprintStore();
                return true;
            }
            else
            {
                Logger.Warn("Tried to disable non installed plugin " + installedPlugin.Name);
                return false;
            }
        }

        public static void DisableRemoteCThumbprint(Root root, CThumbprint thumbprint)
        {
            if (_thumbprints.ThumbprintStoreContains(root))
            {
                CThumbprint disabling = _thumbprints.GetKeyPair(root).Thumbprints.Where((x) => x.Equals(thumbprint)).FirstOrDefault();
                if (disabling != null)
                {
                    disabling.Active = false;
                    Logger.Debug("Disabled CThumbprint (" + thumbprint.CIdentifier + ")");
                }
                else
                {
                    Logger.Warn("Tried to disable nonexistant CThumbprint (" + thumbprint.CIdentifier + ")");
                }
            }
        }

        public static Root FindRootFromCIdentifier(String identifier)
        {
            return _thumbprints.GetRootFromIdentifier(identifier);
        }

        public static List<CThumbprint> LocalThumbprints
        {
            get
            {
                if (_thumbprints.ThumbprintStoreContains(LocalRoot.Local))
                {
                    return _thumbprints.GetKeyPair(LocalRoot.Local).Thumbprints;
                }
                else
                {
                    return new List<CThumbprint>();
                }
            }
        }
    }
}
