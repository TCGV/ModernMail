using System;
using System.Text;
using System.Text.RegularExpressions;

namespace ModernMail.Core.Encoding
{
    public class QuotedPrintable
    {
        QuotedPrintable()
        {
            sb = new StringBuilder();
            lineNumber = 0;
            lineWidth = 0;
            maxWidth = 76;
            lineBreak = "=";
            buff = new char[1];
        }

        public static string Inline(string str)
        {
            if (str == null)
                throw new ArgumentNullException();

            var qp = new QuotedPrintable();
            qp.inline = true;
            qp.lineBreak = "?=";

            qp.NewLine();
            qp.EncodeLine(str);
            qp.AppendSuffix();

            return qp.ToString();
        }

        public static string Encode(string str)
        {
            if (str == null)
                throw new ArgumentNullException();

            var qp = new QuotedPrintable();

            foreach (var line in Regex.Split(str, "\r\n|\r|\n"))
            {
                qp.NewLine();
                qp.EncodeLine(line);
            }

            return qp.ToString();
        }

        public override string ToString()
        {
            return sb.ToString();
        }

        private void EncodeLine(string line)
        {
            foreach (var c in line)
            {
                if (IsPrintable(c))
                    AppendPrintable(c);
                else
                    AppendNonPrintable(c);
            }
        }

        private void AppendPrintable(char c)
        {
            if (WillBreakLine(1))
                SoftLine();
            if (RequiresDotStuffing(c))
                Append(c);
            Append(c);
        }

        private void AppendNonPrintable(char c)
        {
            var a = Encode(c);
            if (WillBreakLine(a.Length * 3))
                SoftLine();
            Append(a);
        }

        private void NewLine()
        {
            if (lineNumber++ > 0)
                sb.Append(Keyword.CRLF);
            lineWidth = 0;
            AppendPrefix();
        }

        private void SoftLine()
        {
            AppendSuffix();
            Append(Keyword.CRLF);
            lineNumber++;
            lineWidth = 0;
            AppendPrefix();
        }

        private void AppendPrefix()
        {
            if (inline)
            {
                if (lineNumber > 1)
                {
                    Append(' ');
                }
                Append("=?utf-8?Q?");
            }
        }

        private void AppendSuffix()
        {
            if (!string.IsNullOrWhiteSpace(lineBreak))
                Append(lineBreak);
        }

        private void Append(byte[] a)
        {
            foreach (var b in a)
                Append('=').Append(b.ToString("X2"));
        }

        private QuotedPrintable Append(string str)
        {
            sb.Append(str);
            lineWidth += str.Length;
            return this;
        }

        private QuotedPrintable Append(char c)
        {
            sb.Append(c);
            lineWidth++;
            return this;
        }

        private bool RequiresDotStuffing(char c)
        {
            return c == '.' && lineWidth == 0;
        }

        private bool IsPrintable(char c)
        {
            return (c >= '!' && c <= '~' && c != '=') || c == ' ' || c == '\t';
        }

        private bool WillBreakLine(int length)
        {
            return lineWidth > maxWidth - lineBreak.Length - length;
        }

        private byte[] Encode(char c)
        {
            buff[0] = c;
            return System.Text.Encoding.UTF8.GetBytes(buff);
        }

        private char[] buff;
        private int maxWidth;
        private int lineWidth;
        private int lineNumber;
        private string lineBreak;
        private bool inline;
        private StringBuilder sb;
    }
}

