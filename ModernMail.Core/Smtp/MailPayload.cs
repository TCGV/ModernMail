using ModernMail.Core.Encoding;
using System;
using System.IO;
using System.Net.Mail;

namespace ModernMail.Core.Smtp
{
    public class MailPayload
    {
        public MailPayload(MailMessage message)
        {
            this.message = message;
            main_boundary = "_d7b560ac_268d_4523_97dc_1d75182138a1__d7b560ac_268d_4523_97dc_1d75182138a1_";
            sub_boundary = "_97bcbcd8_6f1e_4c2e_b185_bf8690de2aea__97bcbcd8_6f1e_4c2e_b185_bf8690de2aea_";
        }

        public void CopyTo(TextWriter x)
        {
            WriteHeaders(x);
            WriteContent(x);
        }

        private void WriteHeaders(TextWriter x)
        {
            x.WriteLine("From: " + GetAddressString(message.From));

            if (message.To.Count > 0)
            {
                x.Write("To: ");
                for (int i = 0; i < message.To.Count; i++)
                    x.Write((i > 0 ? ", " : "") + GetAddressString(message.To[i]));
                x.WriteLine();
            }

            if (message.CC.Count > 0)
            {
                x.Write("Cc: ");
                for (int i = 0; i < message.CC.Count; i++)
                    x.Write((i > 0 ? ", " : "") + GetAddressString(message.CC[i]));
                x.WriteLine();
            }

            x.WriteLine("Subject: " + QuotedPrintable.Inline(message.Subject));

            x.WriteLine("MIME-Version: 1.0");
        }

        private void WriteContent(TextWriter x)
        {
            if (IsMultipart())
            {
                x.WriteLine("Content-Type: multipart/mixed; boundary=\"" + main_boundary + "\"");
                x.WriteLine();

                x.WriteLine("--" + main_boundary);
            }

            if (HasAlternateViews())
            {
                x.WriteLine("Content-Type: multipart/alternative; boundary=\"" + sub_boundary + "\"");
                x.WriteLine();

                foreach (var view in message.AlternateViews)
                {
                    x.WriteLine("--" + sub_boundary);
                    WriteAlternateView(x, view);
                }
            }

            if (HasAlternateViews())
                x.WriteLine("--" + sub_boundary);

            WriteBody(x);

            if (HasAlternateViews())
                x.WriteLine("--" + sub_boundary + "--");

            foreach (var attchment in message.Attachments)
            {
                x.WriteLine("--" + main_boundary);
                WriteAttachment(x, attchment);
            }

            if (IsMultipart())
                x.WriteLine("--" + main_boundary + "--");
        }

        private void WriteBody(TextWriter x)
        {
            var contentType = (message.IsBodyHtml ? "text/html" : "text/plain");
            x.WriteLine("Content-Type: " + contentType + "; charset=utf-8");
            x.WriteLine("Content-Transfer-Encoding: quoted-printable");
            x.WriteLine();
            x.WriteLine(QuotedPrintable.Encode(message.Body));
            x.WriteLine();
        }

        private void WriteAlternateView(TextWriter x, AlternateView view)
        {
            x.WriteLine("Content-Type: " + view.ContentType.ToString() + "; charset=utf-8");
            x.WriteLine("Content-Transfer-Encoding: quoted-printable");
            x.WriteLine();

            using (var sr = new StreamReader(view.ContentStream))
            {
                while (!sr.EndOfStream)
                    x.WriteLine(QuotedPrintable.Encode(sr.ReadLine()));
            }

            x.WriteLine();
        }

        private void WriteAttachment(TextWriter x, Attachment attachment)
        {
            x.WriteLine("Content-Type: " + attachment.ContentType.ToString());

            if (attachment.ContentDisposition.Inline)
                x.Write("Content-Disposition: inline; ");
            else
                x.Write("Content-Disposition: attachment; ");
            x.WriteLine("filename=\"" + attachment.Name + "\"");

            if (!string.IsNullOrWhiteSpace(attachment.ContentId))
                x.WriteLine("Content-ID: <" + attachment.ContentId + ">");

            x.WriteLine("Content-Transfer-Encoding: base64");
            x.WriteLine();

            var s = attachment.ContentStream;
            var read = 0;
            byte[] buff = new byte[4617];
            while ((read = s.Read(buff, 0, buff.Length)) > 0)
            {
                var str = Convert.ToBase64String(buff, 0, read);
                for (int i = 0; i < str.Length; i += 76)
                    x.WriteLine(str.Substring(i, Math.Min(str.Length - i, 76)));
            }

            x.WriteLine();
        }

        private bool IsMultipart()
        {
            return HasAlternateViews() || HasAttachments();
        }

        private bool HasAlternateViews()
        {
            return message.AlternateViews.Count > 0;
        }

        private bool HasAttachments()
        {
            return message.Attachments.Count > 0;
        }

        private string GetAddressString(MailAddress addr)
        {
            var dn = addr.DisplayName;
            dn = (string.IsNullOrWhiteSpace(dn) ? "" :
                QuotedPrintable.Inline(dn) + " ");
            return dn + "<" + addr.Address + ">";
        }

        private string main_boundary;
        private string sub_boundary;
        private MailMessage message;
    }
}
