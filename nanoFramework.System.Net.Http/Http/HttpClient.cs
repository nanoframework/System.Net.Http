//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright (c) Microsoft Corporation.  All rights reserved.
// See LICENSE file in the project root for full license information.
//

using System.IO;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

namespace System.Net.Http
{
    /// <summary>
    /// Initializes a new instance of the HttpClient class.
    /// </summary>
    /// <remarks>
    /// The HttpClient class instance acts as a session to send HTTP requests.
    /// An HttpClient instance is a collection of settings applied to all requests executed by that instance.
    /// In addition, every HttpClient instance uses its own connection pool,
    /// isolating its requests from requests executed by other HttpClient instances.
    ///
    /// HttpClient is intended to be instantiated once and reused throughout the life of an application. 
    /// </remarks>
    public partial class HttpClient : HttpMessageInvoker
    {
        private const HttpCompletionOption DefaultCompletionOption = HttpCompletionOption.ResponseContentRead;
        private Version _defaultRequestVersion = HttpRequestMessage.DefaultRequestVersion;

        private bool _operationStarted;
        private bool _disposed;

        private HttpRequestHeaders _headers;
        private Uri _baseAddress;
        private TimeSpan _timeout;

        /// <summary>
        /// Gets the headers which should be sent with each request.
        /// </summary>
        /// <value>
        /// The headers which should be sent with each request.
        /// </value>
        /// <remarks>
        /// Headers set on this property don't need to be set on request messages again. <see cref="DefaultRequestHeaders"/> should not be modified while there are outstanding requests, because it is not thread-safe.
        /// </remarks>
        public HttpRequestHeaders DefaultRequestHeaders => _headers ??= new HttpRequestHeaders();

        /// <summary>
        /// Gets or sets the base address of Uniform Resource Identifier (URI) of the Internet resource used when sending requests.
        /// </summary>
        /// <value>
        /// The base address of Uniform Resource Identifier (URI) of the Internet resource used when sending requests.
        /// </value>
        /// <exception cref="ArgumentException">Value is null or it not an absolute Uniform Resource Identifier (URI).</exception>
        /// <exception cref="InvalidOperationException">An operation has already been started on the current instance.</exception>
        /// <exception cref="ObjectDisposedException">The current instance has been disposed.</exception>
        public Uri BaseAddress
        {
            get => _baseAddress;

            set
            {
                // It's OK to not have a base address specified, but if one is, it needs to be absolute.
                if (value is not null
                    && !value.IsAbsoluteUri)
                {
                    throw new ArgumentException();
                }

                CheckDisposedOrStarted();

                _baseAddress = value;
            }
        }

        /// <summary>
        /// Gets or sets the timespan to wait before the request times out.
        /// </summary>
        /// <value>
        /// The timespan to wait before the request times out.
        /// </value>
        /// <remarks>
        /// <para>The default value is 100,000 milliseconds (100 seconds).</para>
        /// <para>To set an infinite timeout, set the property value to <see cref="Threading.Timeout.InfiniteTimeSpan"/>.</para>
        /// <para>
        /// A Domain Name System (DNS) query may take up to 15 seconds to return or time out. If your request contains a host name that requires resolution and you set <see cref="Timeout"/> to a value less than 15 seconds, it may take 15 seconds or more before a <see cref="WebException"/> is thrown to indicate a timeout on your request.
        /// </para>
        /// <para>
        /// The same timeout will apply for all requests using this <see cref="HttpClient"/> instance. You may also set different timeouts for individual requests using a CancellationTokenSource on a task.Note that only the shorter of the two timeouts will apply.
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentException">Value is null or it not an absolute Uniform Resource Identifier (URI).</exception>
        /// <exception cref="InvalidOperationException">An operation has already been started on the current instance.</exception>
        /// <exception cref="ObjectDisposedException">The current instance has been disposed.</exception>
        public TimeSpan Timeout
        {
            get => _timeout;

            set
            {
                if (value != Threading.Timeout.InfiniteTimeSpan && (value <= TimeSpan.Zero || value.TotalMilliseconds > int.MaxValue))
                {
                    throw new ArgumentOutOfRangeException();
                }

                CheckDisposedOrStarted();

                _timeout = value;
            }
        }

