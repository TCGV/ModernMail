using Microsoft.VisualStudio.TestTools.UnitTesting;
using ModernMail.Core.Model;
using ModernMail.Core.Smtp;
using ModernMail.Core.UnitTest.Smtp.Samples;
using System;

namespace ModernMail.Core.UnitTest.Smtp
{
    [TestClass]
    public class ResourceCacheUnitTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ResourceCache_MessageNotPrepared_Test()
        {
            var msg = new InlineMessage();

            var r1 = new MailPayload(msg).Content;

            var r2 = new MailPayload(msg).Content;
        }

        [TestMethod]
        public void ResourceCache_MessagePrepared_Test()
        {
            using (var cache = new ResourceCache())
            {
                var msg = new InlineMessage();

                cache.Prepare(msg);
                var r1 = new MailPayload(msg).Content;

                cache.Prepare(msg);
                var r2 = new MailPayload(msg).Content;

                Assert.AreEqual(r1, r2);
            }
        }
    }
}
