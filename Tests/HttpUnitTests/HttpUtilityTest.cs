//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright (c) Microsoft Corporation.  All rights reserved.
// See LICENSE file in the project root for full license information.
//

using nanoFramework.TestFramework;
using System;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Web;

namespace HttpUnitTests
{
    [TestClass]
    public class HttpUtilityTest
    {

        [TestMethod]
        public void UrlDecodeNoThrow()
        {
            string str = "../../&amp;param2=%CURRREV%";

            Assert.AreEqual(str, HttpUtility.UrlDecode(str));
        }

        [TestMethod]
        public void UrlEncodeTest()
        {
            for (char c = char.MinValue; c < '\uD800'; c++)
            {
                byte[] bIn;
                bIn = Encoding.UTF8.GetBytes(c.ToString());
                MemoryStream expected = new MemoryStream();
                MemoryStream expUnicode = new MemoryStream();

                // build expected result for UrlEncode
                for (int i = 0; i < bIn.Length; i++)
                {
                    UrlEncodeChar((char)bIn[i], expected, false);
                }

                // build expected result for UrlEncodeUnicode
                UrlEncodeChar(c, expUnicode, true);

                byte[] bOut = expected.ToArray();

                string expectedResult = Encoding.UTF8.GetString(bOut, 0, bOut.Length);
                string actualResult = HttpUtility.UrlEncode(c.ToString());

                Assert.AreEqual(expectedResult, actualResult,
                    $"Expecting UrlEncode of '{c}' ({(int)c}) as [{expectedResult}] got {actualResult}");
            }
        }

        static void UrlEncodeChar(char c, Stream result, bool isUnicode)
        {
            if (c > 255)
            {
                int idx;
                int i = (int)c;

                result.WriteByte((byte)'%');
                result.WriteByte((byte)'u');
                idx = i >> 12;
                result.WriteByte((byte)hexChars[idx]);
                idx = (i >> 8) & 0x0F;
                result.WriteByte((byte)hexChars[idx]);
                idx = (i >> 4) & 0x0F;
                result.WriteByte((byte)hexChars[idx]);
                idx = i & 0x0F;
                result.WriteByte((byte)hexChars[idx]);
                return;
            }

            if (c > ' ' && notEncoded.IndexOf(c) != -1)
            {
                result.WriteByte((byte)c);
                return;
            }
            if (c == ' ')
            {
                result.WriteByte((byte)'+');
                return;
            }
            if ((c < '0') ||
                (c < 'A' && c > '9') ||
                (c > 'Z' && c < 'a') ||
                (c > 'z'))
            {
                if (isUnicode && c > 127)
                {
                    result.WriteByte((byte)'%');
                    result.WriteByte((byte)'u');
                    result.WriteByte((byte)'0');
                    result.WriteByte((byte)'0');
                }
                else
                {
                    result.WriteByte((byte)'%');
                }

                int idx = ((int)c) >> 4;
                result.WriteByte((byte)hexChars[idx]);
                idx = c & 0x0F;
                result.WriteByte((byte)hexChars[idx]);
            }
            else
            {
                result.WriteByte((byte)c);
            }
        }

        static char[] hexChars = "0123456789ABCDEF".ToCharArray();
        const string notEncoded = "~-._";
    }
}
