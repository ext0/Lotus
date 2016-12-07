using log4net;
using LotusRoot.LComm.TCP;
using LotusWeb.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LotusWeb.Logic.RComm
{
    public static class RListener
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(RListener));
        public static readonly int MAX_PENDING_TCP_BACKLOG = int.MaxValue;
        public static readonly int ALIVE_POLL_TIME = 1000 * 30;

        private static short _port;
        private static TcpListener _listener;
        private static bool _listening;

        public static bool Listening
        {
            get
            {
                return _listening;
            }
        }

        public static void Initialize(short port)
        {
            _port = port;
            _listener = new TcpListener(IPAddress.Parse("0.0.0.0"), _port);
        }

        public static void Bind()
        {
            _listener.Start(MAX_PENDING_TCP_BACKLOG);
        }

        public static async void Start()
        {
            _listening = true;
            while (_listening)
            {
                TcpClient client = await _listener.AcceptTcpClientAsync().ConfigureAwait(false);
                Logger.Debug("Root (" + client.Client.RemoteEndPoint.ToString() + ") connected!");
                ThreadPool.QueueUserWorkItem(HandleClient, client);
            }
        }

        private static void HandleClient(object state)
        {
            try
            {
                TcpClient client = (TcpClient)state;
                RConnection processor = new RConnection(client);
                processor.OpenStream();
                bool handshakeSuccess = processor.Handshake();
                if (!handshakeSuccess)
                {
                    //SHIT BAIL BAIL BAIL
                    client.Close();
                    return;
                }
                WRootStore.AddRoot(processor.Root);
                Logger.Debug("Root (" + client.Client.RemoteEndPoint.ToString() + ") handshake completed (" + processor.Root.Identifier + ")");
                Timer poller = new Timer(new TimerCallback(processor.PollConnection));
                poller.Change(0, RConnection.HEARTBEAT_POLL_TIME);
                ThreadPool.QueueUserWorkItem(processor.KeepOpen, null);
            }
            catch (Exception e)
            {
                Logger.Error("Unexpected error during root handling process : " + e.Message);
            }
        }
    }
}
