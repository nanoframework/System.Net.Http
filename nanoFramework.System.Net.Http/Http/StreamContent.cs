//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright (c) Microsoft Corporation.  All rights reserved.
// See LICENSE file in the project root for full license information.
//

using System.IO;
using System.Threading;

namespace System.Net.Http
{
    /// <summary>
    /// Provides HTTP content based on a stream.
    /// </summary>
    public class StreamContent : HttpContent
    {
        private readonly Stream _content;
        private readonly int _bufferSize;
        private readonly long _startPosition;
        private bool _contentCopied;

        /// <summary>
        /// Creates a new instance of the <see cref="StreamContent"/> class.
        /// </summary>
        /// <param name="content">The content used to initialize the <see cref="StreamContent"/>.</param>
        /// <remarks>
        /// The <see cref="StreamContent"/> object calls <see cref="Dispose"/> on the provided Stream object when <see cref="StreamContent.Dispose"/> is called.
        /// </remarks>
        public StreamContent(Stream content)
        : this(content, 16 * 1024)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="StreamContent"/> class.
        /// </summary>
        /// <param name="content">The content used to initialize the <see cref="StreamContent"/>.</param>
        /// <param name="bufferSize">The size, in bytes, of the buffer for the <see cref="StreamContent"/>.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="content"/> was <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="bufferSize"/> was less than or equal to zero.</exception>
        /// <remarks>
        /// The <see cref="StreamContent"/> object calls <see cref="Dispose"/> on the provided Stream object when <see cref="StreamContent.Dispose"/> is called.
        /// </remarks>
        public StreamContent(
            Stream content,
            int bufferSize)
        {
            if (bufferSize <= 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            _content = content ?? throw new ArgumentNullException();
            
            _bufferSize = bufferSize;

            if (content.CanSeek)
            {
                _startPosition = content.Position;
            }
        }

        /// <inheritdoc/>
        protected override void SerializeToStream(Stream stream)
        {
            if (_contentCopied)
            {
                if (!_content.CanSeek)
                {
                    throw new InvalidOperationException();
                }

                _content.Seek(_startPosition, SeekOrigin.Begin);
            }
            else
            {
                _contentCopied = true;
            }

            byte[] buffer = new byte[_bufferSize];
            int read;

            while ((read = _content.Read(buffer, 0, buffer.Length)) != 0)
            {
                stream.Write(buffer, 0, read);
            }
        }

        /// <inheritdoc/>
        protected internal override bool TryComputeLength(out long length)
        {
            if (!_content.CanSeek)
            {
                length = 0;
                return false;
            }

            length = _content.Length - _startPosition;

            return true;
        }
    }
}
