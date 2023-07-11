//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright (c) Microsoft Corporation.  All rights reserved.
// See LICENSE file in the project root for full license information.
//

using System.Net.Http.Headers;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace System.Net.Http
{
    /// <summary>
    /// The default message handler used by HttpClient in .NET nanoFramework.
    /// </summary>
    public partial class HttpClientHandler : HttpMessageHandler
    {
        private NetworkCredential _credentials;
        private IWebProxy _proxy;
        private bool _useProxy = false;
        private TimeSpan _timeout = TimeSpan.Zero;

        private bool _disposed;
        private bool _sentRequest;
        private SslProtocols _sslProtocols;
        private X509Certificate _caCert;
        private X509Certificate _clientCert;
        private ClientCertificateOption _clientCertificateOptions = ClientCertificateOption.Manual;

        /// <summary>
        /// Gets or sets a value that indicates if the certificate is automatically picked from the certificate store or if the caller is allowed to pass in a specific client certificate.
        /// </summary>
        /// <value>The collection of security certificates associated with this handler.</value>
        public ClientCertificateOption ClientCertificateOptions
        {
            get
            {
                return _clientCertificateOptions;
            }

            set
            {
                EnsureModifiability();
                _clientCertificateOptions = value;
            }
        }

        /// <summary>
        /// Gets the collection of security certificates that are associated with requests to the server.
        /// </summary>
        /// <value>The <see cref="X509Certificate"/> that is presented to the server when performing certificate based client authentication.</value>
        /// <remarks>
        /// .NET nanoFramework only supports using one client certificate on the request, therefore this is not a collection has it happens with the equivalent .NET property.
        /// </remarks>
        public X509Certificate ClientCertificate
        {
            get
            {
                if (ClientCertificateOptions != ClientCertificateOption.Manual)
                {
                    throw new InvalidOperationException();
                }

                return _clientCert;
            }
        }

        /// <summary>
        /// Gets or sets authentication information used by this handler.
        /// </summary>
        /// <value>The authentication credentials associated with the handler. The default is null.</value>
        public NetworkCredential Credentials
        {
            get
            {
                return _credentials;
            }

            set
            {
                EnsureModifiability();
                _credentials = value;
            }
        }

        /// <summary>
        /// Gets or sets the TLS/SSL protocol used by the <see cref="HttpClientHandler"/> class.
        /// </summary>
        /// <value>
        /// One of the values defined in the <see cref="Security.SslProtocols"/> enumeration.
        /// </value>
        public SslProtocols SslProtocols
        {
            get
            {
                return _sslProtocols;
            }

            set
            {
                EnsureModifiability();
                _sslProtocols = value;
            }
        }

        /// <summary>
        /// Gets or sets proxy information used by the handler.
        /// </summary>
        /// <value>The proxy information used by the handler. The default value is null.</value>
        public IWebProxy Proxy
        {
            get
            {
                return _proxy;
            }

            set
            {
                EnsureModifiability();

                if (!_useProxy)
                {
                    throw new InvalidOperationException();
                }

                _proxy = value;
            }
        }

        /// <summary>
        /// Gets a value that indicates whether the handler supports automatic response content decompression.
        /// </summary>
        /// <value><see langword="true"/> if the if the handler supports automatic response content decompression; otherwise <see langword="false"/>. The default value is <see langword="false"/>.</value>
        /// <remarks>
        /// The default value for .NET nanoFramework is <see langword="false"/>.
        /// </remarks>
        public virtual bool SupportsAutomaticDecompression => false;

        /// <summary>
        /// Gets a value that indicates whether the handler supports proxy settings.
        /// </summary>
        /// <value><see langword="true"/> if the if the handler supports proxy settings; otherwise <see langword="false"/>. The default value is <see langword="true"/>.</value>
        public virtual bool SupportsProxy => true;

        /// <summary>
        /// Gets a value that indicates whether the handler supports configuration settings for the AllowAutoRedirect and MaxAutomaticRedirections properties.
        /// </summary>
        /// <value><see langword="true"/> if the if the handler supports configuration settings for the AllowAutoRedirect and MaxAutomaticRedirections properties; otherwise <see langword="false"/>. The default value is <see langword="false"/>.</value>
        /// <remarks>
        /// The default value for .NET nanoFramework is <see langword="false"/>.
        /// </remarks>
        public virtual bool SupportsRedirectConfiguration => false;

        /// <summary>
        /// Gets or sets a value that indicates whether the handler uses the CookieContainer property to store server cookies and uses these cookies when sending requests.
        /// </summary>
        /// <value><see langword="true"/> if the if the handler supports uses the CookieContainer property to store server cookies and uses these cookies when sending requests; otherwise <see langword="false"/>. The default value is <see langword="false"/>.</value>
        /// <remarks>
        /// The default value for .NET nanoFramework is <see langword="false"/>.
        /// </remarks>
        public bool UseCookies => false;

        /// <summary>
        /// Gets or sets a value that indicates whether the handler uses a proxy for requests.
        /// </summary>
        /// <value><see langword="true"/> if the handler should use a proxy for requests; otherwise <see langword="false"/>. The default value is <see langword="true"/>.</value>
        public bool UseProxy
        {
            get
            {
                return _useProxy;
            }

            set
            {
                EnsureModifiability();
                _useProxy = value;
            }
        }

        /// <summary>
        /// Creates an instance of a HttpClientHandler class.
        /// </summary>
        public HttpClientHandler()
        {
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                _disposed = true;
            }

            base.Dispose(disposing);
        }

        /// <inheritdoc/>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="HttpRequestException"></exception>
        protected internal override HttpResponseMessage Send(HttpRequestMessage request)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException();
            }

            var webRequest = CreateWebRequest(request);

            HttpWebResponse wresponse = null;

            try
            {
                var content = request.Content;

                if (content != null)
                {
                    var headers = webRequest.Headers;

                    foreach (var headerKey in content.Headers._headerStore.AllKeys)
                    {
                        headers.AddInternal(headerKey, content.Headers._headerStore[headerKey]);
                    }

                    if (request.Headers.TransferEncodingChunked == true)
                    {
                        webRequest.SendChunked = true;
                    }

                    // set content length
                    request.Content.TryComputeLength(out long lenght);
                    webRequest.ContentLength = lenght;

                    // set request sent flag
                    _sentRequest = true;

                    var stream = webRequest.GetRequestStream();

                    request.Content.ReadAsStream().CopyTo(stream);

                }
                else if (MethodHasBody(request.Method))
                {
                    // Explicitly set this to make sure we're sending a "Content-Length: 0" header.
                    webRequest.ContentLength = 0;

                    // set request sent flag
                    _sentRequest = true;
                }

                wresponse = (HttpWebResponse)webRequest.GetResponse();
            }
            catch (WebException we)
            {
                if (we.Status != WebExceptionStatus.RequestCanceled)
                {
                    throw new HttpRequestException("An error occurred while sending the request", we);
                }
            }
            catch (IO.IOException ex)
            {
                throw new HttpRequestException("An error occurred while sending the request", ex);
            }
            
            return CreateResponseMessage(wresponse, request);
        }

        private HttpWebRequest CreateWebRequest(HttpRequestMessage request)
        {
            var wr = new HttpWebRequest(request.RequestUri);

            wr.AllowWriteStreamBuffering = false;

            if (request.Version == HttpVersion.Version10)
            {
                wr.ProtocolVersion = HttpVersion.Version10;
            }
            else
            {
                wr.ProtocolVersion = request.Version;
            }

            wr.Method = request.Method.Method;

            if (wr.ProtocolVersion == HttpVersion.Version10)
            {
                wr.KeepAlive = GetConnectionKeepAlive(request.Headers);
            }
            else
            {
                wr.KeepAlive = request.Headers.ConnectionClose != true;
            }

            wr.Credentials = _credentials;

            if (_useProxy)
            {
                wr.Proxy = _proxy;
            }

            if (_timeout != TimeSpan.Zero)
            {
                wr.Timeout = (int)_timeout.TotalMilliseconds;
            }

            wr.SslProtocols = _sslProtocols;
            wr.HttpsAuthentCert = _caCert;

            if (ClientCertificateOptions == ClientCertificateOption.Manual)
            {
                wr._clientCert = ClientCertificate;
            }

            // Add request headers
            var headers = wr.Headers;
            foreach (var headerKey in request.Headers._headerStore.AllKeys)
            {
                headers.AddInternal(headerKey, request.Headers._headerStore[headerKey]);
            }

            return wr;
        }

        HttpResponseMessage CreateResponseMessage(HttpWebResponse wr, HttpRequestMessage requestMessage)
        {
            var response = new HttpResponseMessage(wr.StatusCode)
            {
                RequestMessage = requestMessage,
                ReasonPhrase = wr.StatusDescription
            };

            // set content
            response.Content = new StreamContent(wr.GetResponseStream());

            var headers = wr.Headers;

            foreach (var headerKey in headers.AllKeys)
            {
                response.Headers._headerStore.AddInternal(headerKey, headers[headerKey]);
                response.Content.Headers._headerStore.AddInternal(headerKey, headers[headerKey]);
            }

            requestMessage.RequestUri = wr.ResponseUri;

            return response;
        }

        bool GetConnectionKeepAlive(HttpRequestHeaders headers)
        {
            // In theory, the value should be lower case but it can with upper case.
            return headers.Connection.ToLower().Equals("keep-alive");
        }

        internal void EnsureModifiability()
        {
            // This instance has already started one or more requests.
            // Properties can only be modified before sending the first request.

            if (_sentRequest)
            {
                throw new InvalidOperationException();
            }
        }

        static bool MethodHasBody(HttpMethod method)
        {
            switch (method.Method)
            {
                case "HEAD":
                case "GET":
                case "MKCOL":
                case "CONNECT":
                case "TRACE":
                    return false;
                default:
                    return true;
            }
        }

        internal void SetWebRequestTimeout(TimeSpan timeout)
        {
            _timeout = timeout;
        }

        internal void SetWebRequestSslProcol(SslProtocols sslProtocols)
        {
            _sslProtocols = sslProtocols;
        }

        internal void SetWebRequestHttpAuthCert(X509Certificate certificate)
        {
            _caCert = certificate;
        }
    }
}
