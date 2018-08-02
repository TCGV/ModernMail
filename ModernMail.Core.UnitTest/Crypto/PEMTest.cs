using Microsoft.VisualStudio.TestTools.UnitTesting;
using ModernMail.Core.Crypto;
using ModernMail.Core.Parameters;
using System.IO;
using System.Web.Script.Serialization;

namespace ModernMail.Core.UnitTest.Crypto
{
    [TestClass]
    public class PEMTest
    {
        [TestMethod]
        public void ExportPublicKeyTest()
        {
            var json = File.ReadAllText(@"Resources/DkimConfig.json");
            var jss = new JavaScriptSerializer();
            var config = jss.Deserialize<DkimConfig>(json);

            var pubKey = PEM.ExportPublicKey(config.PrivateKey);
            Assert.AreEqual(expected, pubKey);
        }

        string expected = @"-----BEGIN PUBLIC KEY-----
MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQDIke02CZa+Ph/E8zwWhrwWgL2D
oS1//Pi3GI4WRaZglBVBRs9Sq5j3Au362Tb3QUkrM9eorvw9gVjnomFtyMvWQy01
Cw8xRP0sU0D/kO5PmoR33s86BQv9pqbF1Lk9+EYd9yYWRPBnlZZGB0oXuJxnbp7m
yNsd740EwB0f83R2MQIDAQAB
-----END PUBLIC KEY-----
";
    }
}
