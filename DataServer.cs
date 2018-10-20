using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using ReversiSerializableTypes;
using ReversiSerializableTypes.UserData;

namespace Reversi_Online_Server_1._1
{
    class DataServer : Server
    {
        public DataServer(int port) : base(port) { }
        protected override void HandleClient(ClientDialog client)
        {
            
            IPEndPoint remoteIPEndPoint = client.Socket.RemoteEndPoint as IPEndPoint;
            Console.WriteLine($"Connection request from {client.IPEndPoint.Address}:{client.IPEndPoint.Port} has been accepted");
            UserConfiguration userConfiguration = new UserConfiguration(client);
            try
            {
                while (true)
                {
                    Message request = client.ReceiveMessage();
                    switch (request.Title)
                    {
                        case "SIGNUP":
                            userConfiguration.SignUp((EditableUserData)request.Payload["EditableData"]);
                            break;
                        case "LOGIN":
                            userConfiguration.LogIn((LoginUserData)request.Payload["LoginData"]);
                            break;

                        case "DISCONNECT":
                            client.Dispose();
                            throw new SocketException();

                    }
                }
            }
            catch (Exception ex) when (ex is SocketException || ex is NullReferenceException)
            {
                if (client.Authenticated)
                {
                    userConfiguration.MarkOffline();
                }
                this.clients.Remove(client);
                Console.WriteLine($"{client.IPEndPoint.Address}:{client.IPEndPoint.Port} disconnected");
            }
        }
    }
}
