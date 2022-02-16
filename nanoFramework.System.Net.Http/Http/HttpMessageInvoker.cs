//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright (c) Microsoft Corporation.  All rights reserved.
// See LICENSE file in the project root for full license information.
//

using System.Threading;

namespace System.Net.Http
{
    public class HttpMessageInvoker : IDisposable
    {
        private bool _disposed;
        private readonly bool _disposeHandler;
        protected readonly HttpMessageHandler _handler;

        public HttpMessageInvoker(HttpMessageHandler handler)
            : this(handler, true)
        {
        }

        public HttpMessageInvoker(HttpMessageHandler handler, bool disposeHandler)
        {
            if(handler is null)
            {
                throw new ArgumentNullException();
            }

            _handler = handler;
            _disposeHandler = disposeHandler;
        }

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
