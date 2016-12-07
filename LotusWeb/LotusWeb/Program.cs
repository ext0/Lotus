using log4net;
using log4net.Config;
using LotusWeb.Controllers;
using LotusWeb.Logic.RComm;
using LotusWeb.WebSockets;
using SaneWeb.Resources.Attributes;
using SaneWeb.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp.Server;

namespace LotusWeb
{
    class Program
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(Program));
        private static readonly short RLISTENER_PORT = 12581;

        static void Main(string[] args)
        {
            BasicConfigurator.Configure();

            Logger.Info("Starting webserver...");
            SaneServer server = SaneServer.CreateServer(new SaneServerConfiguration(SaneServerConfiguration.SaneServerPreset.DEFAULT, Assembly.GetExecutingAssembly(), "LotusWeb"));
            server.SetErrorHandler(ErrorHandler);

            server.AddController(typeof(HomeController));
            server.AddController(typeof(RegisterController));
            server.AddController(typeof(LogoutController));
            server.AddController(typeof(LoginController));
            server.AddController(typeof(CPanelController));
            server.AddController(typeof(COPController));

            server.Run();
            Logger.Info("Webserver live!");

            RListener.Initialize(12581);
            RListener.Bind();
            RListener.Start();

            Logger.Info("RListener opened on " + RLISTENER_PORT);

            WebSocketServer wssv = new WebSocketServer(IPAddress.Any, 8888);
            wssv.AddWebSocketService<COPServer>("/COP");
            wssv.Start();

            Logger.Info("Websocket server opened!");

            Console.ReadKey();
            server.Stop();
        }

        public static void ErrorHandler(Object sender, SaneErrorEventArgs e)
        {
            Logger.Error("Unexpected error - " + e.Exception.Message);
            e.Propogate = false;
        }
    }
}
