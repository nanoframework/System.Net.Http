//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright (c) Microsoft Corporation.  All rights reserved.
// See LICENSE file in the project root for full license information.
//

using System.Net.Http.Http.Headers;

namespace System.Net.Http.Headers
{
    /// <summary>
    /// Represents the collection of Content Headers as defined in RFC 2616.
    /// </summary>
    public sealed class HttpContentHeaders : HttpHeaders
    {
        private readonly HttpContent _content;
        private bool _contentLengthSet;

		//private HttpHeaderValueCollection<string>? _allow;
		//private HttpHeaderValueCollection<string>? _contentEncoding;
		//private HttpHeaderValueCollection<string>? _contentLanguage;

		// TODO this one may not be required at all
		//public long ContentLength
		//{
		//	get
		//	{
		//		var contentLengthValue = _content.Headers._headerStore.GetValues(HttpKnownHeaderNames.ContentLength);

		//		if (contentLengthValue.Length > 0)
		//		{
		//			return Convert.ToInt64(contentLengthValue[0]);
		//		}

		//              if (_content.TryComputeLength(out long contentLength))
		//              {
		//                  _content.Headers._headerStore.Add(HttpKnownHeaderNames.ContentLength, contentLength.ToString());
		//                  return contentLength;
		//              }

		//              return -1;
		//	}

		//	set
		//	{
		//		_content.Headers._headerStore.Add(HttpKnownHeaderNames.ContentLength, value.ToString());
		//	}
		//}

		/// <summary>
		/// Gets or sets the value of the Content-Type content header on an HTTP response.
		/// </summary>
		/// <value>The value of the Content-Type content header on an HTTP response.</value>
		public MediaTypeHeaderValue ContentType
        {
			get
			{
				return MediaTypeHeaderValue.Parse(_headerStore[HttpKnownHeaderNames.ContentType]);
			}

			set
			{
				// build header value, OK to add ; even if CharSet is empty
				_headerStore.Add(HttpKnownHeaderNames.ContentType, value.ToString());
			}
		}

		internal HttpContentHeaders(HttpContent parent)
          : base(HttpHeaderType.Content | HttpHeaderType.Custom, HttpHeaderType.None)
        {
            _content = parent;
        }
    }
}
