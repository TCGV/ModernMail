using System.Net.Mail;

namespace ModernMail.Core.UnitTest.Smtp.Samples
{
    public class AttachmentMessage : MailMessage, ISignedMessage
    {
        public AttachmentMessage()
        {
            From = new MailAddress("thomas@gmail.com", "Thomas Edison");
            To.Add(new MailAddress("john.doe@gmail.com", "John Doe"));
            Subject = "PayPal logo";

            IsBodyHtml = true;
            Body = @"<h4>Hi John,</h4>
<br/>
<p>I'm sending to you an image of PayPal's logo. Please take a look at the attachment and feel free to tell me what you think.</p>
<br/>
<br/>
<div>Best Regards,</div>
<span style=""font-style: italic; color:darkorange; margin-bottom:20px"">Thomas</span>";

            var img = new Attachment(@"./Smtp/Attachments/PayPal.png");
            img.ContentId = "ac899a35-0cb5-4141-ae03-8ac66aac37d9";
            Attachments.Add(img);
        }

        public string GetBodyHash()
        {
            return "2NcHOE6Du22EUadoBHmZeMR8Xe+SANWu/yQhdnk7Nok=";
        }

        public string GetDkimSignature()
        {
            return "gKd+HjOsi4PBsI1HlepPkz2LPb3HRa4szVGor8rmBEYqMK9cukL1NyvNVx/Q6oDGS8IN6QLO7OTFoGC70szZI9uxIvZacZ5PKs/frcDjYI3Q7W1clj3eZ+ZyRsvrpmua8QzwHi6g0r24hiwFT1CC1xwOQTZIMwAkLo4FD2OBbIc=";
        }

        public override string ToString()
        {
            return @"From: =?utf-8?Q?Thomas Edison?= <thomas@gmail.com>
To: =?utf-8?Q?John Doe?= <john.doe@gmail.com>
Subject: =?utf-8?Q?PayPal logo?=
MIME-Version: 1.0
Content-Type: multipart/mixed; boundary=""_d7b560ac_268d_4523_97dc_1d75182138a1__d7b560ac_268d_4523_97dc_1d75182138a1_""

--_d7b560ac_268d_4523_97dc_1d75182138a1__d7b560ac_268d_4523_97dc_1d75182138a1_
Content-Type: text/html; charset=utf-8
Content-Transfer-Encoding: quoted-printable

<h4>Hi John,</h4>
<br/>
<p>I'm sending to you an image of PayPal's logo. Please take a look at the =
attachment and feel free to tell me what you think.</p>
<br/>
<br/>
<div>Best Regards,</div>
<span style=3D""font-style: italic; color:darkorange; margin-bottom:20px"">Th=
omas</span>

--_d7b560ac_268d_4523_97dc_1d75182138a1__d7b560ac_268d_4523_97dc_1d75182138a1_
Content-Type: application/octet-stream; name=""./Smtp/Attachments/PayPal.png""
Content-Disposition: attachment; filename=""./Smtp/Attachments/PayPal.png""
Content-ID: <ac899a35-0cb5-4141-ae03-8ac66aac37d9>
Content-Transfer-Encoding: base64

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

--_d7b560ac_268d_4523_97dc_1d75182138a1__d7b560ac_268d_4523_97dc_1d75182138a1_--
";
        }
    }
}
