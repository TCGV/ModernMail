using ModernMail.Core.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;

namespace ModernMail.Core.Smtp
{
    public class ResourceCache : IDisposable
    {
        public ResourceCache(string fullPath = null)
        {
            temp = new TemporaryDirectory(fullPath);
            attachments = new List<AttachmentBase>();
            files = new HashSet<string>();
        }

        public void Prepare(MailMessage message)
        {
            PrepareAlternateViews(message);
            PrepareAttachments(message);
        }

        public void Dispose()
        {
            if (temp != null)
            {
                foreach (var att in attachments)
                    att.Dispose();

                files.Clear();
                files = null;

                temp.Dispose();
                temp = null;
            }
        }

        private void PrepareAlternateViews(MailMessage message)
        {
            for (int i = 0; i < message.AlternateViews.Count; i++)
            {
                var alt = message.AlternateViews[i];
                alt.ContentId = alt.ContentId ?? Guid.NewGuid().ToString();
                var fullPath = Path.Combine(temp.FullPath, alt.ContentId);

                if (!files.Contains(fullPath))
                {
                    using (var fileStream = File.Create(fullPath))
                        alt.ContentStream.CopyTo(fileStream);
                    files.Add(fullPath);
                }

                using (alt)
                {
                    var aux = new AlternateView(fullPath, alt.ContentType);
                    attachments.Add(aux);
                    aux.ContentId = alt.ContentId;
                    message.AlternateViews[i] = aux;
                }
            }
        }

        private void PrepareAttachments(MailMessage message)
        {
            for (int i = 0; i < message.Attachments.Count; i++)
            {
                var att = message.Attachments[i];
                att.ContentId = att.ContentId ?? Guid.NewGuid().ToString();
                var fullPath = Path.Combine(temp.FullPath, att.ContentId);

                if (!files.Contains(fullPath))
                {
                    using (var fileStream = File.Create(fullPath))
                        att.ContentStream.CopyTo(fileStream);
                    files.Add(fullPath);
                }

                using (att)
                {
                    var aux = new Attachment(fullPath, att.ContentType);
                    attachments.Add(aux);
                    aux.ContentId = att.ContentId;
                    aux.Name = att.Name;
                    message.Attachments[i] = aux;
                }
            }
        }

        private TemporaryDirectory temp;
        private List<AttachmentBase> attachments;
        private HashSet<string> files;
    }
}
