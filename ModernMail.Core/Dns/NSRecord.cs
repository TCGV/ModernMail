
namespace ModernMail.Core.Net.Dns
{
	public class NSRecord : RecordBase
	{
        public string DomainName { get; private set; }
				
		internal NSRecord(Pointer pointer)
		{
            DomainName = pointer.ReadDomain();
		}

		public override string ToString()
		{
            return DomainName;
		}
	}
}