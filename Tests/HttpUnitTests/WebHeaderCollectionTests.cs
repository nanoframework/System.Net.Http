//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

using nanoFramework.TestFramework;
using System.Net;

namespace HttpUnitTests
{
    [TestClass]
    public class WebHeaderCollectionTests
    {
        [TestMethod]
        public void Add_Authorization_BearerWithSpaceAndNoValue_ShouldNotThrow()
        {
            var headers = new WebHeaderCollection();
            headers.Add("Authorization: Bearer ");
        }

        [TestMethod]
        public void Add_Authorization_NoSpaceSingleChar_ShouldNotThrow()
        {
            var headers = new WebHeaderCollection();
            headers.Add("Authorization: 1");
        }

        [TestMethod]
        public void Add_Authorization_ValidBearer_ShouldSucceed()
        {
            var headers = new WebHeaderCollection();
            headers.Add("Authorization: Bearer a11111");
            string value = headers["Authorization"];
            Assert.AreEqual("Bearer a11111", value);
        }

        [TestMethod]
        public void Add_Authorization_ValidTestValue_ShouldSucceed()
        {
            var headers = new WebHeaderCollection();
            headers.Add("Authorization: test 1");
            string value = headers["Authorization"];
            Assert.AreEqual("test 1", value);
        }

        [TestMethod]
        public void Add_Authorization_ValidSingleLetterPair_ShouldSucceed()
        {
            var headers = new WebHeaderCollection();
            headers.Add("Authorization: a b");
            string value = headers["Authorization"];
            Assert.AreEqual("a b", value);
        }
        [TestMethod]
        public void Add_Authorization_EmptyValue_ShouldNotThrow()
        {
            var headers = new WebHeaderCollection();
            headers.Add("Authorization:");
            string value = headers["Authorization"];
            Assert.AreEqual(string.Empty, value);
        }

        [TestMethod]
        public void Add_Authorization_ColonWithSpaceOnly_ShouldNotThrow()
        {
            var headers = new WebHeaderCollection();
            headers.Add("Authorization: ");
            string value = headers["Authorization"];
            Assert.AreEqual(string.Empty, value);
        }
    }
}
