using Microsoft.VisualStudio.TestTools.UnitTesting;
using ModernMail.Core.Encoding;
using ModernMail.Core.Model;
using ModernMail.Core.Parameters;
using ModernMail.Core.UnitTest.Smtp.Samples;
using System;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web.Script.Serialization;

namespace ModernMail.Core.UnitTest.Smtp
{
    [TestClass]
    public class MailPayloadUnitTest
    {
        [TestInitialize]
        public void TestInitialize()
        {
            var json = File.ReadAllText(@"Resources/DkimConfig.json");
            var jss = new JavaScriptSerializer();
            config = jss.Deserialize<DkimConfig>(json);
            signatureDate = new DateTime(2018, 01, 01);
        }

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

        [TestMethod]
        public void MailPayload_PlainTextMessage_DkimSigning_Test()
        {
            AssertDkimHeader(new PlainTextMessage());
        }

        [TestMethod]
        public void MailPayload_HtmlMessage_DkimSigning_Test()
        {
            AssertDkimHeader(new HtmlMessage());
        }

        [TestMethod]
        public void MailPayload_InlineMessage_DkimSigning_Test()
        {
            AssertDkimHeader(new InlineMessage());
        }

        [TestMethod]
        public void MailPayload_AttachmentMessage_DkimSigning_Test()
        {
            AssertDkimHeader(new AttachmentMessage());
        }

        private void AssertPayload(MailMessage msg)
        {
            var payload = new MailPayload(msg);

            Assert.AreEqual(msg.ToString(), payload.Content);
        }

        private void AssertDkimHeader(ISignedMessage msg)
        {
            var payload = new MailPayload(config, msg as MailMessage, signatureDate);

            var dkim = payload
                .Headers
                .Single(h => h.Key.Equals(HeaderName.DkimSignature))
                .Value
                .Split(new[] { " ", Keyword.CRLF }, StringSplitOptions.RemoveEmptyEntries);

            var bh = msg.GetBodyHash();
            var actualBh = dkim.Single(x => x.StartsWith("bh=")).TrimEnd(';');
            Assert.AreEqual(bh, actualBh.Substring(3));

            var b = msg.GetDkimSignature();
            var actualB = dkim.Single(x => x.StartsWith("b=")).TrimEnd(';');
            Assert.AreEqual(b, actualB.Substring(2));
        }

        private DateTime signatureDate;

        private DkimConfig config;
    }
}
