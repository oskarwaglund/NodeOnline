using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;

namespace NodeOnline.Logic
{
    class GameConnection
    {
        const byte STATE         = 0x10;
        const byte PLAYER_DATA   = 0x11;

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

        private int playerId;

        public void Connect(string name, string ip, int port)
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

            recvBuffer = new byte[1000];
            gameStateBuffer = new byte[1000];
            playerDataBuffer = new byte[1000];

            playerId = recv[1];

            Thread thread = new Thread(Listen);
            running = true;
            thread.Start();
        }

        public void Listen()
        {
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
            while (running)
            {
                byte[] recv = client.Receive(ref remoteEP);
                int bufferLength = recv.Length - 1;
                switch (recv[0])
                {
                    case STATE:
                        Array.Copy(recv, 1, gameStateBuffer, 0, bufferLength);
                        bytesInGameStateBuffer = bufferLength;
                        if (StateReceived != null)
                        {
                            StateReceived.Invoke(this, EventArgs.Empty);
                        }
                        break;
                    case PLAYER_DATA:
                        Array.Copy(recv, 1, playerDataBuffer, 0, bufferLength);
                        bytesInPlayerDataBuffer = bufferLength;
                        if (PlayerDataReceived != null)
                        {
                            PlayerDataReceived.Invoke(this, EventArgs.Empty);
                        }
                        break;
                }
                
            }
        }

        public void UpdatePlayerColor(Color color)
        {
            Send(PacketBuilder.ColorUpdate(playerId, color));
        }

        public void SendInput(byte mask)
        {
            if (mask != 0)
            {
                Send(PacketBuilder.Input(playerId, mask));
            }
        }

        public void Send(byte[] packet)
        {
            client.Send(packet, packet.Length);
        }

        public void Stop()
        {
            running = false;
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
