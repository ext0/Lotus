using log4net;
using LotusRoot.Bson;
using LotusRoot.CComm.CData;
using LotusRoot.LComm.Data;
using LotusRoot.LComm.TCP;
using LotusRoot.RComm;
using LotusWeb.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LotusWeb.Logic.RComm
{
    public class RConnection : LConnection
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(LConnection));
        private static readonly LPacket ROOT_HEARTBEAT_PACKET = new LPacket(new byte[] { 0xFF }, LMetadata.HEARTBEAT);

        private Root _root;
        private LCipher _remoteCipher;

        public RConnection(TcpClient client)
        {
            _client = client;
            _cipher = new LCipher();
            _cmdProcessor = new RCommandProcessor(this);
            _tracker = new LASyncRequestTracker();
        }

        public Root Root
        {
            get
            {
                return _root;
            }
        }

        public bool Handshake()
        {
            try
            {
                LPacket publicKeyPacket = WaitForResponse();
                byte[] publicKeyPackage = publicKeyPacket.PackagedData;
                LPublicKey key = BsonConvert.DeserializeObject<LPublicKey>(publicKeyPackage);
                _remoteCipher = new LCipher(key);

                LPacket handshakePacket = new LPacket(BsonConvert.SerializeObject(_cipher.PublicKey), LMetadata.NOTHING);
                SendPacket(handshakePacket);

                LPacket remoteAESPacket = WaitForResponse();
                byte[] remoteAESPackage = remoteAESPacket.PackagedData;
                LAESInfo remoteAESInfo = BsonConvert.DeserializeObject<LAESInfo>(_cipher.PDecrypt(remoteAESPackage));
                _cipher.LoadRemoteAES(remoteAESInfo);

                LPacket localAESPacket = new LPacket(_remoteCipher.PEncrypt(BsonConvert.SerializeObject(_cipher.LocalAESInfo)), LMetadata.HANDSHAKE | LMetadata.ENCRYPTED);
                SendPacket(localAESPacket);

                LPacket rootPacket = WaitForResponse();
                byte[] rootPacketPackage = rootPacket.PackagedData;
                byte[] decrypted = _cipher.LocalAESDecrypt(rootPacketPackage);
                _root = BsonConvert.DeserializeObject<Root>(decrypted);

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
            while (true)
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
                        if (data.Metadata.HasFlag(LMetadata.REQUEST))
                        {
                            LRequest request = BsonConvert.DeserializeObject<LRequest>(packaged);
                            _cmdProcessor.ProcessRequest(request);
                        }
                        else if (data.Metadata.HasFlag(LMetadata.RESPONSE))
                        {
                            LResponse response = BsonConvert.DeserializeObject<LResponse>(packaged);
                            _cmdProcessor.ProcessResponse(response);
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.Error("Unrecognized/unprocessed (possible scary) data in packet " + data.ToString() + " : " + e.Message);
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
                    SendPacket(ROOT_HEARTBEAT_PACKET);
                }
                else
                {
                    Logger.Debug("Poll connection dropped for Root (" + _root.Identifier + ")!");
                    ((Timer)state).Dispose();
                }
            }
            catch (Exception e)
            {
                Logger.Warn("Failed to poll connection for Root (" + _root.Identifier + ") : " + e.Message);
            }
        }

        public void CloseConnection()
        {
            try
            {
                _client.Close();
                Logger.Debug("Forcibly closed connection for Root (" + _root.Identifier + ")");
            }
            catch
            {
                Logger.Info("Could not forcifully kill RConnection (" + _root.Identifier + "), connection already forcibly closed.");
            }
            WRootStore.RemoveRoot(_root);
            WClientStore.DisableAllCThumbprints(_root);
        }
    }
}
