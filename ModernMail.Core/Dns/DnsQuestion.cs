using ModernMail.Core.Net.Dns.Enums;
using System;
using System.Text.RegularExpressions;

namespace ModernMail.Core.Net.Dns
{
    [Serializable]
    public class DnsQuestion
    {
        internal DnsQuestion(Pointer pointer)
        {
            Domain = pointer.ReadDomain();
            Type = pointer.ReadType();
            Class = pointer.ReadClass();
        }

        public DnsQuestion(string domain, DnsType dnsType, DnsClass dnsClass)
        {
            if (domain == null) throw new ArgumentNullException("domain");

            if (domain.Length == 0 || domain.Length > 255 || !Regex.IsMatch(domain, @"^[a-zA-Z0-9-_]{1,63}(\.[a-zA-Z0-9-_]{1,63})+$"))
            {
                throw new ArgumentException("The supplied domain name was not in the correct form", "domain");
            }

            if (!Enum.IsDefined(typeof(DnsType), dnsType))
            {
                throw new ArgumentOutOfRangeException("dnsType", "Not a valid value");
            }

            if (!Enum.IsDefined(typeof(DnsClass), dnsClass) || dnsClass == DnsClass.None)
            {
                throw new ArgumentOutOfRangeException("dnsClass", "Not a valid value");
            }

            Domain = domain;
            Type = dnsType;
            Class = dnsClass;
        }

        public string Domain { get; private set; }
        public DnsType Type { get; private set; }
        public DnsClass Class { get; private set; }
    }
}
