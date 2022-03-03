//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright (c) Microsoft Corporation.  All rights reserved.
// See LICENSE file in the project root for full license information.
//


namespace System.Net
{
    /// <summary>
    /// The interface to let its consumer know work is done
    /// </summary>
    internal interface IKnowWhenDone
    {
        /// <summary>
        /// The property reflects if work is done
        /// </summary>
        /// <returns>
        /// True when work is done.
        /// </returns>
        bool IsDone { get; }
    }
}
