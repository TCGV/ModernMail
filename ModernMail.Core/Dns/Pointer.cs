using ModernMail.Core.Net.Dns.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModernMail.Core.Net.Dns
{
    internal class Pointer
    {
        public Pointer(byte[] message, int position)
        {
            _message = message;
            _position = position;
        }

        public Pointer Copy()
        {
            return new Pointer(_message, _position);
        }

        public void Seek(int offset)
        {
            _position += offset;
        }

        public void SetPosition(int position)
        {
            _position = position;
        }

        public byte Peek()
        {
            return _message[_position];
        }

        public byte ReadByte()
        {
            return _message[_position++];
        }

        public short ReadShort()
        {
            return (short)(ReadByte() << 8 | ReadByte());
        }

        public int ReadInt()
        {
            return (ushort)ReadShort() << 16 | (ushort)ReadShort();
        }

        public char ReadChar()
        {
            return (char)ReadByte();
        }

        public DnsType ReadType()
        {
            var val = (DnsType)ReadShort();
            if (!_types.Contains((int)val))
                throw new InvalidOperationException();
            return val;
        }

        public DnsClass ReadClass()
        {
            var val = (DnsClass)ReadShort();
            if (!_classes.Contains((int)val))
                throw new InvalidOperationException();
            return val;
        }

        public string ReadDomain()
        {
            StringBuilder domain = new StringBuilder();
            int length = 0;

            while ((length = ReadByte()) != 0)
            {
                if (IsCompressed(length))
                {
                    Pointer newPointer = Copy();
                    newPointer.SetPosition((length & 0x3f) << 8 | ReadByte());
                    domain.Append(newPointer.ReadDomain());
                    return domain.ToString();
                }

                while (length > 0)
                {
                    domain.Append(ReadChar());
                    length--;
                }

                if (Peek() != 0) domain.Append('.');
            }

            return domain.ToString();
        }

        private static bool IsCompressed(int length)
        {
            return (length & 0xc0) == 0xc0;
        }

        private byte[] _message;
        private int _position;

        private static HashSet<int> _types = new HashSet<int>(Enum.GetValues(typeof(DnsType)) as int[]);
        private static HashSet<int> _classes = new HashSet<int>(Enum.GetValues(typeof(DnsClass)) as int[]);
    }
}