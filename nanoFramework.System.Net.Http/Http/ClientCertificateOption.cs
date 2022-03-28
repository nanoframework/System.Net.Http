//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright (c) Microsoft Corporation.  All rights reserved.
// See LICENSE file in the project root for full license information.
//

namespace System.Net.Http
{
	/// <summary>
	/// Specifies how client certificates are provided.
	/// </summary>
	public enum ClientCertificateOption
	{
        /// <summary>
        /// The application manually provides the client certificates to the WebRequestHandler. This value is the default.
        /// </summary>
        Manual = 0,

        /// <summary>
        /// The <see cref="HttpClientHandler"/> will attempt to provide all available client certificates automatically.
        /// </summary>
        Automatic
	}
}
