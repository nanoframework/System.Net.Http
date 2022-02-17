//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright (c) Microsoft Corporation.  All rights reserved.
// See LICENSE file in the project root for full license information.
//

using System;
using System.Net.Http.Http.Headers;

namespace System.Net.Http.Headers
{
    public sealed class HttpContentHeaders : HttpHeaders
    {
        private readonly HttpContent _content;
        private bool _contentLengthSet;

		//private HttpHeaderValueCollection<string>? _allow;
		//private HttpHeaderValueCollection<string>? _contentEncoding;
		//private HttpHeaderValueCollection<string>? _contentLanguage;

		public long ContentLength
		{
			get
			{
				var contentLengthValue = _content.Headers._headerStore.GetValues(HttpKnownHeaderNames.ContentLength);

				if (contentLengthValue.Length > 0)
				{
					return Convert.ToInt64(contentLengthValue[0]);
				}

                //v = _content.LoadedBufferLength;
                //if (v != null)
                //	return v;

                if (_content.TryComputeLength(out long contentLength))
                {
                    _content.Headers._headerStore.Add(HttpKnownHeaderNames.ContentLength, contentLength.ToString());
                    return contentLength;
                }

                return -1;
			}

			set
			{
				_content.Headers._headerStore.Add(HttpKnownHeaderNames.ContentLength, value.ToString());
			}
		}

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
