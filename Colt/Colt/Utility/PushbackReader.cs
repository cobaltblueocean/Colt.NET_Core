using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace System.IO
{
    public class PushbackReader: StreamReader
    {
        private PushbackReader _bufferReader;

        #region Constructors
        public PushbackReader(Stream stream): base (stream) { }

        public PushbackReader(String path) : base(path) { }

        public PushbackReader(Stream stream, Boolean detectEncodingFromByteOrderMarks) : base(stream, detectEncodingFromByteOrderMarks) { }

        public PushbackReader(Stream stream, Encoding encoding): base(stream, encoding) { }

        public PushbackReader(String path, Boolean detectEncodingFromByteOrderMarks) : base(path, detectEncodingFromByteOrderMarks) { }

        public PushbackReader(String path, Encoding encoding) : base(path, encoding) { }

        public PushbackReader(Stream stream, Encoding encoding, Boolean detectEncodingFromByteOrderMarks) : base(stream, encoding, detectEncodingFromByteOrderMarks) { }

        public PushbackReader(String path, Encoding encoding, Boolean detectEncodingFromByteOrderMarks) : base(path, encoding, detectEncodingFromByteOrderMarks) { }

        public PushbackReader(Stream stream, Encoding encoding, Boolean detectEncodingFromByteOrderMarks, int bufferSize) : base(stream, encoding, detectEncodingFromByteOrderMarks, bufferSize) { }

        public PushbackReader(String path, Encoding encoding, Boolean detectEncodingFromByteOrderMarks, int bufferSize) : base(path, encoding, detectEncodingFromByteOrderMarks, bufferSize) { }

        public PushbackReader(Stream stream, Encoding encoding, Boolean detectEncodingFromByteOrderMarks, int bufferSize, bool leaveOpen) : base(stream, encoding, detectEncodingFromByteOrderMarks, bufferSize, leaveOpen) { }

        #endregion

        public new int Read()
        {
            _bufferReader = (PushbackReader)this.MemberwiseClone();
            return this.Read();
        }

        public PushbackReader Unread()
        {
            return _bufferReader;
        }
    }
}
