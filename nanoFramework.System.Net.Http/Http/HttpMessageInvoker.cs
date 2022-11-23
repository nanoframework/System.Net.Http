//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright (c) Microsoft Corporation.  All rights reserved.
// See LICENSE file in the project root for full license information.
//

namespace System.Net.Http
{
    /// <summary>
    /// A specialty class that allows applications to call the <see cref="Send"/>(HttpRequestMessage) method on an HTTP handler chain.
    /// </summary>
    public class HttpMessageInvoker : IDisposable
    {
        private bool _disposed;
        private readonly bool _disposeHandler;
        internal readonly HttpMessageHandler _handler;

        /// <summary>
        /// Initializes an instance of a <see cref="HttpMessageInvoker"/> class with a specific <see cref="HttpMessageHandler"/>.
        /// </summary>
        /// <param name="handler">The <see cref="HttpMessageHandler"/> responsible for processing the HTTP response messages.</param>
        public HttpMessageInvoker(HttpMessageHandler handler)
            : this(handler, true)
        {
        }

        /// <summary>
        /// Initializes an instance of a <see cref="HttpMessageInvoker"/> class with a specific <see cref="HttpMessageHandler"/>.
        /// </summary>
        /// <param name="handler">The <see cref="HttpMessageHandler"/> responsible for processing the HTTP response messages.</param>
        /// <param name="disposeHandler"><see langword="true"/> if the inner handler should be disposed of by <see cref="HttpMessageInvoker.Dispose"/>, <see langword="false"/> if you intend to reuse the inner handler.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public HttpMessageInvoker(
            HttpMessageHandler handler,
            bool disposeHandler)
        {
            if (handler is null)
            {
                throw new ArgumentNullException();
            }

            _handler = handler;
            _disposeHandler = disposeHandler;
        }

        /// <summary>
        /// Sends an HTTP request.
        /// </summary>
        /// <param name="request">The HTTP request message to send.</param>
        /// <returns>The HTTP response message.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="request"/> was <see langword="null"/>.</exception>
        public virtual HttpResponseMessage Send(HttpRequestMessage request)
        {
            if (request is null)
            {
                throw new ArgumentNullException();
            }

            CheckDisposed();

            return _handler.Send(request);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc/>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                _disposed = true;

                if (_disposeHandler)
                {
                    _handler.Dispose();
                }
            }
        }

        private void CheckDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException();
            }
        }
    }
}
