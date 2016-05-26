using System.Net.Mail;

namespace ModernMail.Core.UnitTest.Smtp.Samples
{
    public class HtmlMessage : MailMessage
    {
        public HtmlMessage()
        {
            From = new MailAddress("john.doe@gmail.com", "John Doe");
            To.Add(new MailAddress("thomas@gmail.com", "Thomas Edison"));
            Subject = "Plain Text";

            IsBodyHtml = true;
            Body = @"<h4>Hi Thomas,</h4>
<br/>
<p>This message has an HTML body.</p>
<br/>
<br/>
<div>Best Regards,</div>
<span style=""font-style: italic; color:darkorange; margin-bottom:20px"">Jhon Doe</span>";
        }

        public override string ToString()
        {
            return @"From: =?utf-8?Q?John Doe?= <john.doe@gmail.com>
To: =?utf-8?Q?Thomas Edison?= <thomas@gmail.com>
Subject: =?utf-8?Q?Plain Text?=
MIME-Version: 1.0
Content-Type: text/html; charset=utf-8
Content-Transfer-Encoding: quoted-printable

<h4>Hi Thomas,</h4>
<br/>
<p>This message has an HTML body.</p>
<br/>
<br/>
<div>Best Regards,</div>
<span style=3D""font-style: italic; color:darkorange; margin-bottom:20px"">Jh=
on Doe</span>

";
        }
    }
}
