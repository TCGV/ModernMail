using ModernMail.Core.Crypto;
using ModernMail.Core.Encoding;
using ModernMail.Core.Model;
using ModernMail.Core.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModernMail.Core.Dkim
{
    public class DkimSigner
    {
        DkimSigner()
        {
            SigningAlgorithm = DkimSigningAlgorithm.RSASha256;
            HeaderCanonicalization = DkimCanonicalizationAlgorithm.Relaxed;
            BodyCanonicalization = DkimCanonicalizationAlgorithm.Relaxed;
            Encoding = System.Text.Encoding.UTF8;
        }

        public DkimSigner(DkimConfig config)
            : this()
        {
            Domain = config.Domain;
            Selector = config.Selector;
            privateKey = config.PrivateKey;
        }

        public string Domain { get; private set; }

        public string Selector { get; private set; }

        public DkimSigningAlgorithm SigningAlgorithm { get; private set; }

        public DkimCanonicalizationAlgorithm HeaderCanonicalization { get; private set; }

        public DkimCanonicalizationAlgorithm BodyCanonicalization { get; private set; }

        public System.Text.Encoding Encoding { get; private set; }

        public MailHeader CreateHeader(IEnumerable<MailHeader> headers, string body, DateTime date)
        {
            var _headers = headers
                .Where(h => required_headers.Contains(h.Key.Trim().ToLower()))
                .ToArray();

            TimeSpan t = date -
                         DateTime.SpecifyKind(DateTime.Parse("00:00:00 January 1, 1970"), DateTimeKind.Utc);

            var value = new StringBuilder();

            const string start = " ";
            const string end = ";";

            value.Append("v=1;");

            // algorithm used
            value.Append(start);
            value.Append("a=");
            value.Append(GetAlgorithmName());
            value.Append(end);

            // Canonicalization
            value.Append(start);
            value.Append("c=");
            value.Append(HeaderCanonicalization.ToString().ToLower());
            value.Append('/');
            value.Append(BodyCanonicalization.ToString().ToLower());
            value.Append(end);

            // signing domain
            value.Append(start);
            value.Append("d=");
            value.Append(Domain);
            value.Append(end).Append(Keyword.CRLF);

            // headers to be signed
            value.Append(start);
            value.Append("h=");
            foreach (var header in _headers)
            {
                value.Append(header.Key.ToLower());
                value.Append(':');
            }
            value.Length--;
            value.Append(end).Append(Keyword.CRLF);

            // public key location
            value.Append(start);
            value.Append("q=dns/txt");
            value.Append(end);

            // selector
            value.Append(start);
            value.Append("s=");
            value.Append(Selector);
            value.Append(end);

            // time sent
            value.Append(start);
            value.Append("t=");
            value.Append((int)t.TotalSeconds);
            value.Append(end);

            // hash of body
            value.Append(start);
            value.Append("bh=");
            value.Append(SignBody(body));
            value.Append(end).Append(Keyword.CRLF);

            value.Append(start);
            value.Append("b=");

            var x = new MailHeader(HeaderName.DkimSignature, value.ToString());
            x.Append(SignHeaders(_headers, x));
            return x;
        }

        private string SignHeaders(MailHeader[] headers, MailHeader dkimHeader)
        {
            if (headers == null || headers.Length == 0)
                throw new ArgumentException("headers");

            var canonicalizer = new DkimCanonicalizer();
            var ch = canonicalizer.CanonicalizeHeaders(
                headers.Union(new[] { dkimHeader }).ToArray(),
                HeaderCanonicalization);

            if (SigningAlgorithm != DkimSigningAlgorithm.RSASha256)
                throw new NotImplementedException();

            var data = Encoding.GetBytes(ch.TrimEnd());

            return Convert.ToBase64String(RSA.Sign(data, privateKey));
        }

        private string GetAlgorithmName()
        {
            switch (SigningAlgorithm)
            {
                case DkimSigningAlgorithm.RSASha1:
                    return "rsa-sha1";
                case DkimSigningAlgorithm.RSASha256:
                    return "rsa-sha256";
                default:
                    throw new InvalidOperationException("Invalid SigningAlgorithm");
            }
        }

        private string SignBody(string body)
        {
            var canonicalizer = new DkimCanonicalizer();
            var cb = canonicalizer.CanonicalizeBody(body, BodyCanonicalization);

            var bytes = Encoding.GetBytes(cb);

            if (SigningAlgorithm != DkimSigningAlgorithm.RSASha256)
                throw new InvalidOperationException();

            return Convert.ToBase64String(SHA256.Hash(bytes));
        }

        private System.Security.Cryptography.RSAParameters privateKey;

        private static HashSet<string> required_headers = new HashSet<string>(
            new string[]
            {
                "From",
                "Subject",
                "Date",
                "Message-ID",
                "To",
                "Cc",
                "MIME-Version",
                "Content-Type",
                "Content-Transfer-Encoding",
                "Content-ID",
                "Content-Description",
                "Resent-Date",
                "Resent-From",
                "Resent-Sender", 
                "Resent-To",
                "Resent-Cc",
                "Resent-Message-ID",
                "In-Reply-To",
                "References",
                "List-Id", 
                "List-Help",
                "List-Unsubscribe",
                "List-Subscribe", 
                "List-Post",
                "List-Owner",
                "List-Archive"
            }.Select(s => s.Trim().ToLower())
        );
    }
}
