using Microsoft.VisualStudio.TestTools.UnitTesting;
using ModernMail.Core.Encoding;
using System;

namespace ModernMail.Core.UnitTest.Encoding
{
    [TestClass]
    public class QuotedPrintableTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EncodingNullStringTest()
        {
            QuotedPrintable.Encode(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EncodingInlineNullStringTest()
        {
            QuotedPrintable.Encode(null);
        }

        [TestMethod]
        public void EncodingSingleLineTest()
        {
            var str = "If you believe that truth=beauty, then surely mathematics is the most beautiful branch of philosophy.";
            var expected = @"If you believe that truth=3Dbeauty, then surely mathematics is the most bea=
utiful branch of philosophy.";

            var result = QuotedPrintable.Encode(str);
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void EncodingWithDotStuffingTest()
        {
            var str = "If you believe that truth=beauty, then surely mathematics is the most bea.utiful branch of philosophy.";
            var expected = @"If you believe that truth=3Dbeauty, then surely mathematics is the most bea=
..utiful branch of philosophy.";

            var result = QuotedPrintable.Encode(str);
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void EncodingMultipleLinesTest()
        {
            var str = @"If you believe that truth=beauty,
then surely mathematics
is the most beautiful
branch of philosophy.";
            var expected = @"If you believe that truth=3Dbeauty,
then surely mathematics
is the most beautiful
branch of philosophy.";

            var result = QuotedPrintable.Encode(str);
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void EncodingSpaceAtEndOfLineTest()
        {
            var str = "If you believe that truth=beauty, then surely mathematics is the most be autiful branch of philosophy.";
            var expected = @"If you believe that truth=3Dbeauty, then surely mathematics is the most be =
autiful branch of philosophy.";

            var result = QuotedPrintable.Encode(str);
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void EncodingQuotedCharAtEndOfLineTest()
        {
            var str = "If you believe that truth=beauty, then surely mathematics is the most beãutiful branch of philosophy.";
            var expected = @"If you believe that truth=3Dbeauty, then surely mathematics is the most be=
=C3=A3utiful branch of philosophy.";

            var result = QuotedPrintable.Encode(str);
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void EncodingHTMLTest()
        {
            var str = @"<table width='100%' cellspacing='0' cellpadding='0' align='center' background='https://s3-sa-east-1.amazonaws.com/buckets/images/mail-mkt/page-bg.jpg' bgcolor='#eaeaea'>
    <tbody>
    </tbody>
</table>
<span>© Todos direitos reservados. – Buckets</span>";
            var expected = @"<table width=3D'100%' cellspacing=3D'0' cellpadding=3D'0' align=3D'center' =
background=3D'https://s3-sa-east-1.amazonaws.com/buckets/images/mail-mkt/pa=
ge-bg.jpg' bgcolor=3D'#eaeaea'>
    <tbody>
    </tbody>
</table>
<span>=C2=A9 Todos direitos reservados. =E2=80=93 Buckets</span>";

            var result = QuotedPrintable.Encode(str);
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void EncodingShortStringInlineTest()
        {
            var str = "the quick brown fox jumps over the lazy dog";
            var expected = @"=?utf-8?Q?the quick brown fox jumps over the lazy dog?=";

            var result = QuotedPrintable.Inline(str);
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void EncodingLongStringInlineTest()
        {
            var str = "If you believe that truth=beauty, then surely mathematics is the most beãutiful branch of philosophy.";
            var expected = @"=?utf-8?Q?If you believe that truth=3Dbeauty, then surely mathematics is t?=
 =?utf-8?Q?he most be=C3=A3utiful branch of philosophy.?=";

            var result = QuotedPrintable.Inline(str);
            Assert.AreEqual(expected, result);
        }
    }
}
