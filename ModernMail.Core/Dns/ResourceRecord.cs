using ModernMail.Core.Net.Dns.Enums;
using System;

namespace ModernMail.Core.Net.Dns
{
    [Serializable]
    public class ResourceRecord
    {
        public string Domain { get; private set; }
        public DnsType Type { get; private set; }
        public DnsClass Class { get; private set; }
        public int Ttl { get; private set; }
        public RecordBase Record { get; private set; }

        internal ResourceRecord(Pointer pointer)
        {
            Domain = pointer.ReadDomain();
            Type = pointer.ReadType();
            Class = (DnsClass)pointer.ReadShort();
            Ttl = pointer.ReadInt();

            int recordLength = pointer.ReadShort();

            switch (Type)
            {
                case DnsType.ANAME: Record = new ANameRecord(pointer); break;
                case DnsType.AAAA: Record = new AAAARecord(pointer); break;
                case DnsType.NS: Record = new NSRecord(pointer); break;
                case DnsType.MX: Record = new MXRecord(pointer, Ttl); break;
                case DnsType.SOA: Record = new SoaRecord(pointer); break;
                default:
                    {
                        pointer.Seek(recordLength);
                        break;
                    }
            }
        }
    }

    [Serializable]
    public class Answer : ResourceRecord
    {
        internal Answer(Pointer pointer) : base(pointer) { }
    }

    [Serializable]
    public class NameServer : ResourceRecord
    {
        internal NameServer(Pointer pointer) : base(pointer) { }
    }

    [Serializable]
    public class AdditionalRecord : ResourceRecord
    {
        internal AdditionalRecord(Pointer pointer) : base(pointer) { }
    }
}