using ModernMail.Core.Dkim;
using ModernMail.Core.Encoding;
using ModernMail.Core.Parameters;
using System;
using System.IO;
using System.Net.Mail;

namespace ModernMail.Core.Model
{
    public class MailPayload : MailWriter
    {
        public MailPayload(MailMessage message)
            : this(null, message) { }

        public MailPayload(DkimConfig config, MailMessage message)
            : this(config, message, DateTime.UtcNow) { }

        public MailPayload(DkimConfig config, MailMessage message, DateTime signatureDate)
            : base(message)
        {
            this.config = config;
            this.signatureDate = signatureDate;
            WriteContent();
        }

        private void WriteContent()
        {
            if (this.config != null)
                WriteSignedContent();
            else
                WriteUnsignedContent();
        }

        private void WriteSignedContent()
        {
            var content = new MailPayload(GetMessage());

            var dkim = new DkimSigner(this.config);
            var header = dkim.CreateHeader(content.Headers, content.Body, signatureDate);

            WriteHeader(header);
            Write(content);
        }

        private void WriteUnsignedContent()
        {
            WriteHeader(new MailHeader(HeaderName.From, GetMessage().From));

            if (GetMessage().To.Count > 0)
                WriteHeader(new MailHeader(HeaderName.To, GetMessage().To));

            if (GetMessage().CC.Count > 0)
                WriteHeader(new MailHeader(HeaderName.Cc, GetMessage().CC));

            WriteHeader(new MailHeader(HeaderName.Subject, GetMessage().Subject));

            WriteHeader(new MailHeader(HeaderName.MimeVersion, "1.0"));

            if (IsMultipart())
            {
                WriteHeader(HeaderName.ContentType, "multipart/mixed; boundary=\"" + GetMainBoundary() + "\"");
                WriteLine();

                WriteMainBoundary();
            }

            if (HasAlternateViews())
            {
                WriteHeader(HeaderName.ContentType, "multipart/alternative; boundary=\"" + GetSubBoundary() + "\"");
                WriteLine();

                foreach (var view in GetMessage().AlternateViews)
                {
                    WriteSubBoundary();
                    WriteAlternateView(view);
                }
            }

            if (HasAlternateViews())
                WriteSubBoundary();

            WriteBody();

            if (HasAlternateViews())
                WriteSubBoundaryEnd();

            foreach (var attchment in GetMessage().Attachments)
            {
                WriteMainBoundary();
                WriteAttachment(attchment);
            }

            if (IsMultipart() || GetMessage().Attachments.Count > 0)
                WriteMainBoundaryEnd();
        }

        private void WriteBody()
        {
            var contentType = (GetMessage().IsBodyHtml ? "text/html" : "text/plain");
            WriteHeader(HeaderName.ContentType, contentType + "; charset=utf-8");
            WriteHeader(HeaderName.ContentTransferEncoding, "quoted-printable");
            WriteLine();
            WriteLine(QuotedPrintable.Encode(GetMessage().Body));
            WriteLine();
        }

        private void WriteAlternateView(AlternateView view)
        {
            WriteHeader(HeaderName.ContentType, view.ContentType.ToString() + "; charset=utf-8");
            WriteHeader(HeaderName.ContentTransferEncoding, "quoted-printable");
            WriteLine();

            using (var sr = new StreamReader(view.ContentStream))
            {
                while (!sr.EndOfStream)
                    WriteLine(QuotedPrintable.Encode(sr.ReadLine()));
            }

            WriteLine();
        }

        private void WriteAttachment(Attachment attachment)
        {
            WriteHeader(HeaderName.ContentType, attachment.ContentType.ToString());
            WriteHeader(HeaderName.ContentDisposition, GetContentDisposition(attachment));

            if (!string.IsNullOrWhiteSpace(attachment.ContentId))
                WriteHeader(HeaderName.ContentID, "<" + attachment.ContentId + ">");

            WriteHeader(HeaderName.ContentTransferEncoding, "base64");
            WriteLine();

            var s = attachment.ContentStream;
            var read = 0;
            byte[] buff = new byte[4617];
            while ((read = s.Read(buff, 0, buff.Length)) > 0)
            {
                var str = Convert.ToBase64String(buff, 0, read);
                for (int i = 0; i < str.Length; i += 76)
                    WriteLine(str.Substring(i, Math.Min(str.Length - i, 76)));
            }

            WriteLine();
        }

        private bool IsMultipart()
        {
            return HasAlternateViews() || HasAttachments();
        }

        private bool HasAlternateViews()
        {
            return GetMessage().AlternateViews.Count > 0;
        }

        private bool HasAttachments()
        {
            return GetMessage().Attachments.Count > 0;
        }

        private string GetContentDisposition(Attachment attachment)
        {
            var value = "";
            if (attachment.ContentDisposition.Inline)
                value = "inline; ";
            else
                value = "attachment; ";
            value += "filename=\"" + attachment.Name + "\"";
            return value;
        }

        private DkimConfig config;
        private DateTime signatureDate;
    }
}
