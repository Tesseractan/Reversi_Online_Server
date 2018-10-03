using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using ReversiSerializableTypes;

namespace Reversi_Online_Server_1._1
{
    class ClientDialog : IDisposable
    {
        private Socket socket;
        private string username;
        private IPEndPoint ipEndPoint;
        private ASCIIEncoding ascii = new ASCIIEncoding();
        private Serializer serializer = new Serializer();
        public ClientDialog(Socket socket)
        {
            if (!socket.Connected) throw new ArgumentException("The socket should be already in connected state!", "socket");
            this.socket = socket;
            this.ipEndPoint = this.socket.RemoteEndPoint as IPEndPoint;
        }
        public void Dispose()
        {
            this.Socket.Close();
            this.Socket.Dispose();
        }
        public void SendBytes(byte[] bytes)
        {
            if (this.socket.Connected) socket.Send(bytes);
        }
        public byte[] ReceiveBytes()
        {
            List<byte> data = new List<byte>();
            do
            {
                if (!this.socket.Connected) return null;
                byte[] bitOctet = new byte[1];
                socket.Receive(bitOctet);
                data.AddRange(bitOctet);
            }
            while (this.socket.Available > 0);
            return data.ToArray();
        }
        public void SendText(string message)
        {
            SendBytes(ascii.GetBytes(message.ToArray()));
        }
        public string ReceiveText()
        {
            return ascii.GetString(ReceiveBytes());
        }
        public void SendObject(object @object)
        {
            SendBytes(this.serializer.Serialize(@object));
        }
        public object ReceiveObject()
        {
            byte[] bytes = ReceiveBytes();
            return this.serializer.Deserialize(bytes);
        }
        public void SendMessage(string title, params (string, object)[] payload)
        {
            Message message = new Message(title, payload);
            SendObject(message);
        }
        public Message ReceiveMessage()
        {
            return (Message)ReceiveObject();
        }
        public Socket Socket => this.socket;
        public string Username
        {
            get { return this.username; }
            set { this.username = value; }
        }
        public bool Authenticated => this.username != null;
        public IPEndPoint IPEndPoint => this.ipEndPoint;
    }
}
