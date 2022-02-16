//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright (c) Microsoft Corporation.  All rights reserved.
// See LICENSE file in the project root for full license information.
//

using System.Net.Http.Headers;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

namespace System.Net.Http
{
    public partial class HttpClientHandler : HttpMessageHandler
    {
		//readonly IMonoHttpClientHandler _delegatingHandler;
		//private ClientCertificateOption _clientCertificateOptions;
		private NetworkCredential _credentials;
		private IWebProxy _proxy;
		private bool _useProxy = false;
		private TimeSpan _timeout = TimeSpan.Zero;

		private bool _disposed;
		private bool sentRequest;
        private SslProtocols _sslProtocols;
        private X509Certificate _caCert;

        public HttpClientHandler()
        {

            //_delegatingHandler = CreateDefaultHandler();
            //ClientCertificateOptions = ClientCertificateOption.Manual;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                _disposed = true;
                //                _delegatingHandler.Dispose();
            }

            base.Dispose(disposing);
        }

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

		public SslProtocols SslProtocols { get; set; }

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

		protected internal override HttpResponseMessage Send(HttpRequestMessage request)
        {
            // TODO

            if (_disposed)
            {
                throw new ObjectDisposedException();
            }

            var wrequest = CreateWebRequest(request);

			HttpWebResponse wresponse = null;

			try
			{
				var content = request.Content;

				if (content != null)
				{
					var headers = wrequest.Headers;

					foreach (var headerKey in content.Headers._headerStore.AllKeys)
					{
						headers.AddInternal(headerKey, content.Headers._headerStore[headerKey]);
					}

					if (request.Headers.TransferEncodingChunked == true)
					{
						wrequest.SendChunked = true;
					}
					//else
					//{
					//	//
					//	// Content length has to be set because HttpWebRequest is running without buffering
					//	//
					//	var contentLength = content.Headers.ContentLength;
					//	if (contentLength > -1)
					//	{
					//		wrequest.ContentLength = contentLength;
					//	}
					//	else
					//	{
					//		// TODO
					//		//if (MaxRequestContentBufferSize == 0)
					//  //                     {
					//  //                         throw new InvalidOperationException("The content length of the request content can't be determined. Either set TransferEncodingChunked to true, load content into buffer, or set MaxRequestContentBufferSize.");
					//  //                     }

					//  //                     await content.LoadIntoBufferAsync(MaxRequestContentBufferSize).ConfigureAwait(false);
					//		//wrequest.ContentLength = content.Headers.ContentLength.Value;
					//	}
					//}


					var stream = wrequest.GetRequestStream();
					
						// TODO replace with Stream.CopyTo when made available.
						byte[] buffer = new byte[2048];
						int read;

						while ((read = request.Content.ReadAsStream().Read(buffer, 0, buffer.Length)) != 0)
						{
							stream.Write(buffer, 0, read);
						}

						wrequest.ContentLength = read;
					
				}
				else if (MethodHasBody(request.Method))
				{
					// Explicitly set this to make sure we're sending a "Content-Length: 0" header.
					wrequest.ContentLength = 0;
				}

				wresponse = (HttpWebResponse)wrequest.GetResponse();
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

            //wr.ConnectionGroupName = connectionGroupName;
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
			else
			{
				// WebRequest.DefaultWebProxy it's already null
			}

			if (_timeout != TimeSpan.Zero)
            {
                wr.Timeout = (int)_timeout.TotalMilliseconds;
            }

			wr.SslProtocols = _sslProtocols;
			wr.HttpsAuthentCert = _caCert;

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
			var response = new HttpResponseMessage(wr.StatusCode);
			response.RequestMessage = requestMessage;
			response.ReasonPhrase = wr.StatusDescription;
			response.Content = new StreamContent(wr.GetResponseStream());

			var headers = wr.Headers;

			//for (int i = 0; i < headers.Count; ++i)
			//{
			//	var key = headers.GetKey(i);
			//	var value = headers.GetValues(i);

			//	HttpHeaders item_headers;

			//	if (key == HttpKnownHeaderNames.con))
   //             {
   //                 item_headers = response.Content.Headers;
   //             }
   //             else
   //             {
   //                 item_headers = response.Headers;
   //             }

   //             item_headers.TryAddWithoutValidation(key, value);
			//}

			requestMessage.RequestUri = wr.ResponseUri;

			return response;
		}

		bool GetConnectionKeepAlive(HttpRequestHeaders headers)
		{
			return headers.Connection.Equals("Keep-Alive");
		}

		internal void EnsureModifiability()
		{
			// This instance has already started one or more requests.
			// Properties can only be modified before sending the first request.

			if (sentRequest)
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
