
namespace ModernMail.Core.UnitTest.Smtp.Samples
{
    public interface ISignedMessage
    {
        string GetBodyHash();

        string GetDkimSignature();
    }
}
