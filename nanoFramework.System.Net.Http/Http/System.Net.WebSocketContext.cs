//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright (c) Microsoft Corporation.  All rights reserved.
// See LICENSE file in the project root for full license information.
//

using System.Net.Sockets;

namespace System.Net
{
    /// <summary>
    /// Provides access to the networkStream and socket for creating a websocket
    /// <itemref>HttpListener</itemref> class.  This class cannot be inherited.
    /// </summary>
    internal class WebSocketContext
    {
        /// <summary>
        /// Actual network or SSL stream connected to the client.
        /// It could be SSL stream, so NetworkStream is not exact type, m_Stream would be derived from NetworkStream
        /// </summary>
        internal NetworkStream NetworkStream;

        /// <summary>
        /// This is a socket connected to client.
        /// OutputNetworkStreamWrapper owns the socket, not NetworkStream.
        /// If connection is persistent, then the m_Socket is transferred to the list of
        /// </summary>
        internal Socket Socket;

        /// <summary>
        /// Gets the collection of header name/value pairs sent in the request.
        /// </summary>
        /// <value>A <itemref>WebHeaderCollection</itemref> that contains the
        /// HTTP headers included in the request.</value>
        internal WebHeaderCollection Headers;

        internal WebSocketContext(Socket socket, NetworkStream networkStream, WebHeaderCollection headers)
        {
            Socket = socket;
            NetworkStream = networkStream;
            Headers = headers;

        }
    }
}
