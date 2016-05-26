using ModernMail.Core.Net.Dns.Enums;
using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace ModernMail.Core.Net.Dns
{
    public sealed class DnsResolver
    {
        private DnsResolver() { }

        public static MXRecord[] MXLookup(string domain, IPAddress dnsServer = null)
        {
            if (domain == null)
                throw new ArgumentNullException("domain");

            DnsRequest request = new DnsRequest();
            request.AddQuestion(new DnsQuestion(domain, DnsType.MX, DnsClass.IN));
            DnsResponse response = Lookup(request, dnsServer);

            if (response == null)
                return null;

            return response.Answers
                .Where(a => a.Record is MXRecord)
                .Select(a => a.Record as MXRecord)
                .ToArray();
        }

        public static DnsResponse Lookup(DnsRequest request, IPAddress dnsServer = null)
        {
            if (request == null) throw new ArgumentNullException("request");

            if (dnsServer == null)
            {
                dnsServer = GetDnsServer();
            }

            IPEndPoint server = new IPEndPoint(dnsServer, _dnsPort);
            byte[] requestMessage = request.GetMessage();
            byte[] responseMessage = UdpTransfer(server, requestMessage);

            return new DnsResponse(responseMessage);
        }

        private static byte[] UdpTransfer(IPEndPoint server, byte[] requestMessage)
        {
            int attempts = 0;

            while (attempts <= _udpRetryAttempts)
            {
                StampMessage(requestMessage);

                var socket = new Socket(server.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
                socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 1000);
                socket.SendTo(requestMessage, requestMessage.Length, SocketFlags.None, server);

                try
                {
                    byte[] responseMessage = new byte[512];
                    socket.Receive(responseMessage);
                    if (responseMessage[0] == requestMessage[0] && responseMessage[1] == requestMessage[1])
                        return responseMessage;
                }
                catch (SocketException)
                {
                    attempts++;
                }
                finally
                {
                    _uniqueId++;
                    socket.Close();
                }
            }

            throw new NoResponseException();
        }

        private static void StampMessage(byte[] requestMessage)
        {
            unchecked
            {
                requestMessage[0] = (byte)(_uniqueId >> 8);
                requestMessage[1] = (byte)_uniqueId;
            }
        }

        private static IPAddress GetDnsServer()
        {
            foreach (var adapter in NetworkInterface.GetAllNetworkInterfaces()
                .Where(x => x.OperationalStatus == OperationalStatus.Up)
                .OrderByDescending(x => x.NetworkInterfaceType == NetworkInterfaceType.Ethernet ||
                    x.NetworkInterfaceType == NetworkInterfaceType.Wireless80211))
            {
                var properties = adapter.GetIPProperties();
                var servers = properties.DnsAddresses;
                if (servers.Count > 0)
                    return servers[0];
            }
            return null;
        }

        const int _dnsPort = 53;
        const int _udpRetryAttempts = 2;
        static int _uniqueId;
    }
}