        /// <summary>
        /// Gets or sets the root CA certificate used to authenticate with https servers.
        /// This certificate is used only for https connections; http connections do not require this.
        /// </summary>
        /// <remarks>
        /// This property is an extension from the full .NET required by nanoFramework.
        /// </remarks>
        public X509Certificate HttpsAuthentCert { get; set; }

        /// <summary>
        /// Gets or sets the TLS/SSL protocol used by the <see cref="HttpClient"/> class.
        /// </summary>
        /// <value>
        /// One of the values defined in the <see cref="Security.SslProtocols"/> enumeration. Default value is <see cref="SslProtocols.Tls12"/>.
        /// </value>
        /// <remarks>
        /// This property is an extension from the full .NET required by nanoFramework.
        /// </remarks>
        public SslProtocols SslProtocols { get; set; } = SslProtocols.Tls12;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpClient"/> class using a <see cref="HttpClientHandler"/> that is disposed when this instance is disposed.
        /// </summary>
        public HttpClient() : base(
            new HttpClientHandler(),
            true)
        {
            _timeout = Threading.Timeout.InfiniteTimeSpan;

            // The default in REST API is to close the connection after each request.
            DefaultRequestHeaders.ConnectionClose = true;
        }

        #endregion Constructors

        #region REST Overloads

        /// <summary>
        /// Send a DELETE request to the specified Uri as a synchronous operation.
        /// </summary>
        /// <param name="requestUri">The <see cref="Uri"/> the request is sent to.</param>
        /// <returns>The <see cref="HttpResponseMessage"/> object resulting from the HTTP request.</returns>
        /// <exception cref="ArgumentNullException">The request is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">The request message was already sent by the <see cref="HttpClient"/> instance.</exception>
        /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, or server certificate validation.</exception>
        /// <remarks>
        /// <para>
        /// This operation will block.
        /// </para>
        /// <para>
        /// This is the .NET nanoFramework equivalent of DeleteAsync.
        /// </para>
        /// </remarks>
        public HttpResponseMessage Delete(string requestUri) => Send(new HttpRequestMessage(HttpMethod.Delete, requestUri), DefaultCompletionOption);

        /// <summary>
        /// Sends a GET request to the specified <see cref="Uri"/>.
        /// </summary>
        /// <param name="requestUri">The <see cref="Uri"/> the request is sent to.</param>
        /// <returns>The <see cref="HttpResponseMessage"/> object resulting from the HTTP request.</returns>
        /// <exception cref="InvalidOperationException">Request operation has already started.</exception>
        /// <exception cref="ArgumentNullException">The request is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">The request message was already sent by the <see cref="HttpClient"/> instance.</exception>
        /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, or server certificate validation.</exception>
        /// <remarks>
        /// <para>
        /// This operation will block.
        /// </para>
        /// <para>
        /// This is the .NET nanoFramework equivalent of GetAsync.
        /// </para>
        /// </remarks>
        public HttpResponseMessage Get(string requestUri) => Send(new HttpRequestMessage(HttpMethod.Get, requestUri), DefaultCompletionOption);

        /// <summary>
        /// Send a GET request to the specified Uri.
        /// </summary>
        /// <param name="requestUri">The <see cref="Uri"/> the request is sent to.</param>
        /// <param name="completionOption"></param>
        /// <returns>The <see cref="HttpResponseMessage"/> object resulting from the HTTP request.</returns>
        /// <exception cref="InvalidOperationException">Request operation has already started.</exception>
        /// <exception cref="ArgumentNullException">The request is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">The request message was already sent by the <see cref="HttpClient"/> instance.</exception>
        /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, or server certificate validation.</exception>
        /// <remarks>
        /// <para>
        /// This operation will block.
        /// </para>
        /// <para>
        /// This is the .NET nanoFramework equivalent of GetAsync.
        /// </para>
        /// </remarks>
        public HttpResponseMessage Get(string requestUri, HttpCompletionOption completionOption) => Send(new HttpRequestMessage(HttpMethod.Get, requestUri), completionOption);

