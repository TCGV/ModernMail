using ModernMail.Core.Net.Dns.Enums;
using System;

namespace ModernMail.Core.Net.Dns
{
    public class DnsResponse
    {
        public ReturnCode ReturnCode { get; private set; }
        public bool AuthoritativeAnswer { get; private set; }
        public bool RecursionAvailable { get; private set; }
        public bool MessageTruncated { get; private set; }
        public DnsQuestion[] Questions { get; private set; }
        public Answer[] Answers { get; private set; }
        public NameServer[] NameServers { get; private set; }
        public AdditionalRecord[] AdditionalRecords { get; private set; }

        internal DnsResponse(byte[] message)
        {
            byte flags1 = message[2];
            byte flags2 = message[3];

            int returnCode = flags2 & 15;

            if (returnCode > 6) returnCode = 6;
            ReturnCode = (ReturnCode)returnCode;

            AuthoritativeAnswer = ((flags1 & 4) != 0);
            RecursionAvailable = ((flags2 & 128) != 0);
            MessageTruncated = ((flags1 & 2) != 0);

            Questions = new DnsQuestion[GetShort(message, 4)];
            Answers = new Answer[GetShort(message, 6)];
            NameServers = new NameServer[GetShort(message, 8)];
            AdditionalRecords = new AdditionalRecord[GetShort(message, 10)];

            Pointer pointer = new Pointer(message, 12);

            for (int index = 0; index < Questions.Length; index++)
            {
                try
                {
                    Questions[index] = new DnsQuestion(pointer);
                }
                catch (Exception ex)
                {
                    throw new InvalidResponseException(ex);
                }
            }
            for (int index = 0; index < Answers.Length; index++)
            {
                Answers[index] = new Answer(pointer);
            }
            for (int index = 0; index < NameServers.Length; index++)
            {
                NameServers[index] = new NameServer(pointer);
            }
            for (int index = 0; index < AdditionalRecords.Length; index++)
            {
                AdditionalRecords[index] = new AdditionalRecord(pointer);
            }
        }

        private static short GetShort(byte[] message, int position)
        {
            return (short)(message[position] << 8 | message[position + 1]);
        }
    }
}
