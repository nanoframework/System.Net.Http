//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright (c) Microsoft Corporation.  All rights reserved.
// See LICENSE file in the project root for full license information.
//

using System;
using System.Net.Http.Headers;

namespace System.Net.Http
{
    public class HttpResponseMessage : IDisposable
    {
        private static Version DefaultResponseVersion => HttpVersion.Version11;

        private HttpStatusCode _statusCode;
        private HttpWebResponse _response;
        private HttpResponseHeaders _headers;
        //private HttpResponseHeaders _trailingHeaders;
        private string _reasonPhrase;
        private HttpRequestMessage _requestMessage;
        private Version _version;
        private HttpContent _content;
        private bool _disposed;

        public Version Version
        {
            get { return _version; }
            set
            {
                CheckDisposed();

                _version = value;
            }
        }

        public HttpContent Content
        {
            get { return _content; }
            set
            {
                CheckDisposed();

                _content = value;
            }
        }

        public HttpResponseHeaders Headers
        {
            get
            {
                return _headers ?? (_headers = new HttpResponseHeaders());
            }
        }


        public HttpStatusCode StatusCode
        {
            get { return _response.StatusCode; }
        }

        public string ReasonPhrase
        {
            get
            {
                return _reasonPhrase;
            }

            set
            {
                _reasonPhrase = value;
            }
        }

        public HttpRequestMessage RequestMessage
        {
            get { return _requestMessage; }

            set
            {
                CheckDisposed();

                _requestMessage = value;
            }
        }

        public bool IsSuccessStatusCode
        {
            get { return ((int)_statusCode >= 200) && ((int)_statusCode <= 299); }
        }

  

        public HttpResponseMessage()
            : this(HttpStatusCode.OK)
        {
        }

        public HttpResponseMessage(HttpStatusCode statusCode)
        {
            if (((int)statusCode < 0) || ((int)statusCode > 999))
            {
                throw new ArgumentOutOfRangeException();
            }

            _statusCode = statusCode;
            _version = DefaultResponseVersion;
        }

        public HttpResponseMessage EnsureSuccessStatusCode()
        {
            if (!IsSuccessStatusCode)
            {
                throw new HttpRequestException();
            }

            return this;
        }

        #region IDisposable Members

        /// <inheritdoc/>
        protected virtual void Dispose(bool disposing)
        {
            // The reason for this type to implement IDisposable is that it contains instances of types that implement
            // IDisposable (content).
            if (disposing && !_disposed)
            {
                _disposed = true;
                // TODO
                //if (_content != null)
                //{
                //    _content.Dispose();
                //}
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        private void CheckDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException();
            }
        }
    }
}
