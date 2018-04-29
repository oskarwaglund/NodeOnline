using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NodeOnline.Logic
{
    class GameConnection
    {
        UdpClient mcServer;

        public int Connect(string name, string ip, int port)
        {
            UdpClient client = new UdpClient();
            client.Connect(ip, port);

            byte[] packet = PacketBuilder.Connect(name);
            client.Send(packet, packet.Length);

            IPEndPoint ep = new IPEndPoint(IPAddress.Any, 0);
            byte[] recv = client.Receive(ref ep);
            if(recv.Length != 2 && recv[0] != PacketBuilder.CONNECT)
            {
                throw new Exception("Could not connect to server!");
            }

            return recv[1];
        }

        public void ConnectToMcServer(string ip, int port)
        {
            mcServer = new UdpClient();
            mcServer.Connect(ip, port);
        }

        public byte[] ListenToMcServer()
        {
            IPEndPoint ep = new IPEndPoint(IPAddress.Any, 0);
            return mcServer.Receive(ref ep);
        }
    }
}
