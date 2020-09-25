//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright (c) Microsoft Corporation.  All rights reserved.
// See LICENSE file in the project root for full license information.
//

namespace System.Net
{
    /// <summary>
    /// Network authentication type.
    /// Currently supports:
    /// Basic Authentication
    /// Microsoft Live Id Delegate Authentication
    /// </summary>
    public enum AuthenticationType
    {
        /// <summary>
        /// 
        /// </summary>
        Basic,
        /// <summary>
        /// 
        /// </summary>
        WindowsLive
    };
}
