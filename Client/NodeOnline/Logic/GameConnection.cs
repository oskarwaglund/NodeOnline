using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NodeOnline.Logic
{
    class GameConnection
    {
        Socket mcSocket;
        byte[] recvBuffer;
        int bytesInBuffer;

        UdpClient client;

        bool running;

        Stopwatch watch = new Stopwatch();

        public event EventHandler DataReceived;

        public int Connect(string name, string ip, int port)
        {
            client = new UdpClient();
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

        public void ConnectToMcServer(string ip, int port, string localIP)
        {
            mcSocket = new Socket(AddressFamily.InterNetwork,
                SocketType.Dgram,
                ProtocolType.Udp);
            EndPoint localEP = new IPEndPoint(IPAddress.Parse(localIP), port);
            mcSocket.Bind(localEP);

            MulticastOption mcOption = new MulticastOption(IPAddress.Parse(ip), IPAddress.Parse(localIP));
            mcSocket.SetSocketOption(SocketOptionLevel.IP,
                SocketOptionName.AddMembership,
                mcOption);

            recvBuffer = new byte[1000];

            Thread thread = new Thread(new ThreadStart(Listen));
            running = true;
            thread.Start();
        }

        public void Listen()
        {
            EndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
            while (running)
            {
                bytesInBuffer = mcSocket.ReceiveFrom(recvBuffer, ref remoteEP);
                DataReceived?.Invoke(this, EventArgs.Empty);
            }
        }

        public void Stop()
        {
            running = false;
        }

        public void SendInput(int id, byte mask)
        {
            if (mask != 0)
            {
                byte[] packet = PacketBuilder.Input(id, mask);
                client.Send(packet, packet.Length);
            }
        }

        public byte[] GetState(out int numberOfBytes)
        {
            numberOfBytes = bytesInBuffer;
            return recvBuffer;
        }
    }
}
