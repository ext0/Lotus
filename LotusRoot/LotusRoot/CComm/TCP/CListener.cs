using log4net;
using LotusRoot.Bson;
using LotusRoot.CComm.CData;
using LotusRoot.Datastore;
using LotusRoot.LComm.Data;
using LotusRoot.RComm;
using LotusRoot.WComm;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LotusRoot.CComm.TCP
{
    public class CListener
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(CListener));
        public static readonly int MAX_PENDING_TCP_BACKLOG = int.MaxValue;
        public static readonly int ALIVE_POLL_TIME = 1000 * 30;

        private short _port;
        private TcpListener _listener;
        private bool _listening;

        public CListener(short port)
        {
            _port = port;
        }

        public bool Listening
        {
            get
            {
                return _listening;
            }
        }

        public void Initialize()
        {
            _listener = new TcpListener(LocalRoot.LOCAL_CAPTURE_ADDRESS, _port);
        }

        public void Bind()
        {
            _listener.Start(MAX_PENDING_TCP_BACKLOG);
        }

        public async void Start()
        {
            _listening = true;
            while (_listening)
            {
                TcpClient client = await _listener.AcceptTcpClientAsync().ConfigureAwait(false);
                Logger.Debug("Client (" + client.Client.RemoteEndPoint.ToString() + ") connected!");
                ThreadPool.QueueUserWorkItem(HandleClient, client);
            }
        }

        private void HandleClient(object state)
        {
            try
            {
                TcpClient client = (TcpClient)state;
                CConnection processor = new CConnection(client);
                processor.OpenStream();
                bool handshakeSuccess = processor.Handshake();
                if (!handshakeSuccess)
                {
                    //SHIT BAIL BAIL BAIL
                    client.Close();
                    return;
                }
                processor.Thumbprint.UpdateHeartbeat();
                processor.Thumbprint.CIP = client.Client.RemoteEndPoint.ToString();
                Logger.Debug("Client (" + client.Client.RemoteEndPoint.ToString() + ") handshake completed (" + processor.Thumbprint.CIdentifier + ")");
                RClientStore.AddLocalCThumbprint(processor);

                if (WHandler.WConnection != null && WHandler.WConnection.Ready)
                {
                    String base64thumbprint = Convert.ToBase64String(BsonConvert.SerializeObject(processor.Thumbprint));
                    LRequest request = new LRequest(null, "ADDCTHUMB", false, base64thumbprint);
                    WHandler.WConnection.SendRequest(request, LMetadata.NOTHING);
                }

                foreach (Root root in RHandler.LiveRoots)
                {
                    RRemoteInvokable remoteRoot = (RRemoteInvokable)Activator.GetObject(typeof(RRemoteInvokable), "tcp://" + root.Endpoint.ToString() + ":" + root.RPort + "/RRemote");
                    remoteRoot.NotifyCConnection(LocalRoot.Local, processor.Thumbprint);
                }

                Timer poller = new Timer(new TimerCallback(processor.PollConnection));
                poller.Change(0, CConnection.HEARTBEAT_POLL_TIME);
                ThreadPool.QueueUserWorkItem(processor.KeepOpen, null);
            }
            catch (Exception e)
            {
                Logger.Error("Unexpected error during client handling process : " + e.Message);
                Logger.Error(e.StackTrace);
            }
        }
    }
}
