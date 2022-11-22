//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright (c) Microsoft Corporation.  All rights reserved.
// See LICENSE file in the project root for full license information.
//

using System;
using System.Text;

namespace System.Web
{
    /// <summary>
    /// Utilities to encode and decode url
    /// </summary>
    public class HttpUtility
    {
        /// <summary>
        /// Encode an URL using UTF8
        /// </summary>
        /// <param name="str">The URL string to encode</param>
        /// <returns>The encoded string URL</returns>
        public static string UrlEncode(string str)
        {
            if ((str == null) || (str == string.Empty))
            {
                return string.Empty;
            }

            return new string(Encoding.UTF8.GetChars(UrlEncodeToBytes(str, Encoding.UTF8)));
        }

        /// <summary>
        /// Encode an URL
        /// </summary>
        /// <param name="str">The URL string to encode</param>
        /// <param name="e">The Encoding object that specifies the encoding scheme</param>
        /// <returns>The encoded string URL</returns>
        public static string UrlEncode(string str, Encoding e)
        {
            if ((str == null) || (str == string.Empty))
            {
                return string.Empty;
            }

            return new string(e.GetChars(UrlEncodeToBytes(str, e)));
        }

        /// <summary>
        /// Encode an URL
        /// </summary>
        /// <param name="bytes">The array of bytes to encode</param>
        /// <param name="offset">The position in the byte array at which to begin encoding</param>
        /// <param name="count">The number of bytes to encode</param>
        /// <returns>The encoded string URL</returns>
        public static string UrlEncode(byte[] bytes, int offset, int count)
        {
            return new string(Encoding.UTF8.GetChars(UrlEncodeBytesToBytesInternal(bytes, 0, bytes.Length, false)));
        }

        /// <summary>
        /// Encode an URL
        /// </summary>
        /// <param name="bytes">The array of bytes to encode</param>
        /// <returns>The encoded string URLL</returns>
        public static string UrlEncode(byte[] bytes) => UrlEncode(bytes, 0, bytes.Length);

        /// <summary>
        /// Encode an URL into a byte array
        /// </summary>
        /// <param name="str">The URL string to encode</param>
        /// <param name="e"></param>
        /// <returns>The encoded byte array</returns>
        public static byte[] UrlEncodeToBytes(string str, Encoding e)
        {
            if (str == null)
            {
                return null;
            }
            var bytes = e.GetBytes(str);
            return UrlEncodeBytesToBytesInternal(bytes, 0, bytes.Length, false);
        }

        /// <summary>
        /// Encode a byte array URL into a byte array
        /// </summary>
        /// <param name="bytes">The array of bytes to encode</param>
        /// <returns>The encoded byte array</returns>
        public static byte[] UrlEncodeToBytes(byte[] bytes) => UrlEncodeBytesToBytesInternal(bytes, 0, bytes.Length, false);

        /// <summary>
        /// Encode a byte array URL into a byte array
        /// </summary>
        /// <param name="bytes">The array of bytes to encode</param>
        /// <param name="offset">The position in the byte array at which to begin encoding</param>
        /// <param name="count">The number of bytes to encode</param>
        /// <returns>The encoded byte array</returns>		
        public static byte[] UrlEncodeToBytes(byte[] bytes, int offset, int count) => UrlEncodeBytesToBytesInternal(bytes, offset, count, false);

        private static byte[] UrlEncodeBytesToBytesInternal(byte[] bytes, int offset, int count, bool alwaysCreateReturnValue)
        {
            var num = 0;
            var num2 = 0;
            for (var i = 0; i < count; i++)
            {
                var ch = (char)bytes[offset + i];
                if (ch == ' ')
                {
                    num++;
                }
                else if (!IsSafe(ch))
                {
                    num2++;
                }
            }
            if ((!alwaysCreateReturnValue && (num == 0)) &&
                (num2 == 0))
            {
                return bytes;
            }
            var buffer = new byte[count + (num2 * 2)];
            var num4 = 0;
            for (var j = 0; j < count; j++)
            {
                var num6 = bytes[offset + j];
                var ch2 = (char)num6;
                if (IsSafe(ch2))
                {
                    buffer[num4++] = num6;
                }
                else if (ch2 == ' ')
                {
                    buffer[num4++] = 0x2b;
                }
                else
                {
                    buffer[num4++] = 0x25;
                    buffer[num4++] = (byte)IntToHex((num6 >> 4) & 15);
                    buffer[num4++] = (byte)IntToHex(num6 & 15);
                }
            }
            return buffer;
        }

        private static char IntToHex(int n)
        {
            if (n <= 9)
            {
                return (char)(n + 0x30);
            }
            return (char)((n - 10) + 0x61);
        }

