using log4net;
using LotusRoot.Configuration;
using LotusRoot.WComm.TCP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LotusRoot.WComm
{
    public static class WHandler
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(WHandler));
        private static readonly int WEB_RETRY_TIME = 1000 * 30;

        private static WConnection _connection = null;

        public static WConnection WConnection
        {
            get
            {
                return _connection;
            }
        }

        public static void ConnectToWebServer()
        {
            while (_connection == null)
            {
                _connection = WConnection.Connect(ConfigLoader.Config.RemoteWebHost, ConfigLoader.Config.RemoteWebHostPort);
                if (_connection == null)
                {
                    Logger.Warn("Failed to connect to web server at " + ConfigLoader.Config.RemoteWebHost + ":" + ConfigLoader.Config.RemoteWebHostPort + " : Retrying in " + WEB_RETRY_TIME + "ms...");
                    Thread.Sleep(WEB_RETRY_TIME);
                }
            }
            _connection.OpenStream();
        }

        public static void Start()
        {
            _connection.Handshake();

            Timer poller = new Timer(new TimerCallback(_connection.PollConnection));
            poller.Change(0, WConnection.HEARTBEAT_POLL_TIME);
            ThreadPool.QueueUserWorkItem(_connection.KeepOpen, null);
        }

        public static void Reconnect()
        {
            _connection.CloseConnection();
            _connection = null;
            ConnectToWebServer();
            Start();
        }
    }
}
