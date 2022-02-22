//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright (c) Microsoft Corporation.  All rights reserved.
// See LICENSE file in the project root for full license information.
//

using nanoFramework.TestFramework;
using System;
using System.Net.Http;
using System.Threading;

namespace HttpUnitTests
{
    [TestClass]
    public class HttpClientTest
    {
        [Setup]
        public void SetupHttpClientTest()
        {
            Assert.SkipTest("Can't run HttpClient unit tests on WIN32 nanoCLR. No network support.");
        }

        [TestMethod]
        public void Dispose_MultipleTimes_Success()
        {
            HttpClient client = CreateHttpClient();
            client.Dispose();
            client.Dispose();
        }

        [TestMethod]
        public void DefaultRequestHeaders_Idempotent()
        {
            using (HttpClient client = CreateHttpClient())
            {
                Assert.NotNull(client.DefaultRequestHeaders);
                Assert.Same(client.DefaultRequestHeaders, client.DefaultRequestHeaders);
            }
        }

        [TestMethod]
        public void BaseAddress_Roundtrip_Equal()
        {
            using (HttpClient client = CreateHttpClient())
            {
                Assert.Null(client.BaseAddress);

                Uri uri = new Uri(CreateFakeUri());
                client.BaseAddress = uri;
                Assert.Equal(uri.ToString(), client.BaseAddress.ToString());

                client.BaseAddress = null;
                Assert.Null(client.BaseAddress);
            }
        }

        [TestMethod]
        public void BaseAddress_InvalidUri_Throws()
        {
            using (HttpClient client = CreateHttpClient())
            {
                Assert.Throws(typeof(ArgumentException),
                    () => client.BaseAddress = new Uri("ftp://onlyhttpsupported"));

                Assert.Throws(typeof(ArgumentException),
                    () => client.BaseAddress = new Uri("/onlyabsolutesupported", UriKind.Relative));
            }
        }

        [TestMethod]
        public void Timeout_Roundtrip_Equal()
        {
            using (HttpClient client = CreateHttpClient())
            {
                client.Timeout = Timeout.InfiniteTimeSpan;
                Assert.Equal(Timeout.InfiniteTimeSpan.Ticks, client.Timeout.Ticks);

                client.Timeout = TimeSpan.FromSeconds(1);
                Assert.Equal(TimeSpan.FromSeconds(1).Ticks, client.Timeout.Ticks);
            }
        }

        [TestMethod]
        public void Timeout_OutOfRange_Throws()
        {
            using (HttpClient client = CreateHttpClient())
            {
                Assert.Throws(typeof(ArgumentOutOfRangeException),
                    () => client.Timeout = TimeSpan.FromSeconds(-2));
                Assert.Throws(typeof(ArgumentOutOfRangeException), 
                    () => client.Timeout = TimeSpan.FromSeconds(0));
                Assert.Throws(typeof(ArgumentOutOfRangeException),
                    () => client.Timeout = TimeSpan.FromSeconds(int.MaxValue));
            }
        }

        [TestMethod]
        public void MaxResponseContentBufferSize_ThrowsIfTooSmallForContent(int maxSize, int contentLength, bool exceptionExpected)
        {
            maxSize = 1;
            contentLength = 2;
            exceptionExpected = true;

            using HttpClient client = CreateHttpClient();

            var server = LoopbackServer.CreateServer();

            var waithandles = new WaitHandle[2] { new AutoResetEvent(false), new AutoResetEvent(false) };

            Thread clientThread = new Thread(() =>
            {
                string getTask = client.GetString(server.Uri.AbsoluteUri);
                ((AutoResetEvent)waithandles[0]).Set();
            });

            Thread serverThread = new Thread(() =>
            {
                if (exceptionExpected)
                {
                    Assert.Throws(typeof(HttpRequestException),
                        () =>
                        {
                            _ = server.AcceptConnectionSendResponseAndClose(content: new string('s', contentLength));
                        });
                }

                ((AutoResetEvent)waithandles[1]).Set();
            });

            WaitHandle.WaitAll(waithandles);
        }


        private HttpClient CreateHttpClient()
        {
            return new HttpClient();
        }

        private static string CreateFakeUri() => $"http://{Guid.NewGuid()}";
    }
}