        /// <summary>
        /// Sends a PATCH request as a synchronous operation.
        /// </summary>
        /// <param name="requestUri">The <see cref="Uri"/> the request is sent to.</param>
        /// <param name="content">The HTTP request content sent to the server.</param>
        /// <returns>The <see cref="HttpResponseMessage"/> object resulting from the HTTP request.</returns>
        /// <exception cref="ArgumentNullException">The request is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">The request message was already sent by the <see cref="HttpClient"/> instance.</exception>
        /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, or server certificate validation.</exception>
        /// <remarks>
        /// <para>
        /// This operation will block.
        /// </para>
        /// <para>
        /// This is the .NET nanoFramework equivalent of PatchAsync.
        /// </para>
        /// </remarks>
        public HttpResponseMessage Patch(string requestUri, HttpContent content) => Send(new HttpRequestMessage(HttpMethod.Patch, requestUri) { Content = content }, DefaultCompletionOption);

        /// <summary>
        /// Send a POST request as a synchronous operation.
        /// </summary>
        /// <param name="requestUri">The <see cref="Uri"/> the request is sent to.</param>
        /// <param name="content">The HTTP request content sent to the server.</param>
        /// <returns>The <see cref="HttpResponseMessage"/> object resulting from the HTTP request.</returns>
        /// <exception cref="ArgumentNullException">The request is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">The request message was already sent by the <see cref="HttpClient"/> instance.</exception>
        /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, or server certificate validation.</exception>
        /// <remarks>
        /// <para>
        /// This operation will block.
        /// </para>
        /// <para>
        /// This is the .NET nanoFramework equivalent of PostAsync.
        /// </para>
        /// </remarks>
        public HttpResponseMessage Post(string requestUri, HttpContent content) => Send(new HttpRequestMessage(HttpMethod.Post, requestUri) { Content = content }, DefaultCompletionOption);

        /// <summary>
        /// Send a PUT request as a synchronous operation.
        /// </summary>
        /// <param name="requestUri">The <see cref="Uri"/> the request is sent to.</param>
        /// <param name="content">The HTTP request content sent to the server.</param>
        /// <returns>The <see cref="HttpResponseMessage"/> object resulting from the HTTP request.</returns>
        /// <exception cref="ArgumentNullException">The request is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">The request message was already sent by the <see cref="HttpClient"/> instance.</exception>
        /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, or server certificate validation.</exception>
        /// <remarks>
        /// <para>
        /// This operation will block.
        /// </para>
        /// <para>
        /// This is the .NET nanoFramework equivalent of PutAsync.
        /// </para>
        /// </remarks>
        public HttpResponseMessage Put(string requestUri, HttpContent content) => Send(new HttpRequestMessage(HttpMethod.Put, requestUri) { Content = content }, DefaultCompletionOption);

        #endregion

        /// <summary>
        /// Sends a GET request to the specified Uri and return the response body as a byte array in an synchronous operation.
        /// </summary>
        /// <param name="requestUri">The <see cref="Uri"/> the request is sent to.</param>
        /// <returns>A byte array resulting from the HTTP request.</returns>
        /// <exception cref="ArgumentNullException">The request is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">The request message was already sent by the <see cref="HttpClient"/> instance.</exception>
        /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, or server certificate validation.</exception>
        /// <remarks>
        /// <para>
        /// This operation will block. It returns after the whole response body is read.
        /// </para>
        /// <para>
        /// This is the .NET nanoFramework equivalent of GetByteArrayAsync.
        /// </para>
        /// </remarks>
        public byte[] GetByteArray(string requestUri)
        {
            using var resp = Get(requestUri, HttpCompletionOption.ResponseContentRead);
            resp.EnsureSuccessStatusCode();

            return resp.Content.ReadAsByteArray();
        }

        /// <summary>
        /// Send a GET request to the specified Uri and return the response body as a stream in a synchronous operation.
        /// </summary>
        /// <param name="requestUri">The <see cref="Uri"/> the request is sent to.</param>
        /// <returns>A Stream resulting from the HTTP request.</returns>
        /// <exception cref="ArgumentNullException">The request is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">The request message was already sent by the <see cref="HttpClient"/> instance.</exception>
        /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, or server certificate validation.</exception>
        /// <remarks>
        /// <para>
        /// This operation will block.
        /// </para>
        /// <para>
        /// This method does not read nor buffer the response body. It will return as soon as the response headers are read.
        /// </para>
        /// <para>
        /// This is the .NET nanoFramework equivalent of GetStreamAsync.
        /// </para>
        /// </remarks>
        public Stream GetStream(string requestUri)
        {
            var resp = Get(requestUri, HttpCompletionOption.ResponseHeadersRead);
            resp.EnsureSuccessStatusCode();

            return resp.Content.ReadAsStream();
        }

