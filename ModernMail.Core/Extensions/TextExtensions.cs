using System;
using System.Text;

namespace ModernMail.Core.Extensions
{
    public static class StringExtensions
    {
        public static bool IsWhiteSpace(this char c)
        {
            return c == ' ' || c == '\t' || c == '\r' || c == '\n';
        }

        public static string ReduceWitespace(this string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }

            if (text.IndexOfAny(new char[] { ' ', '\t', '\r', '\n' }) == -1)
            {
                return text;
            }

            var sb = new StringBuilder(text.Length);
            bool hasWhiteSpace = false;
            foreach (var c in text)
            {
                if (c.IsWhiteSpace())
                {
                    hasWhiteSpace = true;
                }
                else
                {
                    if (hasWhiteSpace)
                    {
                        sb.Append(' ');
                    }
                    sb.Append(c);
                    hasWhiteSpace = false;
                }
            }

            return sb.ToString();

        }

        public static string RemoveWhitespace(this string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }

            if (text.IndexOfAny(new char[] { ' ', '\t', '\r', '\n' }) == -1)
            {
                return text;
            }

            var sb = new StringBuilder(text.Length);
            foreach (var c in text)
            {
                switch (c)
                {
                    case ' ':
                    case '\t':
                    case '\r':
                    case '\n':
                            break;
                    default:
                            sb.Append(c);
                            break;
                }

            }

            return sb.ToString();
        }
    }
}
