using System.Security.Cryptography;

namespace ModernMail.Core.Crypto
{
    public class RSA
    {
        public static RSAParameters GeneratePrivateKey()
        {
            using (var rsa = new RSACryptoServiceProvider())
                return rsa.ExportParameters(true);
        }

        public static RSAParameters ExtractPublicKey(RSAParameters privateKey)
        {
            var publicKey = privateKey;
            publicKey.D = null;
            publicKey.DP = null;
            publicKey.DQ = null;
            publicKey.InverseQ = null;
            publicKey.P = null;
            publicKey.Q = null;
            return publicKey;
        }

        public static byte[] Sign(byte[] unsignedData, RSAParameters privateKey)
        {
            using (var RSAalg = new RSACryptoServiceProvider())
            {
                RSAalg.ImportParameters(privateKey);
                return RSAalg.SignData(unsignedData, new SHA256CryptoServiceProvider());
            }
        }
    }
}