using System;
using System.Text;
using System.Net.Sockets;

namespace System.Net
{
    /// <summary>
    /// Provides access to the networkStream and socket for creating a websocket
    /// <itemref>HttpListener</itemref> class.  This class cannot be inherited.
    /// </summary>
    public class WebSocketContext : HttpListenerContext
    {
        /// <summary>
        /// Actual network or SSL stream connected to the client.
        /// It could be SSL stream, so NetworkStream is not exact type, m_Stream would be derived from NetworkStream
        /// </summary>
        public NetworkStream m_networkStream;

        /// <summary>
        /// This is a socket connected to client.
        /// OutputNetworkStreamWrapper owns the socket, not NetworkStream.
        /// If connection is persistent, then the m_Socket is transferred to the list of
        /// </summary>
        public Socket m_socket;
        
        internal WebSocketContext(HttpListenerContext httpListenerContext) : base(httpListenerContext.m_clientOutputStream, httpListenerContext.m_ResponseToClient.m_Listener)
        {
            m_socket = httpListenerContext.m_clientOutputStream.m_Socket;
            m_networkStream = httpListenerContext.m_clientOutputStream.m_Stream;
        }
    }

}
