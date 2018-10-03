using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReversiSerializableTypes;
using ReversiSerializableTypes.UserData;
using System.Net.Sockets;

namespace Reversi_Online_Server_1._1
{
    class GameServer : Server
    {
        public GameServer(int port) : base(port)
        {
            
        }
        protected override void HandleClient(ClientDialog client)
        {
            try {
                while (true)
                {
                    Message request = client.ReceiveMessage();
                    switch (request.Title)
                    {
                        case "AUTHENTICATE":
                            if (UserConfiguration.LoginDataMatch((LoginUserData)request.Payload["LoginData"]))
                            {
                                client.Username = ((LoginUserData)request.Payload["LoginData"]).Username;
                            }
                            break;
                        case "MESSAGE":
                            string[] receivers_usernames = (string[])request.Payload["Receivers"];
                            ClientDialog[] receivers = new ClientDialog[receivers_usernames.Length];
                            for (int i = 0; i < receivers.Length; i++)
                            {
                                for (int j = 0; j < this.clients.Count; j++)
                                {
                                    if (receivers_usernames[i] == this.clients[j].Username)
                                    {
                                        this.clients[j].SendMessage("MESSAGE", ("Sender", client.Username), ("Content", request.Payload["Message"]));
                                    }
                                }
                            }
                            break;
                        case "DISCONNECT":
                            client.Dispose();
                            throw new SocketException();
                    }
                }
            }
            catch (SocketException) {
                this.clients.Remove(client);
            }
        }
    }
}
