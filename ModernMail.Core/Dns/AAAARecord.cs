using System.Net;

namespace ModernMail.Core.Net.Dns
{
    public class AAAARecord : RecordBase
    {
        public IPAddress IPAddress { get; private set; }

        internal AAAARecord(Pointer pointer)
        {
            byte[] ipv6 = new byte[16];
            for (int i = 0; i < 16; i++)
                ipv6[i] = pointer.ReadByte();

            IPAddress = new IPAddress(ipv6);
        }

        public override string ToString()
        {
            return IPAddress.ToString();
        }
    }
}
