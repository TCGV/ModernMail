using System.Net.Mail;

namespace ModernMail.Core.UnitTest.Smtp.Samples
{
    public class PlainTextMessage : MailMessage, ISignedMessage
    {
        public PlainTextMessage()
        {
            From = new MailAddress("peter.the.second@gmail.com", "Pedro de Alcântara João Carlos Leopoldo Salvador Bibiano Francisco Xavier de Paula Leocádio Miguel Gabriel Rafael Gonzaga de Bragança e Bourbon");
            To.Add(new MailAddress("thomas@gmail.com"));
            Subject = "Plain Text";

            IsBodyHtml = false;
            Body = @"Hi Thomas,

If you believe that truth=beauty, then surely mathematics is the most beautiful branch of philosophy.

Best Regards,
Peter";
        }

        public string GetBodyHash()
        {
            return "Pj7uVt0MLb5CEhDypUVLXCtpXzNtNjbPhZ9gaKenQg8=";
        }

        public string GetDkimSignature()
        {
            return "B3/jV9nFdGoVlEB9kdf2CcTdpTRzjKqbnl3oUxAIe3ZrHgAxGmSyrGz5KIx/d5KUIrEMulyAFvc/dVXNBfj+wICyS6AuLqGsUE2JGob8HLf3FsUotvpPPkEfQtXVIbTCvqBfXI+LRQ+xAXoAlYkjdhE3ZS6nhBkY7XPqQuE9b/E=";
        }

        public override string ToString()
        {
            return @"From: =?utf-8?Q?Pedro de Alc=C3=A2ntara Jo=C3=A3o Carlos Leopoldo Salvador Bibia?=
 =?utf-8?Q?no Francisco Xavier de Paula Leoc=C3=A1dio Miguel Gabriel Rafae?=
 =?utf-8?Q?l Gonzaga de Bragan=C3=A7a e Bourbon?= <peter.the.second@gmail.com>
To: <thomas@gmail.com>
Subject: =?utf-8?Q?Plain Text?=
MIME-Version: 1.0
Content-Type: text/plain; charset=utf-8
Content-Transfer-Encoding: quoted-printable

Hi Thomas,

If you believe that truth=3Dbeauty, then surely mathematics is the most bea=
utiful branch of philosophy.

Best Regards,
Peter

";
        }
    }
}
