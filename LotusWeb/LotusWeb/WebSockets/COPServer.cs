using log4net;
using LotusRoot.LComm.Data;
using LotusWeb.Data;
using LotusWeb.Data.Contexts;
using LotusWeb.Logic.Crypto;
using SaneWeb.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace LotusWeb.WebSockets
{
    public class COPServer : WebSocketBehavior
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(COPServer));

        private COPProcessor _processor;

        protected override void OnClose(CloseEventArgs e)
        {
        }

        protected override void OnError(ErrorEventArgs e)
        {
        }

        protected override void OnOpen()
        {
            _processor = new COPProcessor(this);
            Logger.Info("Opened!");
        }

        public void SendLResponse(LResponse response)
        {
            Send(Utility.serializeObjectToJSON(response));
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            try
            {
                LRequest request = Utility.deserializeJSONToObject<LRequest>(e.Data);
                User user = SessionHub.GetUserFromCookie(request.Auth);
                if (user == null)
                {
                    throw new Exception("Unauthenticated request using auth (" + request.Auth + ")");
                }
                String email = user.Email;
                byte[] hash = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(email));
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hash.Length; i++)
                {
                    sb.Append(hash[i].ToString("x2"));
                }
                String hashText = sb.ToString();
                _processor.Process(request, hashText);
            }
            catch (Exception ex)
            {
                Logger.Warn("Error occured processing socket message : " + ex.Message);
            }
        }
    }
}
