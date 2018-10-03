using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace Reversi_Online_Server_1._1
{
    abstract class Server : IDisposable
    {
        private TcpListener tcpListener;
        protected readonly int port;

        private bool active;

        protected List<ClientDialog> clients;

        public Server(int port)
        {
            this.port = port;
        }

        public void Dispose()
        {
            this.Stop();
        }

        public void Start()
        {
            this.clients = new List<ClientDialog>();
            if (!this.active)
            {
                this.tcpListener = new TcpListener(IPAddress.Any, this.port);
                tcpListener.Start();
                BeginAcceptingClients();
                this.active = true;
            }
            else Restart();
        }
        public void Restart()
        {
            Stop();
            Start();
        }
        public void Stop()
        {
            if (this.active)
            {
                for (int i = 0; i < this.clients.Count; i++)
                {
                    clients[i].Dispose();
                }
                this.tcpListener.Stop();
                this.active = false;
            }
        }
        private void BeginAcceptingClients()
        {
            Thread accepting = new Thread(() =>
            {
                while (true)
                {
                    Socket socket = this.tcpListener.AcceptSocket();
                    
                    ClientDialog clientDialog = new ClientDialog(socket);
                    this.clients.Add(clientDialog);
                    /* ClientHandler clientHandler = new ClientHandler(clientDialog);
                     clientHandler.HandleClient();*/
                    Thread handling = new Thread(() =>
                    {
                        HandleClient(clientDialog);
                    });
                    handling.Start();
                }
            });
            accepting.Start();
        }

        protected abstract void HandleClient(ClientDialog clientDialog);

        public int Port => this.port;
        public List<ClientDialog> Clients => this.clients;
        public bool Active => this.active;
    }
}
