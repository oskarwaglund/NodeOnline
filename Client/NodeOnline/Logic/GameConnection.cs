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
        const byte STATE         = 0x10;
        const byte PLAYER_DATA   = 0x11;

        Socket mcSocket;
        byte[] recvBuffer;
        byte[] gameStateBuffer;
        byte[] playerDataBuffer;

        int bytesInGameStateBuffer;
        int bytesInPlayerDataBuffer;

        UdpClient client;

        bool running;

        Stopwatch watch = new Stopwatch();

        public event EventHandler StateReceived;
        public event EventHandler PlayerDataReceived;

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
            gameStateBuffer = new byte[1000];
            playerDataBuffer = new byte[1000];

            Thread thread = new Thread(new ThreadStart(Listen));
            running = true;
            thread.Start();
        }

        public void Listen()
        {
            EndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
            while (running)
            {
                int bytes = mcSocket.ReceiveFrom(recvBuffer, ref remoteEP);
                switch (recvBuffer[0])
                {
                    case STATE:
                        Array.Copy(recvBuffer, 1, gameStateBuffer, 0, bytes-1);
                        bytesInGameStateBuffer = bytes-1;
                        StateReceived?.Invoke(this, EventArgs.Empty);
                        break;
                    case PLAYER_DATA:
                        Array.Copy(recvBuffer, 1, playerDataBuffer, 0, bytes - 1);
                        bytesInPlayerDataBuffer = bytes-1;
                        PlayerDataReceived?.Invoke(this, EventArgs.Empty);
                        break;
                }
                
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

        public byte[] GetGameStateBuffer(out int numberOfBytes)
        {
            numberOfBytes = bytesInGameStateBuffer;
            return gameStateBuffer;
        }

        public byte[] GetPlayerDataBuffer(out int numberOfBytes)
        {
            numberOfBytes = bytesInPlayerDataBuffer;
            return playerDataBuffer;
        }
    }
}
