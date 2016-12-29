using log4net;
using LotusRoot.Bson;
using LotusRoot.LComm.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace LotusRoot.LComm.TCP
{
    public abstract class LConnection
    {
        public static readonly int RESPONSE_BUFFER_SIZE = 1024;
        public static readonly int HEARTBEAT_POLL_TIME = 1000 * 30;

        private ILog Logger = LogManager.GetLogger(typeof(LConnection));

        protected TcpClient _client;
        protected NetworkStream _stream;
        protected LCipher _cipher;

        protected bool _open;
        protected ILCMDProcessor _cmdProcessor;
        protected bool _ready;

        protected LASyncRequestTracker _tracker;

        public bool IsConnected
        {
            get
            {
                IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();
                TcpConnectionInformation[] tcpConnections = ipProperties.GetActiveTcpConnections().Where(x => x.LocalEndPoint.Equals(_client.Client.LocalEndPoint) && x.RemoteEndPoint.Equals(_client.Client.RemoteEndPoint)).ToArray();
                if (tcpConnections != null && tcpConnections.Length > 0)
                {
                    TcpState stateOfConnection = tcpConnections.First().State;
                    if (stateOfConnection == TcpState.Established)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
        }

        public bool Ready
        {
            get
            {
                return _ready;
            }
        }

        public LASyncRequestTracker Tracker
        {
            get
            {
                return _tracker;
            }
        }

        public void OpenStream()
        {
            _stream = _client.GetStream();
        }

        public void SendRequest(LRequest request, LMetadata metadata)
        {
            byte[] data = BsonConvert.SerializeObject(request);
            byte[] encrypted = _cipher.RemoteAESEncrypt(data);
            LPacket packet = new LPacket(encrypted, metadata | LMetadata.ENCRYPTED | LMetadata.REQUEST);
            SendPacket(packet);
        }

        public void SendResponse(LResponse response, LMetadata metadata)
        {
            byte[] data = BsonConvert.SerializeObject(response);
            byte[] encrypted = _cipher.RemoteAESEncrypt(data);
            LPacket packet = new LPacket(encrypted, metadata | LMetadata.ENCRYPTED | LMetadata.RESPONSE);
            SendPacket(packet);
        }

        public void SendCallbackRequest(LRequest request, LMetadata metadata, Action<LResponse> callback)
        {
            if (!request.ASyncCallback)
            {
                Logger.Error("Tried to send callback-supported request with a LRequest object marked without the ASyncCallback flag! Call will NOT fire.");
                return;
            }
            _tracker.AddLRequest(request, callback);
            SendRequest(request, metadata);
        }

        protected void SendPacket(LPacket packet)
        {
            _stream.Write(packet.Packet, 0, packet.PacketLength);
        }

        public LPacket WaitForResponse()
        {
            byte[] header = new byte[LPacket.LENGTH_LENGTH];
            for (int i = 0; i < header.Length; i++)
            {
                header[i] = (byte)_stream.ReadByte();
            }
            int length = BitConverter.ToInt32(header, 0);
            byte metadata = (byte)_stream.ReadByte(); //awkward... zzzz
            byte[] data = new byte[length];
            int bytesRead = 0;
            while (bytesRead < length - LPacket.METADATA_LENGTH)
            {
                int read = _stream.Read(data, bytesRead, Math.Min(length - bytesRead, RESPONSE_BUFFER_SIZE));
                bytesRead += read;
            }
            return new LPacket(header, metadata, data);
        }
    }
}
