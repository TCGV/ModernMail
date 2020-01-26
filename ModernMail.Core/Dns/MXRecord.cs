using System;

namespace ModernMail.Core.Net.Dns
{
    [Serializable]
    public class MXRecord : RecordBase
    {
        public string DomainName { get; private set; }

        public int Preference { get; private set; }

        public int Ttl { get; private set; }

        internal MXRecord(Pointer pointer, int ttl)
        {
            Preference = pointer.ReadShort();
            DomainName = pointer.ReadDomain();
            Ttl = ttl;
        }

        public override string ToString()
        {
            return string.Format("Mail Server = {0}, Preference = {1}, TTL = {3}", DomainName, Preference.ToString(), Ttl.ToString());
        }

        public override bool Equals(object obj)
        {
            return obj is MXRecord && Equals((MXRecord)obj);
        }

        public bool Equals(MXRecord that)
        {
            return that != null &&
                DomainName == that.DomainName &&
                Preference == that.Preference &&
                Ttl == that.Ttl;
        }

        public override int GetHashCode()
        {
            return new { DomainName, Preference, Ttl }.GetHashCode();
        }
    }
}
