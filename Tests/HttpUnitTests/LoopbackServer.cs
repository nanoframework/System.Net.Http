﻿//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright (c) Microsoft Corporation.  All rights reserved.
// See LICENSE file in the project root for full license information.
//

using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;

namespace HttpUnitTests
{
    public sealed partial class LoopbackServer : IDisposable
    {
        private Socket _listenSocket;
        private Options _options;
        private Uri _uri;

        public Socket ListenSocket => _listenSocket;
        public Uri Uri => _uri;

        // Use CreateServerAsync or similar to create
        private LoopbackServer(Socket listenSocket, Options options)
        {
            _listenSocket = listenSocket;
            _options = options;

            var localEndPoint = (IPEndPoint)listenSocket.LocalEndPoint;

            string host = options.Address.AddressFamily == AddressFamily.InterNetworkV6 ?
                $"[{localEndPoint.Address}]" :
                localEndPoint.Address.ToString();

            string scheme = options.UseSsl ? "https" : "http";

            if (options.WebSocketEndpoint)
            {
                scheme = options.UseSsl ? "wss" : "ws";
            }

            _uri = new Uri($"{scheme}://{host}:{localEndPoint.Port}/");
        }

        public static LoopbackServer CreateServer(Options options = null)
        {
            options = options ?? new Options();

            using var listenSocket = new Socket(options.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            listenSocket.Bind(new IPEndPoint(options.Address, 0));
            listenSocket.Listen(options.ListenBacklog);

            using LoopbackServer server = new LoopbackServer(listenSocket, options);

            return server;
        }

        public ArrayList AcceptConnection(HttpStatusCode statusCode = HttpStatusCode.OK, string additionalHeaders = null, string content = null)
        {
            using Socket s = _listenSocket.Accept();

            //s.NoDelay = true;

            Stream stream = new NetworkStream(s, ownsSocket: false);

            if (_options.UseSsl)
            {
                // TODO rework code to follow what we're doing in HttpWebRequest
                //var sslStream = new SslStream(s);

                //using (var cert = Configuration.Certificates.GetServerCertificate())
                //{
                //    sslStream.AuthenticateAsServer(
                //        cert,
                //        clientCertificateRequired: true, // allowed but not required
                //        enabledSslProtocols: _options.SslProtocols,
                //        checkCertificateRevocation: false);
                //}
                //stream = sslStream;
            }

            if (_options.StreamWrapper != null)
            {
                stream = _options.StreamWrapper(stream);
            }

            using var connection = new Connection(s, stream);
            return connection.ReadRequestHeaderAndSendResponse(statusCode, additionalHeaders, content);
        }

        public ArrayList AcceptConnectionSendResponseAndClose(HttpStatusCode statusCode = HttpStatusCode.OK, string additionalHeaders = null, string content = null)
        {
            ArrayList lines = null;

            // Note, we assume there's no request body.  
            // We'll close the connection after reading the request header and sending the response.
            return AcceptConnection(statusCode, additionalHeaders, content); ;
        }

        public void Dispose()
        {
            if (_listenSocket != null)
            {
                _listenSocket.Close();
                _listenSocket = null;
            }
        }


        // Stolen from HttpStatusDescription code in the product code
        private static string GetStatusDescription(HttpStatusCode code)
        {
            switch ((int)code)
            {
                case 100:
                    return "Continue";
                case 101:
                    return "Switching Protocols";
                case 102:
                    return "Processing";

                case 200:
                    return "OK";
                case 201:
                    return "Created";
                case 202:
                    return "Accepted";
                case 203:
                    return "Non-Authoritative Information";
                case 204:
                    return "No Content";
                case 205:
                    return "Reset Content";
                case 206:
                    return "Partial Content";
                case 207:
                    return "Multi-Status";

                case 300:
                    return "Multiple Choices";
                case 301:
                    return "Moved Permanently";
                case 302:
                    return "Found";
                case 303:
                    return "See Other";
                case 304:
                    return "Not Modified";
                case 305:
                    return "Use Proxy";
                case 307:
                    return "Temporary Redirect";

                case 400:
                    return "Bad Request";
                case 401:
                    return "Unauthorized";
                case 402:
                    return "Payment Required";
                case 403:
                    return "Forbidden";
                case 404:
                    return "Not Found";
                case 405:
                    return "Method Not Allowed";
                case 406:
                    return "Not Acceptable";
                case 407:
                    return "Proxy Authentication Required";
                case 408:
                    return "Request Timeout";
                case 409:
                    return "Conflict";
                case 410:
                    return "Gone";
                case 411:
                    return "Length Required";
                case 412:
                    return "Precondition Failed";
                case 413:
                    return "Request Entity Too Large";
                case 414:
                    return "Request-Uri Too Long";
                case 415:
                    return "Unsupported Media Type";
                case 416:
                    return "Requested Range Not Satisfiable";
                case 417:
                    return "Expectation Failed";
                case 422:
                    return "Unprocessable Entity";
                case 423:
                    return "Locked";
                case 424:
                    return "Failed Dependency";
                case 426:
                    return "Upgrade Required"; // RFC 2817

                case 500:
                    return "Internal Server Error";
                case 501:
                    return "Not Implemented";
                case 502:
                    return "Bad Gateway";
                case 503:
                    return "Service Unavailable";
                case 504:
                    return "Gateway Timeout";
                case 505:
                    return "Http Version Not Supported";
                case 507:
                    return "Insufficient Storage";
            }
            return null;
        }

        public static string GetHttpResponse(HttpStatusCode statusCode = HttpStatusCode.OK, string additionalHeaders = null, string content = null) =>
            $"HTTP/1.1 {(int)statusCode} {GetStatusDescription(statusCode)}\r\n" +
            $"Date: {DateTime.UtcNow:R}\r\n" +
            $"Content-Length: {(content == null ? 0 : content.Length)}\r\n" +
            additionalHeaders +
            "\r\n" +
            content;

        public static string GetSingleChunkHttpResponse(HttpStatusCode statusCode = HttpStatusCode.OK, string additionalHeaders = null, string content = null) =>
            $"HTTP/1.1 {(int)statusCode} {GetStatusDescription(statusCode)}\r\n" +
            $"Date: {DateTime.UtcNow:R}\r\n" +
            "Transfer-Encoding: chunked\r\n" +
            additionalHeaders +
            "\r\n" +
            (string.IsNullOrEmpty(content) ? "" :
                $"{content.Length:X}\r\n" +
                $"{content}\r\n") +
            $"0\r\n" +
            $"\r\n";

        public class Options
        {
            public IPAddress Address { get; set; } = IPAddress.Loopback;
            public int ListenBacklog { get; set; } = 1;
            public bool UseSsl { get; set; } = false;
            public SslProtocols SslProtocols { get; set; } = SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Tls12;
            public bool WebSocketEndpoint { get; set; } = false;
            public Func<Stream, Stream> StreamWrapper { get; set; }
            public string Username { get; set; }
            public string Domain { get; set; }
            public string Password { get; set; }
            public bool IsProxy { get; set; } = false;
        }

        public sealed class Connection : IDisposable
        {
            private Socket _socket;
            private Stream _stream;
            private StreamReader _reader;
            private StreamWriter _writer;

            public Connection(Socket socket, Stream stream)
            {
                _socket = socket;
                _stream = stream;

                _reader = new StreamReader(stream);
                _writer = new StreamWriter(stream);
            }

            public Socket Socket => _socket;
            public Stream Stream => _stream;
            public StreamReader Reader => _reader;
            public StreamWriter Writer => _writer;

            public void Dispose()
            {
                try
                {
                    // Try to shutdown the send side of the socket.
                    // This seems to help avoid connection reset issues caused by buffered data
                    // that has not been sent/acked when the graceful shutdown timeout expires.
                    // This may throw if the socket was already closed, so eat any exception.
                    // TODO need to investigate what is the appropriate pattern to use with nanoFramework
                    //_socket.Shutdown(SocketShutdown.Send);
                }
                catch (Exception) { }

                _reader.Dispose();
                _writer.Dispose();
                _stream.Dispose();
                _socket = null;
            }

            public ArrayList ReadRequestHeader()
            {
                ArrayList lines = new ArrayList();
                string line;

                while (!string.IsNullOrEmpty(line = _reader.ReadLine()))
                {
                    lines.Add(line);
                }

                if (line == null)
                {
                    throw new Exception("Unexpected EOF trying to read request header");
                }

                return lines;
            }

            public void SendResponse(HttpStatusCode statusCode = HttpStatusCode.OK, string additionalHeaders = null, string content = null)
            {
                _writer.Write(GetHttpResponse(statusCode, additionalHeaders, content));
            }

            public ArrayList ReadRequestHeaderAndSendCustomResponse(string response)
            {
                ArrayList lines = ReadRequestHeader();
                _writer.Write(response);
                return lines;
            }

            public ArrayList ReadRequestHeaderAndSendResponse(HttpStatusCode statusCode = HttpStatusCode.OK, string additionalHeaders = null, string content = null)
            {
                ArrayList lines = ReadRequestHeader();
                SendResponse(statusCode, additionalHeaders, content);
                return lines;
            }
        }
    }
}
