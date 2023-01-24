//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright (c) Microsoft Corporation.  All rights reserved.
// See LICENSE file in the project root for full license information.
//

namespace System.Net
{
    using System.IO;
    using System.Net.Sockets;
    using System.Text;

    /// <summary>
    /// The OutputNetworkStreamWrapper is used to re-implement calls to  NetworkStream.Write
    /// On first write HttpListenerResponse needs to send HTTP headers to client.
    /// </summary>
    internal class OutputNetworkStreamWrapper : Stream
    {

        /// <summary>
        /// This is a socket connected to client.
        /// OutputNetworkStreamWrapper owns the socket, not NetworkStream.
        /// If connection is persistent, then the m_Socket is transferred to the list of
        /// </summary>
        internal Socket m_Socket;

        /// <summary>
        /// Actual network or SSL stream connected to the client.
        /// It could be SSL stream, so NetworkStream is not exact type, m_Stream would be derived from NetworkStream
        /// </summary>
        internal NetworkStream m_Stream;

        /// <summary>
        /// If true causes all written data to be encoded as chunks
        /// </summary>
        internal bool m_enableChunkedEncoding = false;

        /// <summary>
        /// Type definition of delegate for sending of HTTP headers.
        /// </summary>
        internal delegate void SendHeadersDelegate();

        /// <summary>
        /// If not null - indicates whether we have sent headers or not.
        /// Calling of delegete sends HTTP headers to client - HttpListenerResponse.SendHeaders()
        /// </summary>
        private SendHeadersDelegate m_headersSend;

        /// <summary>
        /// EOL marker in chunked encoding
        /// </summary>
        private readonly byte[] EOLMarker = { 0xd, 0xa };

        /// <summary>
        /// Just passes parameters to the base.
        /// Socket is not owned by base NetworkStream
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="stream"></param>
        public OutputNetworkStreamWrapper(Socket socket, NetworkStream stream)
        {
            m_Socket = socket;
            m_Stream = stream;
        }

        /// <summary>
        /// Sets the delegate for sending of headers.
        /// </summary>
        internal SendHeadersDelegate HeadersDelegate { set { m_headersSend = value; } }

        /// <summary>
        /// Return true if stream support reading.
        /// </summary>
        public override bool CanRead { get { return false; } }

        /// <summary>
        /// Return true if stream supports seeking
        /// </summary>
        public override bool CanSeek { get { return false; } }

        /// <summary>
        /// Return true if timeout is applicable to the stream
        /// </summary>
        public override bool CanTimeout { get { return m_Stream.CanTimeout; } }

        /// <summary>
        /// Return true if stream support writing. It should be true, as this is output stream.
        /// </summary>
        public override bool CanWrite { get { return true; } }

        /// <summary>
        /// Gets the length of the data available on the stream.
        /// Since this is output stream reading is not allowed and length does not have meaning.
        /// </summary>
        /// <returns>The length of the data available on the stream.</returns>
        public override long Length { get { throw new NotSupportedException(); } }

        /// <summary>
        /// Position is not supported for NetworkStream
        /// </summary>
        public override long Position
        {
            get
            {
                throw new NotSupportedException();
            }

            set
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Timeout for read operations.
        /// </summary>
        public override int ReadTimeout
        {
            get { return m_Stream.ReadTimeout; }
            set { m_Stream.ReadTimeout = value; }
        }

        /// <summary>
        /// Timeout for write operations.
        /// </summary>
        public override int WriteTimeout
        {
            get { return m_Stream.WriteTimeout; }
            set { m_Stream.WriteTimeout = value; }
        }

        /// <summary>
        /// Writes to stream size of chunk and marks start of the chunk 
        /// </summary>
        private void WriteChunkStart(int size)
        {
            byte[] chunkLengthBytes = Encoding.UTF8.GetBytes($"{size:X}");
            m_Stream.Write(chunkLengthBytes, 0, chunkLengthBytes.Length);
            m_Stream.Write(EOLMarker, 0, EOLMarker.Length);
        }

        /// <summary>
        /// Writes to stream marker - finish of the chunk
        /// </summary>
        private void WriteChunkEnd()
        {
            m_Stream.Write(EOLMarker, 0, EOLMarker.Length);
        }

        /// <summary>
        /// Writes to stream marker - finish of all chunks
        /// </summary>
        private void WriteChunkFinish()
        {
            byte[] zero = { 0x30 };
            m_Stream.Write(zero, 0, 1);
            m_Stream.Write(EOLMarker, 0, EOLMarker.Length);
            m_Stream.Write(EOLMarker, 0, EOLMarker.Length);
        }

        /// <summary>
        /// Closes the stream. Verifies that HTTP response is sent before closing.
        /// </summary>
        public override void Close()
        {
            if (m_headersSend != null)
            {
                // Calls HttpListenerResponse.SendHeaders. HttpListenerResponse.SendHeaders sets m_headersSend to null.
                m_headersSend();
            }

            if (m_Stream != null) m_Stream.Close();
            m_Stream = null;
            m_Socket = null;
        }

        /// <summary>
        /// Flushes the stream. Verifies that HTTP response is sent before flushing.
        /// </summary>
        public override void Flush()
        {
            if (m_headersSend != null)
            {
                // Calls HttpListenerResponse.SendHeaders. HttpListenerResponse.SendHeaders sets m_headersSend to null.
                m_headersSend();
            }

            if (m_enableChunkedEncoding)
            {
                WriteChunkFinish();
            }

            // Need to check for null before using here
            m_Stream?.Flush();
        }

        /// <summary>
        /// This putput stream, so read is not supported.
        /// </summary>
        /// <returns></returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// This putput stream, so read is not supported.
        /// </summary>
        /// <returns></returns>
        public override int ReadByte()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Seeking is not suported on network streams
        /// </summary>
        /// <param name="offset">Offset to seek</param>
        /// <param name="origin">Relative origin of the seek</param>
        /// <returns></returns>
        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Setting length is not suported on network streams
        /// </summary>
        /// <param name="value">Length to set</param>
        /// <returns></returns>
        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Writes single byte to the stream.
        /// </summary>
        /// <param name="value">Byte value to write.</param>
        public override void WriteByte(byte value)
        {
            if (m_headersSend != null)
            {
                // Calls HttpListenerResponse.SendHeaders. HttpListenerResponse.SendHeaders sets m_headersSend to null.
                m_headersSend();
            }

            if (m_enableChunkedEncoding)
            {
                WriteChunkStart(1);
            }

            m_Stream.WriteByte(value);

            if (m_enableChunkedEncoding)
            {
                WriteChunkEnd();
            }
        }


        /// <summary>
        /// Re-implements writing of data to network stream.
        /// The only functionality - on first write it sends HTTP headers.
        /// Then calls base
        /// </summary>
        /// <param name="buffer">Buffer with data to write to HTTP client</param>
        /// <param name="offset">Offset at which to use data from buffer</param>
        /// <param name="size">Count of bytes to write.</param>
        public override void Write(byte[] buffer, int offset, int size)
        {
            if (m_headersSend != null)
            {
                // Calls HttpListenerResponse.SendHeaders. HttpListenerResponse.SendHeaders sets m_headersSend to null.
                m_headersSend();
            }

            if (m_enableChunkedEncoding)
            {
                WriteChunkStart(size);
            }

            m_Stream.Write(buffer, offset, size);

            if (m_enableChunkedEncoding)
            {
                WriteChunkEnd();
            }
        }

        public override int Read(SpanByte buffer)
        {
            throw new NotSupportedException();
        }
    }
}


