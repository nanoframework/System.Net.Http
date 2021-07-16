//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright (c) Microsoft Corporation.  All rights reserved.
// See LICENSE file in the project root for full license information.
//

namespace System.Net
{
    internal class HttpConstants
    {
        /// <summary>
        /// Default time (ms) to keep a persistent connection open
        /// </summary>
        internal const int DefaultKeepAliveMilliseconds = 600000;

        /// <summary>
        ///  maximum length of the line in reponse line 
        /// </summary>
        internal const int maxHTTPLineLength = 4000;
    }
}


