//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright (c) Microsoft Corporation.  All rights reserved.
// See LICENSE file in the project root for full license information.
//

using System.IO;
using System.Threading;

namespace System.Net.Http
{
    public class StreamContent : HttpContent
    {
        private readonly Stream _content;
        private readonly int bufferSize;
        private readonly CancellationToken cancellationToken;
        private readonly long startPosition;
        private bool contentCopied;

        public StreamContent(Stream content)
        : this(content, 16 * 1024)
        {
        }

        public StreamContent(Stream content, int bufferSize)
        {
            if (content == null)
            {
                throw new ArgumentNullException();
            }

            if (bufferSize <= 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            this._content = content;
            this.bufferSize = bufferSize;

            if (content.CanSeek)
            {
                startPosition = content.Position;
            }
        }
        internal StreamContent(Stream content, CancellationToken cancellationToken)
        : this(content)
        {
            // We don't own the token so don't worry about disposing it
            this.cancellationToken = cancellationToken;
        }

        protected override void SerializeToStream(Stream stream)
        {
            if (contentCopied)
            {
                if (!_content.CanSeek)
                {
                    throw new InvalidOperationException();
                }

                _content.Seek(startPosition, SeekOrigin.Begin);
            }
            else
            {
                contentCopied = true;
            }

            byte[] buffer = new byte[bufferSize];
            int read;

            while ((read = _content.Read(buffer, 0, buffer.Length)) != 0)
            {
                stream.Write(buffer, 0, read);
            }
        }

        protected internal override bool TryComputeLength(out long length)
        {
            if (!_content.CanSeek)
            {
                length = 0;
                return false;
            }

            length = _content.Length - startPosition;

            return true;
        }
    }
}