        private static bool IsSafe(char ch)
        {
            if ((((ch >= 'a') && (ch <= 'z')) || ((ch >= 'A') && (ch <= 'Z'))) ||
                ((ch >= '0') && (ch <= '9')))
            {
                return true;
            }
            switch (ch)
            {
                case '(':
                case ')':
                case '*':
                case '-':
                case '.':
                case '_':
                case '!':
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Decode an UTF8 encoded URL
        /// </summary>
        /// <param name="url">The encoded URL</param>
        /// <returns>The decoded URL</returns>
        public static string UrlDecode(string url)
        {
            if ((url == null) || (url == string.Empty))
            {
                return string.Empty;
            }

            var data = Encoding.UTF8.GetBytes(url);
            return new string(Encoding.UTF8.GetChars(UrlDecodeToBytes(data, 0, data.Length)));
        }

        /// <summary>
        /// Decode an URL using the specified encoding
        /// </summary>
        /// <param name="str">The string to encode</param>
        /// <param name="e">The Encoding object that specifies the encoding scheme</param>
        /// <returns>The encoded string</returns>
        public static string UrlDecode(string str, Encoding e)
        {
            if ((str == null) || (str == string.Empty))
            {
                return string.Empty;
            }

            var data = e.GetBytes(str);
            return new string(e.GetChars(UrlDecodeToBytes(data, 0, data.Length)));
        }

        /// <summary>
        /// Decode an encoded URL
        /// </summary>
        /// <param name="bytes">The array of bytes to decode</param>
        /// <param name="offset">The position in the byte to begin decoding</param>
        /// <param name="count">The number of bytes to decode</param>
        /// <param name="e">The Encoding object that specifies the encoding scheme</param>
        /// <returns>The decoded URL</returns>
        public static string UrlDecode(byte[] bytes, int offset, int count, System.Text.Encoding e) => new string(e.GetChars(UrlDecodeToBytes(bytes, 0, bytes.Length)));

        /// <summary>
        /// Decode an encoded URL
        /// </summary>
        /// <param name="bytes">The array of bytes to decode</param>
        /// <param name="e">The Encoding object that specifies the encoding scheme</param>
        /// <returns>The decoded URL</returns>
        public static string UrlDecode(byte[] bytes, System.Text.Encoding e) => new string(e.GetChars(UrlDecodeToBytes(bytes, 0, bytes.Length)));

        /// <summary>
        /// Decode bytes array to byte array
        /// </summary>
        /// <param name="bytes">The array of bytes to decode</param>
        /// <param name="offset">The position in the byte array at which to begin decoding</param>
        /// <param name="count">The number of bytes to decode</param>
        /// <returns></returns>
        public static byte[] UrlDecodeToBytes(byte[] bytes, int offset, int count)
        {
            var length = 0;
            var sourceArray = new byte[count];
            for (var i = 0; i < count; i++)
            {
                var index = offset + i;
                var num4 = bytes[index];
                if (num4 == 0x2b)
                {
                    num4 = 0x20;
                }
                else if ((num4 == 0x25) &&
                         (i < (count - 2)))
                {
                    var num5 = HexToInt((char)bytes[index + 1]);
                    var num6 = HexToInt((char)bytes[index + 2]);
                    if ((num5 >= 0) &&
                        (num6 >= 0))
                    {
                        num4 = (byte)((num5 << 4) | num6);
                        i += 2;
                    }
                }
                sourceArray[length++] = num4;
            }
            if (length < sourceArray.Length)
            {
                var destinationArray = new byte[length];
                Array.Copy(sourceArray, destinationArray, length);
                sourceArray = destinationArray;
            }
            return sourceArray;
        }

        /// <summary>
        /// Decode bytes array to byte array
        /// </summary>
        /// <param name="str">The string to encode</param>
        /// <param name="e">The Encoding object that specifies the encoding scheme</param>
        /// <returns></returns>
        public static byte[] UrlDecodeToBytes(string str, System.Text.Encoding e)
        {
            var data = e.GetBytes(str);
            return UrlDecodeToBytes(data, 0, data.Length);
        }

        /// <summary>
        /// Decode bytes array to byte array in UTF8
        /// </summary>
        /// <param name="str">The string to encode</param>
        /// <returns></returns>
        public static byte[] UrlDecodeToBytes(string str) => UrlDecodeToBytes(str, Encoding.UTF8);

        /// <summary>
        /// Decode bytes array to byte array
        /// </summary>
        /// <param name="bytes">The array of bytes to decode</param>
        /// <returns></returns>
        public static byte[] UrlDecodeToBytes(byte[] bytes) => UrlDecodeToBytes(bytes, 0, bytes.Length);

        /// <summary>
        /// Get the int value of a char
        /// </summary>
        /// <param name="h">a char</param>
        /// <returns>The int value of the char</returns>
        public static int HexToInt(char h)
        {
            if ((h >= '0') &&
                (h <= '9'))
            {
                return (h - '0');
            }
            if ((h >= 'a') &&
                (h <= 'f'))
            {
                return ((h - 'a') + 10);
            }
            if ((h >= 'A') &&
                (h <= 'F'))
            {
                return ((h - 'A') + 10);
            }
            return -1;
        }
    }
}