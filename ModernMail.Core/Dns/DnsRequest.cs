using ModernMail.Core.Net.Dns.Enums;
using System;
using System.Collections;

namespace ModernMail.Core.Net.Dns
{
	public class DnsRequest
	{
        public bool RecursionDesired { get; private set; }

        public Opcode Opcode { get; private set; }

		public DnsRequest()
		{
			RecursionDesired = true;
			Opcode = Opcode.StandardQuery;
			_questions = new ArrayList();

		}
		
		public void AddQuestion(DnsQuestion question)
		{
			if (question == null) throw new ArgumentNullException("question");
			_questions.Add(question);
		}

		public byte[] GetMessage()
		{
			ArrayList data = new ArrayList();
			
			data.Add((byte)0);
			data.Add((byte)0);
			
			data.Add((byte)(((byte)Opcode<<3)  | (RecursionDesired?0x01:0)));
			data.Add((byte)0);

			unchecked
			{
				data.Add((byte)(_questions.Count >> 8));
				data.Add((byte)_questions.Count);
			}
			
			data.Add((byte)0); data.Add((byte)0);
			data.Add((byte)0); data.Add((byte)0);
			data.Add((byte)0); data.Add((byte)0);

			foreach (DnsQuestion question in _questions)
			{
				AddDomain(data, question.Domain);
				unchecked
				{
					data.Add((byte)0);
					data.Add((byte)question.Type);
					data.Add((byte)0);
					data.Add((byte)question.Class);
				}
			}

			byte[] message = new byte[data.Count];
			data.CopyTo(message);
			return message;
		}

		private static void AddDomain(ArrayList data, string domainName)
		{
			int position = 0;
			int length = 0;

			while (position < domainName.Length)
			{
				length = domainName.IndexOf('.', position) - position;
				
				if (length < 0) length = domainName.Length - position;
				
				data.Add((byte)length);

				while (length-- > 0)
				{
					data.Add((byte)domainName[position++]);
				}

				position++;
			}
				
			data.Add((byte)0);
		}

        private ArrayList _questions;
	}
}
