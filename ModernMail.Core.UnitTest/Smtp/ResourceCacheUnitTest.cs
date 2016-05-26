using Microsoft.VisualStudio.TestTools.UnitTesting;
using ModernMail.Core.Smtp;
using ModernMail.Core.UnitTest.Smtp.Samples;
using System;
using System.IO;

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
            var payload = new MailPayload(msg);

            var s1 = new StringWriter();
            payload.CopyTo(s1);

            var s2 = new StringWriter();
            payload.CopyTo(s2);
        }

        [TestMethod]
        public void ResourceCache_MessagePrepared_Test()
        {
            using (var cache = new ResourceCache())
            {
                var msg = new InlineMessage();
                var payload = new MailPayload(msg);

                cache.Prepare(msg);
                var s1 = new StringWriter();
                payload.CopyTo(s1);

                cache.Prepare(msg);
                var s2 = new StringWriter();
                payload.CopyTo(s2);

                var r1 = s1.ToString();
                var r2 = s2.ToString();

                Assert.AreEqual(r1, r2);
            }
        }
    }
}
