using System.Net.Mail;

namespace ModernMail.Core.UnitTest.Smtp.Samples
{
    public class HtmlMessage : MailMessage, ISignedMessage
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

        public string GetBodyHash()
        {
            return "v7n/2Jyf8IHZpAuuNjXuSBZ1p8gvmrI40idqOBHKGY4=";
        }

        public string GetDkimSignature()
        {
            return "eGTQZksmZcps6bUIYJcumdUOgnFSwjpASDAJIzpvnSgcbiYREvhLPJBJL5wbYueNvtv3RN4xXjp/JaOo8QEn/XEGcY9/njrV3bTNPZUJu03k9QD+iELilfEhwCzY5aUfn/65QNxB6qKnWlwoIfbTHDc2lqmNwRukZr0wmOdA1gI=";
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
