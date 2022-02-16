//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright (c) Microsoft Corporation.  All rights reserved.
// See LICENSE file in the project root for full license information.
//

namespace System.Net.Http
{
	[Serializable]
	public class HttpRequestException : Exception
	{
		public HttpRequestException()
		{
		}

		public HttpRequestException(string message)
			: base(message)
		{
		}

		public HttpRequestException(string message, Exception inner)
			: base(message, inner)
		{
		}
	}
}
