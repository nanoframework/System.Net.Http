//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright (c) Microsoft Corporation.  All rights reserved.
// See LICENSE file in the project root for full license information.
//

using System;
using System.IO;
using System.Net.Http;
using System.Text;

namespace HttpUnitTests
{
    [Flags]
    public enum MockOptions
    {
        None = 0x0,
        ThrowInSerializeMethods = 0x1,
        ReturnNullInCopyToAsync = 0x2,
        UseWriteByteInCopyTo = 0x4,
        DontOverrideCreateContentReadStream = 0x8,
        CanCalculateLength = 0x10,
        ThrowInTryComputeLength = 0x20,
        ThrowInAsyncSerializeMethods = 0x40
    }

    public class MockException : Exception
    {
        public MockException() { }
        public MockException(string message) : base(message) { }
        public MockException(string message, Exception inner) : base(message, inner) { }
    }

    public class MockContent : HttpContent
    {
        private byte[] _mockData;
        private MockOptions _options;
        private Exception _customException;

        public int TryComputeLengthCount { get; private set; }
        public int SerializeToStreamAsyncCount { get; private set; }
        public int CreateContentReadStreamCount { get; private set; }
        public int DisposeCount { get; private set; }

        public byte[] MockData
        {
            get { return _mockData; }
        }

        public MockContent()
            : this((byte[])null, MockOptions.None)
        {
        }

        public MockContent(byte[] mockData)
            : this(mockData, MockOptions.None)
        {
        }

        public MockContent(MockOptions options)
            : this((byte[])null, options)
        {
        }

        public MockContent(Exception customException, MockOptions options)
            : this((byte[])null, options)
        {
            _customException = customException;
        }

        public MockContent(byte[] mockData, MockOptions options)
        {
            _options = options;

            if (mockData == null)
            {
                _mockData = Encoding.UTF8.GetBytes("data");
            }
            else
            {
                _mockData = mockData;
            }
        }

        public byte[] GetMockData()
        {
            return _mockData;
        }

        protected override bool TryComputeLength(out long length)
        {
            TryComputeLengthCount++;

            if ((_options & MockOptions.ThrowInTryComputeLength) != 0)
            {
                throw new MockException();
            }

            if ((_options & MockOptions.CanCalculateLength) != 0)
            {
                length = _mockData.Length;
                return true;
            }
            else
            {
                length = 0;
                return false;
            }
        }

        protected override void SerializeToStream(Stream stream)
        {
            SerializeToStreamAsyncCount++;

            if ((_options & MockOptions.ReturnNullInCopyToAsync) != 0)
            {
                return;
            }

            if ((_options & MockOptions.ThrowInAsyncSerializeMethods) != 0)
            {
                throw _customException;
            }

            stream.Write(_mockData, 0, _mockData.Length);
        }

        protected override void Dispose(bool disposing)
        {
            DisposeCount++;
            base.Dispose(disposing);
        }

        private void CheckThrow()
        {
            if ((_options & MockOptions.ThrowInSerializeMethods) != 0)
            {
                throw _customException;
            }
        }
    }

    public class MockMemoryStream : MemoryStream
    {
        public int DisposeCount { get; private set; }

        public MockMemoryStream(byte[] buffer)
            : base(buffer)
        {
        }

        protected override void Dispose(bool disposing)
        {
            DisposeCount++;
            base.Dispose(disposing);
        }
    }
}
