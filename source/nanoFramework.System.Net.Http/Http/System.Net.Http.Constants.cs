//
// Copyright (c) 2018 The nanoFramework project contributors
// Portions Copyright (c) Microsoft Corporation.  All rights reserved.
// See LICENSE file in the project root for full license information.
//

namespace System.Net
{
    internal class HttpConstants
    {
        /// <summary>
        /// The time we keep connection idle with HTTP 1.1
        /// This is one minute.
        /// </summary>
        internal const int DefaultKeepAliveMilliseconds = 60000;

        /// <summary>
        ///  maximum length of the line in reponse line 
        /// </summary>
        internal const int maxHTTPLineLength = 4000;
    }
}


