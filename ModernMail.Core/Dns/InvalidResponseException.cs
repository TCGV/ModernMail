using System;

namespace ModernMail.Core.Net.Dns
{
	[Serializable]
	public class InvalidResponseException : SystemException
	{
		public InvalidResponseException(Exception innerException) :  base(null, innerException) 
		{

		}
	}
}
