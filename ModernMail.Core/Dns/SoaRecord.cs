
namespace ModernMail.Core.Net.Dns
{
	public class SoaRecord : RecordBase
	{
		internal SoaRecord(Pointer pointer) 
		{
            PrimaryNameServer = pointer.ReadDomain();
            ResponsibleMailAddress = pointer.ReadDomain();
            Serial = pointer.ReadInt();
            Refresh = pointer.ReadInt();
            Retry = pointer.ReadInt();
            Expire = pointer.ReadInt();
            DefaultTtl = pointer.ReadInt();
		}

		public override string ToString()
		{
			return string.Format("primary name server = {0}\nresponsible mail addr = {1}\nserial  = {2}\nrefresh = {3}\nretry   = {4}\nexpire  = {5}\ndefault TTL = {6}",
                PrimaryNameServer,
                ResponsibleMailAddress,
                Serial.ToString(),
                Refresh.ToString(),
                Retry.ToString(),
                Expire.ToString(),
                DefaultTtl.ToString());
        }

        public string PrimaryNameServer { get; private set; }
        public string ResponsibleMailAddress { get; private set; }
        public int Serial { get; private set; }
        public int Refresh { get; private set; }
        public int Retry { get; private set; }
        public int Expire { get; private set; }
        public int DefaultTtl { get; private set; }
	}
}
