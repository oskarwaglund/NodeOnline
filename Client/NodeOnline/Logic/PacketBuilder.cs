using System.Collections.Generic;
using System.Text;

namespace NodeOnline.Logic
{
    class PacketBuilder
    {
        public const byte CONNECT = 0;
        public const byte LEAVE = 1;
        public const byte INPUT = 2;
            

        public static byte[] Connect(string name)
        {
            List<byte> packet = new List<byte>();
            packet.Add(CONNECT);
            byte[] nameInBytes = Encoding.ASCII.GetBytes(name);
            packet.AddRange(nameInBytes);

            return packet.ToArray();
        }

        public static byte[] Input(int id, byte mask)
        {
            byte[] packet = new byte[3];
            packet[0] = INPUT;
            packet[1] = (byte)id;
            packet[2] = mask;
            return packet;
        }
    }
}
