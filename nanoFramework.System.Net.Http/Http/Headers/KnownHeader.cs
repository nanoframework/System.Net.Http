//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright (c) Microsoft Corporation.  All rights reserved.
// See LICENSE file in the project root for full license information.
//

using System.Diagnostics;

namespace System.Net.Http.Headers
{
    internal sealed partial class KnownHeader
    {
        public string Name { get; }

        public HttpHeaderType HeaderType { get; }

        /// <summary>
        /// If a raw string is a known value, this instance will be returned rather than allocating a new string.
        /// </summary>
        public string[] KnownValues { get; }

        public KnownHeader(
            string name,
            HttpHeaderType headerType,
            string[] knownValues = null)
        {
            Debug.Assert(!string.IsNullOrEmpty(name));

            Name = name;
            HeaderType = headerType;
            KnownValues = knownValues;

            // TODO
            //var asciiBytesWithColonSpace = new byte[name.Length + 2]; // + 2 for ':' and ' '
            //int asciiBytes = Encoding.UTF8.GetBytes(name, asciiBytesWithColonSpace, name.Length - asciiBytesWithColonSpace, asciiBytesWithColonSpace, 0);

            //Debug.Assert(asciiBytes == name.Length);
            
            //asciiBytesWithColonSpace[asciiBytesWithColonSpace.Length - 2] = (byte)':';
            //asciiBytesWithColonSpace[asciiBytesWithColonSpace.Length - 1] = (byte)' ';
            
            //AsciiBytesWithColonSpace = asciiBytesWithColonSpace;
        }
    }
}
