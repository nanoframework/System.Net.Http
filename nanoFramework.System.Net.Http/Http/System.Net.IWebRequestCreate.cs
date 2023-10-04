//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright (c) Microsoft Corporation.  All rights reserved.
// See LICENSE file in the project root for full license information.
//

namespace System.Net
{
    /// <summary>
    /// The interface for creating <see cref="WebRequest"/> class
    /// objects.
    /// </summary>
    public interface IWebRequestCreate
    {
        /// <summary>
        /// Creates an instance of a class derived from
        /// <itemref>WebRequest</itemref>.
        /// </summary>
        /// <param name="uri">The URI for initialization of the class that is
        /// derived from <itemref>WebRequest</itemref>.</param>
        /// <returns>
        /// An instance of the class that is derived from
        /// <itemref>WebRequest</itemref>.
        /// </returns>
        WebRequest Create(Uri uri);
    }
}
