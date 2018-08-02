﻿using ModernMail.Core.Encoding;
using ModernMail.Core.Extensions;
using ModernMail.Core.Model;
using System;
using System.IO;
using System.Text;

namespace ModernMail.Core.Dkim
{
    public class DkimCanonicalizer
    {
        public string CanonicalizeHeaders(MailHeader[] headers, DkimCanonicalizationAlgorithm type)
        {
            if (headers == null || headers.Length == 0)
                throw new ArgumentNullException("headers");

            var sb = new StringBuilder();

            switch (type)
            {
                /*
                 * 3.4.1 The "simple" Header Canonicalization Algorithm
                 * 
                 * The "simple" header canonicalization algorithm does not change header fields in any way. 
                 * Header fields MUST be presented to the signing or verification algorithm exactly as they 
                 * are in the message being signed or verified. In particular, header field names MUST NOT 
                 * be case folded and whitespace MUST NOT be changed.
                 *  
                 * */
                case DkimCanonicalizationAlgorithm.Simple:
                    foreach (var h in headers)
                    {
                        sb.Append(h.Key);
                        sb.Append(':');
                        sb.Append(h.Value);
                        sb.Append(Keyword.CRLF);
                    }

                    break;

                /*
                 * 
                 * 3.4.2 The "relaxed" Header Canonicalization Algorithm
                 * 
                 * The "relaxed" header canonicalization algorithm MUST apply the following steps in order:
                 * 
                 * Convert all header field names (not the header field values) to lowercase. For example, 
                 * convert "SUBJect: AbC" to "subject: AbC".
                 * 
                 * 
                 * Unfold all header field continuation lines as described in [RFC2822]; in particular, lines 
                 * with terminators embedded in continued header field values (that is, CRLF sequences followed 
                 * by WSP) MUST be interpreted without the CRLF. Implementations MUST NOT remove the CRLF at the 
                 * end of the header field value.
                 * 
                 * 
                 * Convert all sequences of one or more WSP characters to a single SP character. WSP characters 
                 * here include those before and after a line folding boundary.
                 * 
                 * 
                 * Delete all WSP characters at the end of each unfolded header field value.
                 * 
                 * 
                 * Delete any WSP characters remaining before and after the colon separating the header field name
                 * from the header field value. The colon separator MUST be retained.
                 * 
                 * */
                case DkimCanonicalizationAlgorithm.Relaxed:
                    foreach (var h in headers)
                    {
                        sb.Append(h.Key.Trim().ToLower());
                        sb.Append(':');

                        sb.Append(h.FoldedValue
                            ? h.Value.Trim().Replace(Keyword.CRLF, string.Empty).ReduceWitespace()
                            : h.Value.Trim().ReduceWitespace());

                        sb.Append(Keyword.CRLF);
                    }

                    break;

                default:
                    throw new ArgumentException("Invalid canonicalization type.");
            }

            return sb.ToString();
        }

        public string CanonicalizeBody(string body, DkimCanonicalizationAlgorithm type)
        {
            if (body == null)
                body = string.Empty;

            var sb = new StringBuilder(body.Length + Keyword.CRLF.Length);

            switch (type)
            {
                /*
                 * 3.4.4 The "relaxed" Body Canonicalization Algorithm
                 * 
                 * The "relaxed" body canonicalization algorithm:
                 * 
                 * Ignores all whitespace at the end of lines. Implementations MUST NOT remove the CRLF at the end of the line.
                 * 
                 * Reduces all sequences of WSP within a line to a single SP character.
                 * 
                 * Ignores all empty lines at the end of the message body. "Empty line" is defined in Section 3.4.3.
                 * 
                 * INFORMATIVE NOTE: It should be noted that the relaxed body canonicalization algorithm may enable certain 
                 * types of extremely crude "ASCII Art" attacks where a message may be conveyed by adjusting the spacing 
                 * between words. If this is a concern, the "simple" body canonicalization algorithm should be used instead.
                 * 
                 * */
                case DkimCanonicalizationAlgorithm.Relaxed:
                    using (var reader = new StringReader(body))
                    {
                        string line;
                        int emptyLineCount = 0;

                        while ((line = reader.ReadLine()) != null)
                        {

                            if (line == string.Empty)
                            {
                                emptyLineCount++;
                                continue;
                            }

                            while (emptyLineCount > 0)
                            {
                                sb.AppendLine();
                                emptyLineCount--;
                            }

                            sb.AppendLine(line.TrimEnd().ReduceWitespace());
                        }
                    }

                    break;

                /*
                 * 
                 * 3.4.3 The "simple" Body Canonicalization Algorithm
                 * 
                 * The "simple" body canonicalization algorithm ignores all empty lines at the end 
                 * of the message body. An empty line is a line of zero length after removal of the 
                 * line terminator. If there is no body or no trailing CRLF on the message body, a 
                 * CRLF is added. It makes no
                 * 
                 * Note that a completely empty or missing body is canonicalized as a single "CRLF"; 
                 * that is, the canonicalized length will be 2 octets.
                 * 
                 * */
                case DkimCanonicalizationAlgorithm.Simple:
                    using (var reader = new StringReader(body))
                    {
                        string line;
                        int emptyLineCount = 0;

                        while ((line = reader.ReadLine()) != null)
                        {

                            if (line == string.Empty)
                            {
                                emptyLineCount++;
                                continue;
                            }

                            while (emptyLineCount > 0)
                            {
                                sb.AppendLine();
                                emptyLineCount--;
                            }

                            sb.AppendLine(line);
                        }
                    }

                    if (sb.Length == 0)
                    {
                        sb.AppendLine();
                    }

                    break;

                default:
                    throw new ArgumentException("Invalid canonicalization type.");
            }

            return sb.ToString();
        }
    }
}
