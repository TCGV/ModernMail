using System.Net;

namespace ModernMail.Core.Net.Dns
{
	public class ANameRecord : RecordBase
	{
        public IPAddress IPAddress { get; private set; }

		internal ANameRecord(Pointer pointer)
		{
			byte b1 = pointer.ReadByte();
			byte b2 = pointer.ReadByte();
			byte b3 = pointer.ReadByte();
			byte b4 = pointer.ReadByte();

            IPAddress = IPAddress.Parse(string.Format("{0}.{1}.{2}.{3}", b1, b2, b3, b4));
		}

		public override string ToString()
		{
            return IPAddress.ToString();
		}
	}
}
