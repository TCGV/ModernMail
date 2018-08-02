using System.Security.Cryptography;

namespace ModernMail.Core.Crypto
{
    public class SHA256
    {
        public static byte[] Hash(byte[] data)
        {
            return sha256.ComputeHash(data);
        }

        private static readonly SHA256Managed sha256 = new SHA256Managed();
    }
}