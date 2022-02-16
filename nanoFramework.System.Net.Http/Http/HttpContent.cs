//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright (c) Microsoft Corporation.  All rights reserved.
// See LICENSE file in the project root for full license information.
//

using System.IO;
using System.Net.Http.Headers;
using System.Text;

namespace System.Net.Http
{
    public abstract class HttpContent : IDisposable
    {
        private HttpContentHeaders _headers;
        private FixedMemoryStream _buffer;
        private Stream _stream;
        private object _contentReadStream; 
        
        private bool _disposed;
        private bool _canCalculateLength;

        internal const int MaxBufferSize = int.MaxValue;
        internal static readonly Encoding DefaultStringEncoding = Encoding.UTF8;

        public HttpContentHeaders Headers
        {
            get
            {
                if (_headers == null)
                {
                    _headers = new HttpContentHeaders(this);
                }

                return _headers;
            }
        }

        //private bool IsBuffered
        //{
        //    get { return _bufferedContent != null; }
        //}

        //internal bool TryGetBuffer(out ArraySegment<byte> buffer)
        //{
        //    if (_bufferedContent != null)
        //    {
        //        return _bufferedContent.TryGetBuffer(out buffer);
        //    }
        //    buffer = default;
        //    return false;
        //}

        protected HttpContent()
        {
            // We start with the assumption that we can calculate the content length.
            _canCalculateLength = true;
        }

        //private string ReadBufferedContentAsString()
        //{
        //    Debug.Assert(IsBuffered);

        //    if (_bufferedContent!.Length == 0)
        //    {
        //        return string.Empty;
        //    }

        //    ArraySegment<byte> buffer;

        //    if (!TryGetBuffer(out buffer))
        //    {
        //        buffer = new ArraySegment<byte>(_bufferedContent.ToArray());
        //    }

        //    return ReadBufferAsString(buffer, Headers);
        //}

        //internal static string ReadBufferAsString(ArraySegment<byte> buffer, HttpContentHeaders headers)
        //{
        //    // We don't validate the Content-Encoding header: If the content was encoded, it's the caller's
        //    // responsibility to make sure to only call ReadAsString() on already decoded content. E.g. if the
        //    // Content-Encoding is 'gzip' the user should set HttpClientHandler.AutomaticDecompression to get a
        //    // decoded response stream.

        //    Encoding encoding = null;
        //    int bomLength = -1;

        //    string charset = headers._headerStore[WebHeaderCollection].ContentType?.CharSet;

        //    // If we do have encoding information in the 'Content-Type' header, use that information to convert
        //    // the content to a string.
        //    if (charset != null)
        //    {
        //        try
        //        {
        //            // Remove at most a single set of quotes.
        //            if (charset.Length > 2 &&
        //                charset[0] == '\"' &&
        //                charset[charset.Length - 1] == '\"')
        //            {
        //                encoding = Encoding.GetEncoding(charset.Substring(1, charset.Length - 2));
        //            }
        //            else
        //            {
        //                encoding = Encoding.GetEncoding(charset);
        //            }

        //            // Byte-order-mark (BOM) characters may be present even if a charset was specified.
        //            bomLength = GetPreambleLength(buffer, encoding);
        //        }
        //        catch (ArgumentException e)
        //        {
        //            throw new InvalidOperationException();
        //        }
        //    }

        //    // If no content encoding is listed in the ContentType HTTP header, or no Content-Type header present,
        //    // then check for a BOM in the data to figure out the encoding.
        //    if (encoding == null)
        //    {
        //        if (!TryDetectEncoding(buffer, out encoding, out bomLength))
        //        {
        //            // Use the default encoding (UTF8) if we couldn't detect one.
        //            encoding = DefaultStringEncoding;

        //            // We already checked to see if the data had a UTF8 BOM in TryDetectEncoding
        //            // and DefaultStringEncoding is UTF8, so the bomLength is 0.
        //            bomLength = 0;
        //        }
        //    }

        //    // Drop the BOM when decoding the data.
        //    return encoding.GetString(buffer.Array!, buffer.Offset + bomLength, buffer.Count - bomLength);
        //}

        //private static bool TryDetectEncoding(ArraySegment<byte> buffer, out Encoding encoding, out int preambleLength)
        //{
        //    byte[] data = buffer.Array;
        //    int offset = buffer.Offset;
        //    int dataLength = buffer.Count;

        //    Debug.Assert(data != null);

        //    if (dataLength >= 2)
        //    {
        //        int first2Bytes = data[offset + 0] << 8 | data[offset + 1];

        //        switch (first2Bytes)
        //        {
        //            case UTF8PreambleFirst2Bytes:
        //                if (dataLength >= UTF8PreambleLength && data[offset + 2] == UTF8PreambleByte2)
        //                {
        //                    encoding = Encoding.UTF8;
        //                    preambleLength = UTF8PreambleLength;
        //                    return true;
        //                }
        //                break;

        //            default:
        //                // all other encodings are not supported
        //                throw new NotSupportedException();
        //        }
        //    }

        //    encoding = null;
        //    preambleLength = 0;
        //    return false;
        //}


        public Stream LoadIntoBuffer()
        {
            return LoadIntoBuffer(int.MaxValue);
        }

        public Stream LoadIntoBuffer(long maxBufferSize)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException();
            }

            if (_buffer != null)
            {
                return _buffer;
            }

            _buffer = new FixedMemoryStream(maxBufferSize);
            
            SerializeToStream(_buffer);
            
            _buffer.Seek(0, SeekOrigin.Begin);

            return _buffer;
        }
        public Stream ReadAsStream()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException();
            }

            if (_buffer != null)
            {

                return new MemoryStream(_buffer.GetBuffer());
            }

            if (_stream == null)
            {
                
                _stream = LoadIntoBuffer();
            }

            return _stream;
        }
        public byte[] ReadAsByteArray()
        {
            LoadIntoBuffer();
            
            return _buffer.ToArray();
        }

        public string ReadAsString()
        {
            LoadIntoBuffer();

            if (_buffer.Length == 0)
            {
                return string.Empty;
            }

            var buf = _buffer.GetBuffer();
            var buf_length = (int)_buffer.Length;
            int preambleLength = 0;
            // we only support UTF-8
            Encoding encoding = Encoding.UTF8; ;

            return encoding.GetString(buf, preambleLength, buf_length - preambleLength);
        }

        protected internal abstract bool TryComputeLength(out long length);

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <inheritdoc/>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                _disposed = true;

                if (_buffer != null)
                {
                    _buffer.Dispose();
                }
            }
        }

        protected abstract Stream SerializeToStream(Stream stream);

        sealed class FixedMemoryStream : MemoryStream
        {
            readonly long maxSize;

            public FixedMemoryStream(long maxSize)
                : base()
            {
                this.maxSize = maxSize;
            }

            void CheckOverflow(int count)
            {
                if (Length + count > maxSize)
                {
                    throw new HttpRequestException();
                }
            }

            public override void WriteByte(byte value)
            {
                CheckOverflow(1);
                base.WriteByte(value);
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                CheckOverflow(count);
                base.Write(buffer, offset, count);
            }
        }
    }
}
