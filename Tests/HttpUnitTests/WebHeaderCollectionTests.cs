//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

using nanoFramework.TestFramework;
using System;
using System.Net;

namespace HttpUnitTests
{
    [TestClass]
    public class WebHeaderCollectionTests
    {
        [TestMethod]
        public void Add_Authorization_BearerWithSpaceAndNoValue()
        {
            var headers = new WebHeaderCollection();
            headers.Add("Authorization: Bearer ");
        }

        [TestMethod]
        public void Add_Authorization_NoSpaceSingleChar()
        {
            var headers = new WebHeaderCollection();
            headers.Add("Authorization: 1");
        }

        [TestMethod]
        public void Add_Authorization_ValidBearer()
        {
            var headers = new WebHeaderCollection();
            headers.Add("Authorization: Bearer a11111");
            string value = headers["Authorization"];
            Assert.AreEqual("Bearer a11111", value);
        }

        [TestMethod]
        public void Add_Authorization_ValidTestValue()
        {
            var headers = new WebHeaderCollection();
            headers.Add("Authorization: test 1");
            string value = headers["Authorization"];
            Assert.AreEqual("test 1", value);
        }

        [TestMethod]
        public void Add_Authorization_ValidSingleLetterPair()
        {
            var headers = new WebHeaderCollection();
            headers.Add("Authorization: a b");
            string value = headers["Authorization"];
            Assert.AreEqual("a b", value);
        }
        [TestMethod]
        public void Add_Authorization_EmptyValue()
        {
            var headers = new WebHeaderCollection();
            headers.Add("Authorization:");
            string value = headers["Authorization"];
            Assert.AreEqual(string.Empty, value);
        }

        [TestMethod]
        public void Add_Authorization_ColonWithSpaceOnly()
        {
            var headers = new WebHeaderCollection();
            headers.Add("Authorization: ");
            string value = headers["Authorization"];
            Assert.AreEqual(string.Empty, value);
        }

        [TestMethod]
        public void Add_NullHeader_ThrowsArgumentNullException()
        {
            var headers = new WebHeaderCollection();
            Assert.ThrowsException(typeof(ArgumentNullException), () => headers.Add(null));
        }

        [TestMethod]
        public void Add_EmptyHeader_ThrowsArgumentNullException()
        {
            var headers = new WebHeaderCollection();
            Assert.ThrowsException(typeof(ArgumentNullException), () => headers.Add(string.Empty));
        }

        [TestMethod]
        public void Add_HeaderWithNoColon_ThrowsArgumentException()
        {
            var headers = new WebHeaderCollection();
            Assert.ThrowsException(typeof(ArgumentException), () => headers.Add("Authorization"));
        }

        [TestMethod]
        public void Add_HeaderNameWithSpace_ThrowsArgumentException()
        {
            var headers = new WebHeaderCollection();
            Assert.ThrowsException(typeof(ArgumentException), () => headers.Add("My Header: value"));
        }
    }
}
