//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright (c) Microsoft Corporation.  All rights reserved.
// See LICENSE file in the project root for full license information.
//

namespace System.Net.Http.Headers
{
    [Flags]
    internal enum HttpHeaderType : byte
    {
        General = 0b1,
        Request = 0b10,
        Response = 0b100,
        Content = 0b1000,
        Custom = 0b10000,
        NonTrailing = 0b100000,

        All = 0b111111,
        None = 0
    }
}
