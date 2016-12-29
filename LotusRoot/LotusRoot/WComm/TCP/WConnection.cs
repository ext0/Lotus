using log4net;
using LotusRoot.Bson;
using LotusRoot.CComm.CData;
using LotusRoot.CComm.TCP;
using LotusRoot.Datastore;
using LotusRoot.LComm.Data;
using LotusRoot.LComm.TCP;
using LotusRoot.RComm;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LotusRoot.WComm.TCP
{
    public class WConnection : LConnection
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(WConnection));
        private static readonly LPacket WEB_HEARTBEAT_PACKET = new LPacket(new byte[] { 0xFF }, LMetadata.HEARTBEAT);

        private String _host;
        private int _port;
        private LCipher _remoteCipher;

        public static WConnection Connect(String host, int port)
        {
            try
            {
                WConnection connection = new WConnection(host, port);
                return connection;
            }
            catch (Exception e)
            {
                Logger.Error("Failed to connect to web server at " + host + ":" + port + " : " + e.Message);
                return null;
            }
        }

        private WConnection(String host, int port)
        {
            _host = host;
            _port = port;
            _client = new TcpClient(_host, _port);
            _cipher = new LCipher();
            _open = true;
            _cmdProcessor = new WCommandProcessor(this);
            _tracker = new LASyncRequestTracker();
        }

        public LCipher LocalCipher
        {
            get
            {
                return _cipher;
            }
        }

        public bool Handshake()
        {
            try
            {
                LPacket handshakePacket = new LPacket(BsonConvert.SerializeObject(_cipher.PublicKey), LMetadata.HANDSHAKE);
                SendPacket(handshakePacket);

                LPacket publicKeyPacket = WaitForResponse();
                byte[] publicKeyPackage = publicKeyPacket.PackagedData;
                LPublicKey key = BsonConvert.DeserializeObject<LPublicKey>(publicKeyPackage);
                _remoteCipher = new LCipher(key);

                LPacket localAESPacket = new LPacket(_remoteCipher.PEncrypt(BsonConvert.SerializeObject(_cipher.LocalAESInfo)), LMetadata.HANDSHAKE | LMetadata.ENCRYPTED);
                SendPacket(localAESPacket);

                LPacket remoteAESPacket = WaitForResponse();
                byte[] remoteAESPackage = remoteAESPacket.PackagedData;
                LAESInfo remoteAESInfo = BsonConvert.DeserializeObject<LAESInfo>(_cipher.PDecrypt(remoteAESPackage));
                _cipher.LoadRemoteAES(remoteAESInfo);

                LPacket rootPacket = new LPacket(_cipher.RemoteAESEncrypt(BsonConvert.SerializeObject(LocalRoot.Local)), LMetadata.HANDSHAKE | LMetadata.ENCRYPTED);
                SendPacket(rootPacket);

                _ready = true;

                return true;
            }
            catch (Exception e)
            {
                Logger.Error("Failed to complete handshake! " + e.Message);
                _ready = false;
                return false;
            }
        }

        public void KeepOpen(Object state)
        {
            while (_open)
            {
                try
                {
                    LPacket data = WaitForResponse();
                    if (data.Metadata.HasFlag(LMetadata.HEARTBEAT))
                    {
                        continue;
                    }
                    byte[] packaged = data.PackagedData;
                    if (data.Metadata.HasFlag(LMetadata.ENCRYPTED))
                    {
                        packaged = _cipher.LocalAESDecrypt(packaged);
                    }
                    try
                    {
                        LRequest request = BsonConvert.DeserializeObject<LRequest>(packaged);
                        _cmdProcessor.ProcessRequest(request);
                    }
                    catch (Exception e)
                    {
                        Logger.Error("Unrecognized (possible scary) data in packet " + data.ToString() + " : " + e.Message);
                    }
                }
                catch (ObjectDisposedException)
                {
                    return;
                }
                catch (Exception e)
                {
                    if (!IsConnected)
                    {
                        CloseConnection();
                    }
                    WHandler.Reconnect();
                    Logger.Warn("Generic failure to WaitForResponse : " + e.Message);
                }
            }
        }

        public void PollConnection(Object state)
        {
            try
            {
                if (_client.Connected && IsConnected)
                {
                    SendPacket(WEB_HEARTBEAT_PACKET);
                }
                else
                {
                    Logger.Debug("Poll connection dropped for web connection (" + _host + ":" + _port + ")");
                    ((Timer)state).Dispose();
                }
            }
            catch (Exception e)
            {
                Logger.Warn("Failed to poll connection for web connection (" + _host + ":" + _port + ") : " + e.Message);
            }
        }

        public void CloseConnection()
        {
            try
            {
                _client.Close();
                Logger.Debug("Forcibly closed connection for web connection (" + _host + ":" + _port + ")");
            }
            catch
            {
                Logger.Info("Could not forcifully kill web connection (" + _host + ":" + _port + "), connection already forcibly closed.");
            }
        }
    }
}
