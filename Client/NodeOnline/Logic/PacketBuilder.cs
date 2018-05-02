using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace NodeOnline.Logic
{
    class PacketBuilder
    {
        public const byte CONNECT = 0;
        public const byte LEAVE = 1;
        public const byte INPUT = 2;

        public const byte UPDATE_COLOR = 5;
            

        public static byte[] Connect(string name)
        {
            List<byte> packet = new List<byte>();
            packet.Add(CONNECT);
            byte[] nameInBytes = Encoding.ASCII.GetBytes(name);
            packet.AddRange(nameInBytes);

            return packet.ToArray();
        }

        public static byte[] Input(int id, byte[] input)
        {
            byte[] packet = new byte[2 + input.Length];
            packet[0] = INPUT;
            packet[1] = (byte)id;
            Array.Copy(input, 0, packet, 2, input.Length);
            return packet;
        }

        public static byte[] ColorUpdate(int id, Color color)
        {
            byte[] packet = new byte[5];
            packet[0] = UPDATE_COLOR;
            packet[1] = (byte)id;
            packet[2] = color.R;
            packet[3] = color.G;
            packet[4] = color.B;

            return packet;
        }
    }
}
