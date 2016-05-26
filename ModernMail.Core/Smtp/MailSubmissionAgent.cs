using ModernMail.Core.Net.Dns;
using ModernMail.Core.Smtp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;

namespace ModernMail.Core.Net.Smtp
{
    public class MailSubmissionAgent : IDisposable
    {
        static MailSubmissionAgent()
        {
            records = new Dictionary<string, MXRecord>();
            expirations = new Dictionary<string, DateTime>();
        }

        public MailSubmissionAgent(string hostName)
        {
            this.hostName = hostName;
            cache = new ResourceCache();
        }

        public List<Result> Send(MailMessage message)
        {
            var results = new List<Result>();

            foreach (var group in message
                .To
                .Union(message.CC)
                .Union(message.Bcc)
                .Distinct()
                .GroupBy(x => x.Host))
                results.AddRange(Send(message, group));

            return results;
        }

        public void Dispose()
        {
            if (cache != null)
            {
                cache.Dispose();
                cache = null;
            }
        }

        private List<Result> Send(MailMessage message, IGrouping<string, MailAddress> group)
        {
            var results = new List<Result>();

            try
            {
                var mxDomain = ResolveMX(group);
                if (!string.IsNullOrWhiteSpace(mxDomain))
                {
                    Begin(mxDomain);
                    Helo();
                    if (StartSsl())
                        Helo();
                    foreach (var addr in group)
                        results.Add(Send(message, addr));
                    Quit();
                }
                else
                {
                    foreach (var addr in group)
                        results.Add(new Result(delivered: false, recipient: addr));
                }
            }
            catch (Exception exc)
            {
                foreach (var addr in group)
                    if (!results.Any(r => r.Recipient == addr))
                        results.Add(new Result(
                            delivered: false,
                            recipient: addr,
                            channel: channel,
                            exception: exc
                        ));
            }
            finally
            {
                CloseChannel();
            }

            return results;
        }

        private string ResolveMX(IGrouping<string, MailAddress> group)
        {
            lock (records)
            {
                if (!records.ContainsKey(group.Key))
                {
                    var r = DnsResolver.MXLookup(group.Key)
                        .OrderBy(x => x.Preference)
                        .FirstOrDefault();
                    records.Add(group.Key, r);

                    var exp = DateTime.UtcNow + (r != null ? TimeSpan.FromSeconds(r.Ttl) : TimeSpan.FromMinutes(1));
                    expirations.Add(group.Key, exp);
                }

                var mx = records[group.Key];
                if (expirations[group.Key] < DateTime.UtcNow)
                {
                    records.Remove(group.Key);
                    expirations.Remove(group.Key);
                }

                return mx != null ? mx.DomainName : null;
            }
        }

        private void Begin(string mxDomain)
        {
            channel = new SmtpChannel(mxDomain);
            Read(SmtpStatusCode.ServiceReady);
        }

        private void Helo()
        {
            WriteLine("HELO " + hostName);
            var r = Read();
            if (r.Status == SmtpStatusCode.ServiceReady)
                Read(SmtpStatusCode.Ok);
            else if (r.Status != SmtpStatusCode.Ok)
                throw new SmtpException(r.Status, r.Message);
        }

        private bool StartSsl()
        {
            WriteLine("STARTTLS");
            if (Read().Status == SmtpStatusCode.ServiceReady)
                channel.StartTls();
            return channel.UsingSsl;
        }

        private Result Send(MailMessage message, MailAddress addr)
        {
            SmtpResponse response = null;

            WriteLine("MAIL FROM: " + "<" + message.From.Address + ">");
            Read(SmtpStatusCode.Ok);

            WriteLine("RCPT TO: " + "<" + addr.Address + ">");
            response = Read();
            if (response.Status == SmtpStatusCode.Ok)
            {
                WriteLine("DATA ");
                Read(SmtpStatusCode.StartMailInput);
                WritePayload(message);
                WriteLine(".");
                response = Read();
            }
            else
            {
                WriteLine("RSET");
                Read(SmtpStatusCode.ServiceReady, SmtpStatusCode.Ok);
            }

            return new Result(
                delivered: response.Status == SmtpStatusCode.Ok,
                recipient: addr,
                channel: channel,
                response: response
            );
        }

        private void Quit()
        {
            WriteLine("QUIT ");
            Read();
        }

        private void CloseChannel()
        {
            if (channel != null)
            {
                channel.Dispose();
                channel = null;
            }
        }

        private void WritePayload(MailMessage message)
        {
            cache.Prepare(message);
            Write(new MailPayload(message));
        }

        private void WriteLine(string value)
        {
            channel.WriteLine(value);
        }

        private void Write(MailPayload payload)
        {
            payload.CopyTo(channel);
        }

        private void Read(params SmtpStatusCode[] expected)
        {
            var response = Read();
            if (!expected.Contains(response.Status))
                throw new SmtpException(response.Status, response.Message);
        }

        public SmtpResponse Read()
        {
            return channel.Read();
        }

        private string hostName;
        private SmtpChannel channel;
        private ResourceCache cache;

        private static Dictionary<string, MXRecord> records;
        private static Dictionary<string, DateTime> expirations;

        public class Result
        {
            public Result(
                bool delivered,
                MailAddress recipient,
                SmtpChannel channel = null,
                SmtpResponse response = null,
                Exception exception = null
                )
            {
                Delivered = delivered;
                Recipient = recipient;
                if (channel != null)
                {
                    MXDomain = channel.MxDomain;
                    UsingSsl = channel.UsingSsl;
                }
                Response = response;
                Exception = exception;
            }

            public bool Delivered { get; private set; }

            public MailAddress Recipient { get; private set; }

            public string MXDomain { get; set; }

            public bool UsingSsl { get; private set; }

            public SmtpResponse Response { get; private set; }

            public Exception Exception { get; private set; }
        }
    }
}
