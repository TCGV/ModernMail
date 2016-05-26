using Microsoft.VisualStudio.TestTools.UnitTesting;
using ModernMail.Core.Net.Smtp;
using System.IO;
using System.Net.Mail;

namespace ModernMail.Core.UnitTest.Smtp
{
    [TestClass]
    public class SmtpResponseTest
    {
        [TestMethod]
        public void SmtpMultiLineTest()
        {
            var expected = "250-This is a\r\n250 Multi-line response\r\n";
            var result = new SmtpResponse(ToStream(expected));
            Assert.AreEqual(SmtpStatusCode.Ok, result.Status);
            Assert.AreEqual(expected, result.Message);
        }

        [TestMethod]
        public void SmtpSingleLineTest()
        {
            var expected = "220 Single-line response\r\n";
            var result = new SmtpResponse(ToStream(expected));
            Assert.AreEqual(SmtpStatusCode.ServiceReady, result.Status);
            Assert.AreEqual(expected, result.Message);
        }

        public Stream ToStream(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}
