using System.Collections.Generic;
using System.IO;
using System.Net.Mail;

namespace ModernMail.Core.Model
{
    public abstract class MailWriter
    {
        public MailWriter(MailMessage message)
        {
            this.message = message;
            main_boundary = "_d7b560ac_268d_4523_97dc_1d75182138a1__d7b560ac_268d_4523_97dc_1d75182138a1_";
            sub_boundary = "_97bcbcd8_6f1e_4c2e_b185_bf8690de2aea__97bcbcd8_6f1e_4c2e_b185_bf8690de2aea_";
            
            afterBoundary = false;
            headers = new List<MailHeader>();
            body = new StringWriter();
            content = new StringWriter();
        }

        public IReadOnlyCollection<MailHeader> Headers
        {
            get { return this.headers; }
        }

        public string Body
        {
            get { return body.ToString(); }
        }

        public string Content
        {
            get { return content.ToString(); }
        }

        protected string GetMainBoundary()
        {
            return main_boundary;
        }

        protected string GetSubBoundary()
        {
            return sub_boundary;
        }

        protected void WriteMainBoundary()
        {
            afterBoundary = true;
            WriteLine("--" + main_boundary);
        }

        protected void WriteSubBoundary()
        {
            afterBoundary = true;
            WriteLine("--" + sub_boundary);
        }

        protected void WriteMainBoundaryEnd()
        {
            WriteLine("--" + main_boundary + "--");
        }

        protected void WriteSubBoundaryEnd()
        {
            WriteLine("--" + sub_boundary + "--");
        }

        protected void WriteLine()
        {
            content.WriteLine();
            if (afterBoundary)
                body.WriteLine();
        }

        protected void WriteLine(string value)
        {
            content.WriteLine(value);
            body.WriteLine(value);
        }

        protected void WriteHeader(string key, string value)
        {
            WriteHeader(new MailHeader(key, value));
        }

        protected void WriteHeader(MailHeader h)
        {
            content.WriteLine(h.Key + ": " + h.Value);
            if (afterBoundary)
                body.WriteLine(h.Key + ": " + h.Value);
            else
                headers.Add(h);
        }

        protected void Write(MailWriter segment)
        {
            content.Write(segment.Content);
            body.Write(segment.body);
        }

        protected MailMessage GetMessage()
        {
            return message;
        }

        private bool afterBoundary;
        private string main_boundary;
        private string sub_boundary;

        private List<MailHeader> headers;
        private TextWriter body;
        private TextWriter content;

        private MailMessage message;
    }
}
