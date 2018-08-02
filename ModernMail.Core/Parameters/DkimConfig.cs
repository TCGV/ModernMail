using System.Security.Cryptography;

namespace ModernMail.Core.Parameters
{
    public class DkimConfig
    {
        public string Domain { get; set; }

        public string Selector { get; set; }

        public RSAParameters PrivateKey { get; set; }
    }
}
