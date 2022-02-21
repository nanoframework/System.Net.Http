﻿//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright (c) Microsoft Corporation.  All rights reserved.
// See LICENSE file in the project root for full license information.
//

namespace System.Net.Http
{
    /// <summary>
    /// A base type for HTTP message handlers.
    /// </summary>
    /// <remarks>
    /// <para>There are various HTTP message handlers that can be used. These include the following.</para>
    /// <list type="number">
    /// <item>HttpClientHandler - A class that operates at the bottom of the handler chain that actually handles the HTTP transport operations.</item>
    /// </list>
    /// <para>
    /// If developers derive classes from <see cref="HttpMessageHandler"/> and override the <see cref="Send"/> method, they must make sure that <see cref="Send"/> can get called concurrently by different threads.
    /// </para>
    /// </remarks>
    public abstract class HttpMessageHandler : IDisposable
    {
        /// <summary>
        /// When overridden in a derived class, sends an HTTP request with the specified request. Otherwise, throws a <see cref="NotSupportedException"/>.
        /// </summary>
        /// <param name="request">The HTTP request message to send.</param>
        /// <returns>The HTTP response message.</returns>
        /// <remarks>
        /// This is the .NET nanoFramework equivalent of Send(HttpRequestMessage).
        /// </remarks>
        protected internal abstract HttpResponseMessage Send(HttpRequestMessage request);

        #region IDisposable Members

        /// <inheritdoc/>
        protected virtual void Dispose(bool disposing)
        {
            // Nothing to do in base class.
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
