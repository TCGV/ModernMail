
namespace ModernMail.Core.Model
{
    public class HeaderName
    {
        public const string MimeVersion = "MIME-Version";

        public const string DkimSignature = "DKIM-Signature";

        public const string ContentType = "Content-Type";

        public const string ContentTransferEncoding = "Content-Transfer-Encoding";

        public const string ContentDisposition = "Content-Disposition";

        public const string ContentID = "Content-ID";

        public static string From = "From";

        public static string To = "To";

        public static string Cc = "Cc";

        public const string Subject = "Subject";
    }
}
