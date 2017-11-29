using log4net;
using log4net.Config;
using LotusRoot.Bson;
using LotusRoot.CComm.CData;
using LotusRoot.CComm.TCP;
using LotusRoot.Configuration;
using LotusRoot.Datastore;
using LotusRoot.RComm;
using LotusRoot.RComm.TCP;
using LotusRoot.WComm;
using LotusRoot.WComm.TCP;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LotusRoot
{
    class Program
    {
        /*
            Welcome to Lotus R00t.
            - Patrick Bell
        */
        private static readonly ILog Logger = LogManager.GetLogger(typeof(Program));

        static void Main(string[] args)
        {
            RemotingConfiguration.CustomErrorsMode = CustomErrorsModes.Off;

            BasicConfigurator.Configure();

            Logger.Info("Starting Lotus Root...");

            Config config = ConfigLoader.Load();

            LocalRoot.Local = new Root(config.LocalExternalIP, config.LocalCPorts, config.LocalRPort, config.LocalIdentifier);
            Logger.Info("Configuration loaded!");

            RChannel.Initialize();
            RChannel.OpenRemoteRootInfo();

            foreach (Config.RootEntry entry in config.Roots)
            {
                RHandler.RegisterRoot(entry.IP, entry.RPort);
            }

            Logger.Info("Opened RChannel on " + config.LocalRPort + "!");

            foreach (short cport in config.LocalCPorts)
            {
                CListener listener = new CListener(cport);
                listener.Initialize();
                listener.Bind();
                listener.Start();
                Logger.Info("Bound to CPort " + cport);
            }

            RHandler.BootstrapRootBeacon();

            WHandler.ConnectToWebServer();
            WHandler.Start();

            Console.ReadKey();

            Logger.Info(RHandler.RegisteredRoots.Count + " registered roots");
            Logger.Info(RHandler.LiveRoots.Count + " live roots");
            Logger.Info(RHandler.DeadRoots.Count + " dead roots");

            foreach (Root root in RHandler.LiveRoots)
            {
                Logger.Info(root.Identifier + " [" + String.Join(",", root.CPorts) + "]");
            }

            Console.ReadKey();
        }
    }
}
