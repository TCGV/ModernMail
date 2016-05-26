using System;
using System.IO;
using System.Net.Mail;

namespace ModernMail.Core.Net.Smtp
{
    public class SmtpResponse
    {
        public SmtpResponse(Stream stream)
        {
            ParseMessage(stream);
            ParseStatus(Message);
        }

        public SmtpStatusCode Status { get; private set; }

        public string Message { get; private set; }

        private void ParseMessage(Stream stream)
        {
            Message = "";
            string line = "";
            var reader = new StreamReader(stream);
            do
            {
                line = reader.ReadLine();
                if (line != null)
                    Message += line + Environment.NewLine;
            }
            while (line == null || line.Length < 4 || line[3] == '-');
        }

        private void ParseStatus(string message)
        {
            int code = 0;
            int.TryParse(message.Substring(0, 3), out code);
            Status = (SmtpStatusCode)code;
        }
    }
}
