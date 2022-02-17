//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright (c) Microsoft Corporation.  All rights reserved.
// See LICENSE file in the project root for full license information.
//

using System.IO;

namespace System.Net.Http
{
    public class ByteArrayContent : HttpContent
    {
        private readonly byte[] _content;
        private readonly int _offset;
        private readonly int _count;

        public ByteArrayContent(byte[] content)
        {
            _content = content;
            _count = content.Length;
        }

        public ByteArrayContent(byte[] content, int offset, int count)
        {
            if(content is null)
            {
                throw new ArgumentNullException();
            }

            if ((offset < 0) || (offset > content.Length))
            {
                throw new ArgumentOutOfRangeException(nameof(offset));
            }

            if ((count < 0) || (count > (content.Length - offset)))
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            _content = content;
            _offset = offset;
            _count = count;
        }

        protected override void SerializeToStream(Stream stream)
        {
            stream.Write(_content, _offset, _count);
        }

        protected internal override bool TryComputeLength(out long length)
        {
            length = _count;
            return true;
        }
    }
}
