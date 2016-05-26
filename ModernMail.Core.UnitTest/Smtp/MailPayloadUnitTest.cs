using Microsoft.VisualStudio.TestTools.UnitTesting;
using ModernMail.Core.Smtp;
using ModernMail.Core.UnitTest.Smtp.Samples;
using System.IO;
using System.Net.Mail;

namespace ModernMail.Core.UnitTest.Smtp
{
    [TestClass]
    public class MailPayloadUnitTest
    {
        [TestMethod]
        public void MailPayload_PlainTextMessage_Test()
        {
            AssertPayload(new PlainTextMessage());
        }

        [TestMethod]
        public void MailPayload_HtmlMessage_Test()
        {
            AssertPayload(new HtmlMessage());
        }

        [TestMethod]
        public void MailPayload_InlineMessage_Test()
        {
            AssertPayload(new InlineMessage());
        }

        [TestMethod]
        public void MailPayload_AttachmentMessage_Test()
        {
            AssertPayload(new AttachmentMessage());
        }

        private static void AssertPayload(MailMessage msg)
        {
            var payload = new MailPayload(msg);
            var sw = new StringWriter();
            payload.CopyTo(sw);
            var result = sw.ToString();

            Assert.AreEqual(msg.ToString(), result);
        }
    }
}
