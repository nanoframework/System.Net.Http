//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright (c) Microsoft Corporation.  All rights reserved.
// See LICENSE file in the project root for full license information.
//

using nanoFramework.TestFramework;
using System;
using System.IO;
using System.Net.Http;
using System.Text;

namespace HttpUnitTests
{
    [TestClass]
    public class ByteArrayContentTest
    {
        [TestMethod]
        public void Ctor_NullSourceArray_ThrowsArgumentNullException()
        {
            Assert.Throws(typeof(ArgumentNullException), () => new ByteArrayContent(null));
        }

        [TestMethod]
        public void Ctor_NullSourceArrayWithRange_ThrowsArgumentNullException()
        {
            Assert.Throws(typeof(ArgumentNullException), () => new ByteArrayContent(null, 0, 1));
        }


        [TestMethod]
        public void Ctor_EmptySourceArrayWithRange_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => new ByteArrayContent(new byte[0], 0, 1));
        }


        [TestMethod]
        public void Ctor_StartIndexTooBig_ThrowsArgumentOufOfRangeException()
        {
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => new ByteArrayContent(new byte[5], 5, 1));
        }

        [TestMethod]
        public void Ctor_StartIndexNegative_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => new ByteArrayContent(new byte[5], -1, 1));
        }

        [TestMethod]
        public void Ctor_LengthTooBig_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => new ByteArrayContent(new byte[5], 1, 5));
        }

        [TestMethod]
        public void Ctor_LengthPlusOffsetCauseIntOverflow_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => new ByteArrayContent(new byte[5], 1, int.MaxValue));
        }

        [TestMethod]
        public void Ctor_LengthNegative_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => new ByteArrayContent(new byte[5], 0, -1));
        }

        // TODO need to fix processing of exception
        //[TestMethod]
        //public void ContentLength_UseWholeSourceArray_LengthMatchesArrayLength()
        //{
        //    var contentData = new byte[10];
        //    var content = new ByteArrayContent(contentData);

        //    Assert.Equal(contentData.Length, content.Headers.ContentLength);
        //}

        // TODO need to fix processing of exception
        //[TestMethod]
        //public void ContentLength_UsePartialSourceArray_LengthMatchesArrayLength()
        //{
        //    Assert.SkipTest("Test disabled on API failure");

        //    // TODO need to fix edge case in ByteArrayContent

        //    var contentData = new byte[10];
        //    var content = new ByteArrayContent(contentData, 5, 3);

        //    Assert.Equal(3, content.Headers.ContentLength);
        //}

        [TestMethod]
        public void ReadAsStreamAsync_EmptySourceArray_Succeed()
        {
            var content = new ByteArrayContent(new byte[0]);
            Stream stream = content.ReadAsStream();
            Assert.Equal(0, stream.Length);
        }

        // TODO need to fix processing of exception
        //[TestMethod]
        //public void ReadAsStream_Call_MemoryStreamWrappingByteArrayReturned()
        //{
        //    Assert.SkipTest("Test disabled on API failure");

        //    // TODO need to fix edge case in stream reader

        //    var contentData = new byte[10];
        //    var content = new MockByteArrayContent(contentData, 5, 3);

        //    Stream stream = content.ReadAsStream();
        //    Assert.False(stream.CanWrite);
        //    Assert.Equal(3, stream.Length);
        //    Assert.Equal(0, content.CopyToCount);
        //}

        // TODO need to fix processing of exception
        //[TestMethod]
        //public void CopyTo_NullDestination_ThrowsArgumentNullException()
        //{
        //    byte[] contentData = CreateSourceArray();
        //    var content = new ByteArrayContent(contentData);

        //    Assert.Throws(typeof(ArgumentNullException),
        //        () =>
        //        {
        //            content.CopyTo(null);
        //        });
        //}

        [TestMethod]
        public void CopyTo_UseWholeSourceArray_WholeContentCopied()
        {
            byte[] contentData = CreateSourceArray();
            var content = new ByteArrayContent(contentData);

            var destination = new MemoryStream();
            content.CopyTo(destination);

            Assert.Equal(contentData.Length, destination.Length);
            CheckResult(destination, 0);
        }

        [TestMethod]
        public void CopyTo_UsePartialSourceArray_PartialContentCopied()
        {
            byte[] contentData = CreateSourceArray();
            var content = new ByteArrayContent(contentData, 3, 5);

            var destination = new MemoryStream();
            content.CopyTo(destination);

            Assert.Equal(5, destination.Length);
            CheckResult(destination, 3);
        }

        // TODO need to fix processing of exception
        //[TestMethod]
        //public void CopyTo_UseEmptySourceArray_NothingCopied()
        //{
        //    var contentData = new byte[0];
        //    var content = new ByteArrayContent(contentData, 0, 0);

        //    var destination = new MemoryStream();
        //    content.CopyTo(destination);

        //    Assert.Equal(0, destination.Length);
        //}

        #region Helper methods

        private static byte[] CreateSourceArray()
        {
            var contentData = new byte[10];
            for (int i = 0; i < contentData.Length; i++)
            {
                contentData[i] = (byte)(i % 256);
            }
            return contentData;
        }

        private static void CheckResult(Stream destination, byte firstValue)
        {
            destination.Position = 0;
            var destinationData = new byte[destination.Length];
            int read = destination.Read(destinationData, 0, destinationData.Length);

            Assert.Equal(destinationData.Length, read);
            Assert.Equal(firstValue, destinationData[0]);

            for (int i = 1; i < read; i++)
            {
                Assert.True((destinationData[i] == (destinationData[i - 1] + 1)) ||
                    ((destinationData[i] == 0) && (destinationData[i - 1] != 0)));
            }
        }

        private class MockByteArrayContent : ByteArrayContent
        {
            public int CopyToCount { get; private set; }

            public MockByteArrayContent(byte[] content, int offset, int count)
                : base(content, offset, count)
            {
            }

            protected override void SerializeToStream(Stream stream)
            {
                CopyToCount++;
                base.CopyTo(stream);
            }
        }

        #endregion
    }
}