        /// <summary>
        /// Send a GET request to the specified Uri and return the response body as a string in an synchronous operation.
        /// </summary>
        /// <param name="requestUri">The <see cref="Uri"/> the request is sent to.</param>
        /// <returns>A string resulting from the HTTP request.</returns>
        /// <exception cref="ArgumentNullException">The request is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">The request message was already sent by the <see cref="HttpClient"/> instance.</exception>
        /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, or server certificate validation.</exception>
        /// <remarks>
        /// <para>
        /// This operation will block.
        /// </para>
        /// <para>
        /// This operation will block. It returns after the whole response body is read.
        /// </para>
        /// <para>
        /// This is the .NET nanoFramework equivalent of GetStringAsync.
        /// </para>
        /// </remarks>
        public string GetString(string requestUri)
        {
            using HttpResponseMessage resp = Get(requestUri, HttpCompletionOption.ResponseContentRead);
            resp.EnsureSuccessStatusCode();

            return resp.Content.ReadAsString();
        }

        #region Advanced Send Overloads

        /// <summary>
        /// Sends an HTTP request with the specified request.
        /// </summary>
        /// <param name="request">The HTTP request message to send.</param>
        /// <param name="completionOption">One of the enumeration values that specifies when the operation should complete (as soon as a response is available or after reading the response content).</param>
        /// <returns>The HTTP response message.</returns>
        /// <exception cref="ArgumentNullException">The request is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">The request message was already sent by the <see cref="HttpClient"/> instance.</exception>
        /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, or server certificate validation.</exception>
        public HttpResponseMessage Send(
            HttpRequestMessage request,
            HttpCompletionOption completionOption)
        {
            if (request == null)
            {
                throw new ArgumentNullException();
            }

            if (request.SetIsUsed())
            {
                throw new InvalidOperationException();
            }

            var uri = request.RequestUri;

            if (uri == null)
            {
                if (_baseAddress == null)
                {
                    throw new InvalidOperationException();
                }

                request.RequestUri = _baseAddress;
            }
            else if (!uri.IsAbsoluteUri)
            {
                if (_baseAddress == null)
                {
                    throw new InvalidOperationException();
                }

                request.RequestUri = new Uri(_baseAddress, uri.OriginalString);
            }

            if (_headers != null)
            {
                request.Headers.AddHeaders(_headers);
            }

            return SendWorker(request, completionOption);
        }

        private HttpResponseMessage SendWorker(HttpRequestMessage request, HttpCompletionOption completionOption)
        {
            // need to pass to the HttpWebRequest:
            // - timeout
            // - SSL protocol
            // - CA root certs
            if (_handler is HttpClientHandler clientHandler)
            {
                clientHandler.SetWebRequestTimeout(_timeout);
                clientHandler.SetWebRequestSslProcol(SslProtocols);
                clientHandler.SetWebRequestHttpAuthCert(HttpsAuthentCert);
            }

            HttpResponseMessage response = base.Send(request);

            // Read the content when default HttpCompletionOption.ResponseContentRead is set
            if (response.Content != null && (completionOption & HttpCompletionOption.ResponseHeadersRead) == 0)
            {
                response.Content.LoadIntoBuffer();
            }

            return response;
        }

        #endregion

        #region helper methods

        private void SetOperationStarted()
        {
            // This method flags the HttpClient instances as "active". I.e. we executed at least one request (or are
            // in the process of doing so). This information is used to lock-down all property setters. Once a
            // Send operation is started, no property can be changed.
            if (!_operationStarted)
            {
                _operationStarted = true;
            }
        }

        private Uri CreateUri(string uri) => string.IsNullOrEmpty(uri) ? null : new Uri(uri, UriKind.RelativeOrAbsolute);

        private HttpRequestMessage CreateRequestMessage(HttpMethod method, string uri) => new HttpRequestMessage(method, uri) { Version = _defaultRequestVersion };

        private void CheckDisposedOrStarted()
        {
            CheckDisposed();

            if (_operationStarted)
            {
                throw new InvalidOperationException();
            }
        }

        private void CheckDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException();
            }
        }

        #endregion
    }
}

