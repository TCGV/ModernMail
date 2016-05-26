using ModernMail.Core.Net.Smtp;
using System;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;

namespace ModernMail.Core.Smtp
{
    public class SmtpChannel : TextWriter, IDisposable
    {
        public SmtpChannel(string mxDomain, int timeout = 10000)
        {
            UsingSsl = false;
            MxDomain = mxDomain;
            smtpServ = new TcpClient(mxDomain, 25);
            netStream = smtpServ.GetStream();
            netStream.ReadTimeout = timeout;
        }

        public bool UsingSsl { get; private set; }

        public string MxDomain { get; private set; }

        public override System.Text.Encoding Encoding
        {
            get { return System.Text.Encoding.UTF8; }
        }

        public void StartTls()
        {
            sslStream = new SslStream(netStream);
            sslStream.AuthenticateAsClient(MxDomain);
            UsingSsl = true;
        }

        public override void WriteLine()
        {
            Write(CRLF);
        }

        public override void WriteLine(string value)
        {
            Write(value);
            Write(CRLF);
        }

        public override void Write(string value)
        {
            var bytea = Encoding.GetBytes(value.ToCharArray());
            GetStream().Write(bytea, 0, bytea.Length);
        }

        public SmtpResponse Read()
        {
            return new SmtpResponse(GetStream());
        }

        public override void Close()
        {
            if (netStream != null)
            {
                netStream.Dispose();
                netStream = null;
            }

            if (sslStream != null)
            {
                sslStream.Dispose();
                sslStream = null;
            }

            if (smtpServ != null)
            {
                ((IDisposable)smtpServ).Dispose();
                smtpServ = null;
            }

            base.Close();
        }

        private Stream GetStream()
        {
            return (UsingSsl ? (Stream)sslStream : netStream);
        }

        private TcpClient smtpServ;
        private NetworkStream netStream;
        private SslStream sslStream;

        private static string CRLF = "\r\n";
    }
}
