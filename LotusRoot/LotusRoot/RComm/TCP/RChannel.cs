using log4net;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LotusRoot.RComm.TCP
{
    public static class RChannel
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(RChannel));

        private static TcpServerChannel _serverChannel;
        private static TcpClientChannel _clientChannel;

        public static void Initialize()
        {
            IDictionary serverChannelProperties = new Hashtable();
            serverChannelProperties["port"] = LocalRoot.Local.RPort;
            serverChannelProperties["name"] = "ServerChannel";
            _serverChannel = new TcpServerChannel(serverChannelProperties, null, null);

            IDictionary clientChannelProperties = new Hashtable();
            clientChannelProperties["name"] = "ClientChannel";
            _clientChannel = new TcpClientChannel(clientChannelProperties, null);
        }

        public static void OpenRemoteRootInfo()
        {
            RemotingConfiguration.RegisterWellKnownServiceType(typeof(RemoteRoot), "RRemote", WellKnownObjectMode.Singleton);
        }
    }
}
