﻿using log4net;
using LotusRoot.Bson;
using LotusRoot.CComm.CData;
using LotusRoot.Datastore;
using LotusRoot.LComm.Data;
using LotusRoot.LComm.TCP;
using LotusRoot.RComm;
using LotusRoot.WComm;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LotusRoot.CComm.TCP
{
    public class CConnection : LConnection
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(CConnection));
        private static readonly LPacket CLIENT_HEARTBEAT_PACKET = new LPacket(new byte[] { }, LMetadata.HEARTBEAT);

        private CThumbprint _thumbprint;
        private LCipher _remoteCipher;

        public CConnection(TcpClient client)
        {
            _client = client;
            _cipher = new LCipher();
            _cmdProcessor = new CCommandProcessor(this);
            _tracker = new LASyncRequestTracker();
        }

        public CThumbprint Thumbprint
        {
            get
            {
                return _thumbprint;
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

                LPacket localAESPacket = new LPacket(_remoteCipher.PEncrypt(BsonConvert.SerializeObject(_cipher.LocalAESInfo)), LMetadata.HANDSHAKE);
                SendPacket(localAESPacket);

                LPacket remoteAESPacket = WaitForResponse();
                byte[] remoteAESPackage = remoteAESPacket.PackagedData;
                LAESInfo remoteAESInfo = BsonConvert.DeserializeObject<LAESInfo>(_cipher.PDecrypt(remoteAESPackage));
                _cipher.LoadRemoteAES(remoteAESInfo);

                LPacket packet = WaitForResponse();
                byte[] packaged = packet.PackagedData;
                byte[] decrypted = _cipher.LocalAESDecrypt(packaged);
                _thumbprint = BsonConvert.DeserializeObject<CThumbprint>(decrypted);
                _thumbprint.Active = true;

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
                    SendPacket(CLIENT_HEARTBEAT_PACKET);
                }
                else
                {
                    Logger.Debug("Poll connection dropped for CThumbprint (" + _thumbprint.CIdentifier + ")!");
                    ((Timer)state).Dispose();
                }
            }
            catch (Exception e)
            {
                Logger.Warn("Failed to poll connection for CThumbprint (" + _thumbprint.CIdentifier + ") : " + e.Message);
            }
        }

        public void CloseConnection()
        {
            try
            {
                _client.Close();
                Logger.Debug("Forcibly closed connection for CThumbprint (" + _thumbprint.CIdentifier + ")");
            }
            catch
            {
                Logger.Info("Could not forcifully kill CConnection (" + Thumbprint.CIdentifier + "), connection already forcibly closed.");
            }

            _ready = false;

            RClientStore.DisableLocalCThumbprint(_thumbprint);

            if (WHandler.WConnection != null && WHandler.WConnection.Ready)
            {
                String base64thumbprint = Convert.ToBase64String(BsonConvert.SerializeObject(Thumbprint));
                LRequest request = new LRequest(null, "DROPCTHUMB", false, base64thumbprint);
                WHandler.WConnection.SendRequest(request, LMetadata.NOTHING);
            }

            foreach (Root root in RHandler.LiveRoots)
            {
                RRemoteInvokable remoteRoot = (RRemoteInvokable)Activator.GetObject(typeof(RRemoteInvokable), "tcp://" + root.Endpoint.ToString() + ":" + root.RPort + "/RRemote");
                remoteRoot.NotifyCDrop(LocalRoot.Local, Thumbprint);
            }
        }
    }
}
