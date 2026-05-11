//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//


using System.Net;
using nanoFramework.TestFramework;

namespace HttpUnitTests
{
    internal class HttpListenerRequestTests
    {
        // Verifies that malformed Authorization header (no space) does not cause a crash
        [TestMethod]
        public void Add_Authorization_NoSpaceMultipleChars_ShouldNotThrow()
        {
            var headers = new WebHeaderCollection();
            headers.Add("Authorization: a111111");
            string value = headers["Authorization"];
            Assert.AreEqual("a111111", value);
        }

        // Verifies that a properly formatted Authorization header (with space) is parsed and stored correctly
        [TestMethod]
        public void Add_Authorization_ValidBasicToken_ShouldSucceed()
        {
            var headers = new WebHeaderCollection();
            headers.Add("Authorization: Basic dXNlcjpwYXNz");
            string value = headers["Authorization"];
            Assert.AreEqual("Basic dXNlcjpwYXNz", value);
        }
    }
}
