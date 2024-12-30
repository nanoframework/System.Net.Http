//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright (c) Microsoft Corporation.  All rights reserved.
// See LICENSE file in the project root for full license information.
//

using nanoFramework.TestFramework;
using System;

namespace HttpUnitTests
{
    [TestClass]
    public class UriUnitTests
    {
        [TestMethod]
        [DataRow("ftp://ftp.example.com")] // ftp
        [DataRow("ftp://username:password@ftp.example.com")] // ftp with username and password
        [DataRow("ftp://ftp.example.com/folder/file.txt")] // ftp with file path
        [DataRow("ftp://username:password@ftp.example.com:22/folder/file.pe")] // ftp with username and password, custom post, and file
        [DataRow("\tftp://abc.com   ")] // ftp with tabs
        [DataRow("file://")] // Empty file path
        [DataRow("file:///")]
        [DataRow("file:////")]
        [DataRow("file://c")]
        [DataRow("file:///c")]
        [DataRow("file:////c")]
        [DataRow("file:///C:/path/to/file.txt")] // local file path
        [DataRow("file:///home/user/file.txt")] // unix file path
        [DataRow("file://hostname/share/path/file.txt")] // network share
        [DataRow("file:///\\\\server\\folder\\file.txt")] // Windows share style file
        [DataRow("file:///c:\\rbllog")] // Windows drive style file
        [DataRow("mailto:user@example.com")] // email
        [DataRow("mailto:user@example.com?subject=Hello&body=World")] // email with subject and body
        [DataRow("postgresql://user:password@localhost:5432/database")] // postgresql
        [DataRow("mongodb://user:password@localhost:27017/database")] // mongo db
        [DataRow("ws://example.com/socket")] // web socket
        [DataRow("ws://ws.pusherapp.com:80/app/?client=js&version=1.9.3&protocol=5")] // web socket with query, path, and port
        [DataRow("wss://example.com/socket")] // Secure web socket 
        [DataRow("wss://ws.pusherapp.com/app/?client=js&version=1.9.3&protocol=5")] // secure web socket with query, path, and port
        [DataRow("ssh://user@hostname:22")] // SSH
        [DataRow("telnet://user:pass@hostname:22")] // Telnet
        [DataRow("sftp://user@hostname/path/to/file")] // SFTP
        [DataRow("rtsp://media.example.com/video")] // RTSP
        [DataRow("http://[2001:db8::1]:8080")] // http with ipv6
        [DataRow("ftp://[2001:db8::1]/folder")] // ftp with ipv6
        [DataRow("urn:isbn:0262531968")] // isbn urn
        [DataRow("urn:uuid:123e4567-e89b-12d3-a456-426614174000")] // uuid urn
        [DataRow("urn:ietf:rfc:2648")] // IETF namespace urn
        [DataRow("urn:uci:I001+SBSi-B10000083052")] // UCI urn
        [DataRow("urn:isan:0000-0000-9E59-0000-O-0000-0000-2")] // ISAN urn
        [DataRow("urn:issn:0167-6423")] // ISSN urn
        [DataRow("urn:mpeg:mpeg7:schema:2001")] // mpeg urn
        [DataRow("urn:oid:2.216.840")] // urn oid
        [DataRow("uuid:6e8bc430-9c3a-11d9-9669-0800200c9a66")] // UUID
        [DataRow("geo:47.645812,-122.134544")] // geo location
        [DataRow("news:comp.infosystems.www.servers.unix")] // news
        [DataRow("tel:+1-816-555-1212")] // Telephone number
        [DataRow("h323:caller@192.168.1.100?codec=g729&bandwidth=256")] // h323 with user, ipv4, and query values
        [DataRow("ldap://[2001:db8::7]/c=GB?objectClass?one")] // ldap
        [DataRow("http://www.co1ntoso.com")] // Http
        [DataRow("http://user:password@www.co5ntoso.com:80/Home/Index.htm?q1=v1&q2=v2#FragmentName")] // Http with bells and whistles
        [DataRow("https://www.co1ntoso.com")] // Https
        [DataRow("https://www.co2ntoso.com/Home/Index.htm")] // Https with path
        [DataRow("https://www.co3ntoso.com/Home/Index.htm?q1=v1&q2=v2")] // Https with path and query
        [DataRow("https://www.co4ntoso.com:443/Home/Index.htm?q1=v1&q2=v2#FragmentName")] // Https with path, query, fragment, and standard port
        [DataRow("https://www.co4ntoso.com:80/Home/Index.htm?q1=v1&q2=v2#FragmentName")] // Https with path, query, fragment, and non-standard port
        [DataRow("https://user:password@www.co5ntoso.com:80/Home/Index.htm?q1=v1&q2=v2#FragmentName")] // Https with user credentials and non-standard port
        [DataRow("h1tp://foo.com")] // typos
        [DataRow("http5://foo.com")] // typos
        [DataRow("HtTp://example.com")] // mixed case http
        [DataRow("FTp://example.com")] // mixed case ftp
        [DataRow("http://example.com/path%20with%20spaces")] // unusual but valid characters
        [DataRow("http://example.com/?q=complex%20query%21%40%23")] // including special characters
        [DataRow("file:///C:/path%20with%20spaces/file.txt")] // file path with encoded spaces
        [DataRow("magnet:?xt=urn:btih:1234567890abcdef")]
        [DataRow("http://example.com/path?redirect=192.168.0.1#section")] // ip in query
        [DataRow("http://example.com/path#192.168.0.1")] // ip in fragment
        [DataRow("http://example.com#")] // empty fragment
        [DataRow("http://example.com/../../etc/passwd")] // path traversal characters
        [DataRow("http:// user : pass @ example.com")] // Space in authority  ** should verify this works in standard uri
        [DataRow("blob:http://example.com/550e8400-e29b-41d4-a716-446655440000")] // blob uri
        //[DataRow("urn:example:animal:ferret:nose?color=black")] // urn with query parameters  **urn parsing is not fully implemented
        public void UriCtor_Should_Not_Throw_Exception(string uri)
        {
            Console.WriteLine("uri: " + uri);
            Uri createdUri = new(uri);

            Assert.IsNotNull(createdUri);
        }

        [TestMethod]
        [DataRow("", typeof(ArgumentNullException), "ExpectedArgumentNullException")]
        [DataRow("foo", typeof(ArgumentException), "Expected ArgumentException")]
        [DataRow("file:///c:", typeof(ArgumentException), "Expected ArgumentException")]
        [DataRow("http:abc/d/", typeof(ArgumentException), "Expected ArgumentException")]
        [DataRow("file:/server", typeof(ArgumentException), "Expected ArgumentException")]
        [DataRow("http:", typeof(ArgumentException), "Expected ArgumentException")]
        [DataRow("1ttp://foo.com", typeof(ArgumentException), "Expected ArgumentException")]
        [DataRow("h@tp://foo.com", typeof(ArgumentException), "Expected ArgumentException")]
        public void UriCtor_Invalid_Should_Throw_Exception(string uri, Type exceptionType, string message)
        {
            try
            {
                Console.WriteLine("uri: " + uri);
                Assert.ThrowsException(exceptionType, () =>
                {
                    _ = new Uri(uri);
                }, message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        [TestMethod]
        public void UriCtor_Https_Should_ParseIntoUri()
        {
            Uri uri = new("https://user:password@www.contoso.com:443/Home/Index.htm?q1=v1&q2=v2#FragmentName");
            PrintUriPropertiesToConsole(uri);

            Assert.AreEqual(@"/Home/Index.htm", uri.AbsolutePath, $"actual: {uri.AbsolutePath} expected: /Home/Index.htm");
            Assert.AreEqual(@"https://user:password@www.contoso.com/Home/Index.htm?q1=v1&q2=v2#FragmentName", uri.AbsoluteUri, $"actual: {uri.AbsoluteUri} expected: https://user:password@www.contoso.com/Home/Index.htm?q1=v1&q2=v2#FragmentName");
            Assert.AreEqual(@"#FragmentName", uri.Fragment, $"actual: {uri.Fragment} expected: #FragmentName");
            Assert.AreEqual(@"www.contoso.com", uri.Host, $"actual: {uri.Host} expected: www.contoso.com");
            Assert.AreEqual(UriHostNameType.Dns, uri.HostNameType, $"actual: {uri.HostNameType} expected: {UriHostNameType.Dns}");
            Assert.IsTrue(uri.IsAbsoluteUri, "IsAbsoluteUri was false, but expected true");
            Assert.IsTrue(uri.IsDefaultPort, "IsDefaultPort was false, but expected true");
            Assert.IsFalse(uri.IsFile, "IsFile was true, but expected false");
            Assert.IsFalse(uri.IsLoopback, "IsLoopback was true, but expected false");
            Assert.IsFalse(uri.IsUnc, "IsUnc was true, but expected false");
            Assert.AreEqual(@"https://user:password@www.contoso.com:443/Home/Index.htm?q1=v1&q2=v2#FragmentName", uri.OriginalString, $"actual: {uri.OriginalString} expected: https://user:password@www.contoso.com:443/Home/Index.htm?q1=v1&q2=v2#FragmentName");
            Assert.AreEqual(@"/Home/Index.htm?q1=v1&q2=v2", uri.PathAndQuery, $"actual: {uri.PathAndQuery} expected: /Home/Index.htm?q1=v1&q2=v2");
            Assert.AreEqual(443, uri.Port, $"actual: {uri.Port} expected: {443}");
            Assert.AreEqual(@"?q1=v1&q2=v2", uri.Query, $"actual: {uri.Query} expected: ?q1=v1&q2=v2");
            Assert.AreEqual(@"https", uri.Scheme, $"actual: {uri.Scheme} expected: https");
            Assert.AreEqual(3, uri.Segments.Length, $"actual: {uri.Segments.Length} expected: 3");
            Assert.AreEqual(@"user:password", uri.UserInfo, $"actual: {uri.UserInfo} expected: user:password");
        }

        [TestMethod]
        public void UriCtor_WebSocket_Should_ParseIntoUri()
        {
            Uri uri = new("ws://ws.pusherapp.com:80/app/?client=js&version=1.9.3&protocol=5");
            PrintUriPropertiesToConsole(uri);

            Assert.AreEqual(@"/app/", uri.AbsolutePath, $"actual: {uri.AbsolutePath} expected: /app/");
            Assert.AreEqual(@"ws://ws.pusherapp.com/app/?client=js&version=1.9.3&protocol=5", uri.AbsoluteUri, $"actual: {uri.AbsoluteUri} expected: ws://ws.pusherapp.com/app/?client=js&version=1.9.3&protocol=5");
            Assert.AreEqual(@"", uri.Fragment, $"actual: {uri.Fragment} expected: ");
            Assert.AreEqual(@"ws.pusherapp.com", uri.Host, $"actual: {uri.Host} expected: ws.pusherapp.com");
            Assert.AreEqual(UriHostNameType.Dns, uri.HostNameType, $"actual: {uri.HostNameType} expected: {UriHostNameType.Dns}");
            Assert.IsTrue(uri.IsAbsoluteUri, "IsAbsoluteUri was false, but expected true");
            Assert.IsTrue(uri.IsDefaultPort, "IsDefaultPort was false, but expected true");
            Assert.IsFalse(uri.IsFile, "IsFile was true, but expected false");
            Assert.IsFalse(uri.IsLoopback, "IsLoopback was true, but expected false");
            Assert.IsFalse(uri.IsUnc, "IsUnc was true, but expected false");
            Assert.AreEqual(@"ws://ws.pusherapp.com:80/app/?client=js&version=1.9.3&protocol=5", uri.OriginalString, $"actual: {uri.OriginalString} expected: ws://ws.pusherapp.com:80/app/?client=js&version=1.9.3&protocol=5");
            Assert.AreEqual(@"/app/?client=js&version=1.9.3&protocol=5", uri.PathAndQuery, $"actual: {uri.PathAndQuery} expected: /app/?client=js&version=1.9.3&protocol=5");
            Assert.AreEqual(80, uri.Port, $"actual: {uri.Port} expected: {80}");
            Assert.AreEqual(@"?client=js&version=1.9.3&protocol=5", uri.Query, $"actual: {uri.Query} expected: ?client=js&version=1.9.3&protocol=5");
            Assert.AreEqual(@"ws", uri.Scheme, $"actual: {uri.Scheme} expected: ws");
            Assert.AreEqual(3, uri.Segments.Length, $"actual: {uri.Segments.Length} expected: 3");
            Assert.AreEqual(@"", uri.UserInfo, $"actual: {uri.UserInfo} expected: ");
        }

        [TestMethod]
        public void UriCtor_ftp_Should_ParseIntoUri()
        {
            Uri uri = new("ftp://user:password@ftp.contoso.com:21/directory/file.txt");
            PrintUriPropertiesToConsole(uri);

            Assert.AreEqual(@"/directory/file.txt", uri.AbsolutePath, $"actual: {uri.AbsolutePath} expected: /directory/file.txt");
            Assert.AreEqual(@"ftp://user:password@ftp.contoso.com/directory/file.txt", uri.AbsoluteUri, $"actual: {uri.AbsoluteUri} expected: ftp://user:password@ftp.contoso.com/directory/file.txt");
            Assert.AreEqual(@"", uri.Fragment, $"actual: {uri.Fragment} expected: ''");
            Assert.AreEqual(@"ftp.contoso.com", uri.Host, $"actual: {uri.Host} expected: ftp.contoso.com");
            Assert.AreEqual(UriHostNameType.Dns, uri.HostNameType, $"actual: {uri.HostNameType} expected: {UriHostNameType.Dns}");
            Assert.IsTrue(uri.IsAbsoluteUri, "IsAbsoluteUri was false, but expected true");
            Assert.IsTrue(uri.IsDefaultPort, "IsDefaultPort was false, but expected true");
            Assert.IsFalse(uri.IsFile, "IsFile was true, but expected false");
            Assert.IsFalse(uri.IsLoopback, "IsLoopback was true, but expected false");
            Assert.IsFalse(uri.IsUnc, "IsUnc was true, but expected false");
            Assert.AreEqual(@"ftp://user:password@ftp.contoso.com:21/directory/file.txt", uri.OriginalString, $"actual: {uri.OriginalString} expected: ftp://user:password@ftp.contoso.com:21/directory/file.txt");
            Assert.AreEqual(@"/directory/file.txt", uri.PathAndQuery, $"actual: {uri.PathAndQuery} expected: /directory/file.txt");
            Assert.AreEqual(21, uri.Port, $"actual: {uri.Port} expected: {21}");
            Assert.AreEqual(@"", uri.Query, $"actual: {uri.Query} expected: ''");
            Assert.AreEqual(@"ftp", uri.Scheme, $"actual: {uri.Scheme} expected: ftp");
            Assert.AreEqual(3, uri.Segments.Length, $"actual: {uri.Segments.Length} expected: 3");
            Assert.AreEqual(@"user:password", uri.UserInfo, $"actual: {uri.UserInfo} expected: user:password");
        }

        [TestMethod]
        public void UriCtor_ftp_Malformed_Should_ParseIntoUri()
        {
            Uri uri = new("\tftp://abc.com   ");
            PrintUriPropertiesToConsole(uri);

            Assert.AreEqual(@"/", uri.AbsolutePath, $"actual: {uri.AbsolutePath} expected: /");
            Assert.AreEqual(@"ftp://abc.com/", uri.AbsoluteUri, $"actual: {uri.AbsoluteUri} expected: ftp://abc.com/");
            Assert.AreEqual(@"", uri.Fragment, $"actual: {uri.Fragment} expected: ''");
            Assert.AreEqual(@"abc.com", uri.Host, $"actual: {uri.Host} expected: abc.com");
            Assert.AreEqual(UriHostNameType.Dns, uri.HostNameType, $"actual: {uri.HostNameType} expected: {UriHostNameType.Dns}");
            Assert.IsTrue(uri.IsAbsoluteUri, "IsAbsoluteUri was false, but expected true");
            Assert.IsTrue(uri.IsDefaultPort, "IsDefaultPort was false, but expected true");
            Assert.IsFalse(uri.IsFile, "IsFile was true, but expected false");
            Assert.IsFalse(uri.IsLoopback, "IsLoopback was true, but expected false");
            Assert.IsFalse(uri.IsUnc, "IsUnc was true, but expected false");
            Assert.AreEqual(@"	ftp://abc.com   ", uri.OriginalString, $"actual: {uri.OriginalString} expected: \tftp://abc.com   ");
            Assert.AreEqual(@"/", uri.PathAndQuery, $"actual: {uri.PathAndQuery} expected: /");
            Assert.AreEqual(21, uri.Port, $"actual: {uri.Port} expected: {21}");
            Assert.AreEqual(@"", uri.Query, $"actual: {uri.Query} expected: ''");
            Assert.AreEqual(@"ftp", uri.Scheme, $"actual: {uri.Scheme} expected: ftp");
            Assert.AreEqual(2, uri.Segments.Length, $"actual: {uri.Segments.Length} expected: 2");
            Assert.AreEqual(@"", uri.UserInfo, $"actual: {uri.UserInfo} expected: ");
        }

        [TestMethod]
        public void UriCtor_CombineCtor_Should_ParseIntoUri()
        {
            Uri baseUri = new(@"http://www.contoso.com/");
            Uri uri = new(baseUri, "catalog/shownew.htm?date=today");
            PrintUriPropertiesToConsole(uri);

            Assert.AreEqual(@"/catalog/shownew.htm", uri.AbsolutePath, $"actual: {uri.AbsolutePath} expected: /catalog/shownew.htm");
            Assert.AreEqual(@"http://www.contoso.com/catalog/shownew.htm?date=today", uri.AbsoluteUri, $"actual: {uri.AbsoluteUri} expected: http://www.contoso.com/catalog/shownew.htm?date=today");
            Assert.AreEqual(@"", uri.Fragment, $"actual: {uri.Fragment} expected: ''");
            Assert.AreEqual(@"www.contoso.com", uri.Host, $"actual: {uri.Host} expected: www.contoso.com");
            Assert.AreEqual(UriHostNameType.Dns, uri.HostNameType, $"actual: {uri.HostNameType} expected: {UriHostNameType.Dns}");
            Assert.IsTrue(uri.IsAbsoluteUri, "IsAbsoluteUri was false, but expected true");
            Assert.IsTrue(uri.IsDefaultPort, "IsDefaultPort was false, but expected true");
            Assert.IsFalse(uri.IsFile, "IsFile was true, but expected false");
            Assert.IsFalse(uri.IsLoopback, "IsLoopback was true, but expected false");
            Assert.IsFalse(uri.IsUnc, "IsUnc was true, but expected false");
            Assert.AreEqual(@"http://www.contoso.com/catalog/shownew.htm?date=today", uri.OriginalString, $"actual: {uri.OriginalString} expected: http://www.contoso.com/catalog/shownew.htm?date=today");
            Assert.AreEqual(@"/catalog/shownew.htm?date=today", uri.PathAndQuery, $"actual: {uri.PathAndQuery} expected: /catalog/shownew.htm?date=today");
            Assert.AreEqual(80, uri.Port, $"actual: {uri.Port} expected: {80}");
            Assert.AreEqual(@"?date=today", uri.Query, $"actual: {uri.Query} expected: ?date=today");
            Assert.AreEqual(@"http", uri.Scheme, $"actual: {uri.Scheme} expected: http");
            Assert.AreEqual(3, uri.Segments.Length, $"actual: {uri.Segments.Length} expected: 3");
            Assert.AreEqual(@"", uri.UserInfo, $"actual: {uri.UserInfo} expected: ''");
        }

        [TestMethod]
        public void UriCtor_CombineCtor_Absolute_Should_ParseIntoUri()
        {
            Uri baseUri = new(@"http://www.contoso.com/", UriKind.Absolute);
            var uri = new Uri(baseUri, "catalog/shownew.htm?date=today");
            PrintUriPropertiesToConsole(uri);

            Assert.AreEqual(@"/catalog/shownew.htm", uri.AbsolutePath, $"actual: {uri.AbsolutePath} expected: /catalog/shownew.htm");
            Assert.AreEqual(@"http://www.contoso.com/catalog/shownew.htm?date=today", uri.AbsoluteUri, $"actual: {uri.AbsoluteUri} expected: http://www.contoso.com/catalog/shownew.htm?date=today");
            Assert.AreEqual(@"", uri.Fragment, $"actual: {uri.Fragment} expected: ''");
            Assert.AreEqual(@"www.contoso.com", uri.Host, $"actual: {uri.Host} expected: www.contoso.com");
            Assert.AreEqual(UriHostNameType.Dns, uri.HostNameType, $"actual: {uri.HostNameType} expected: {UriHostNameType.Dns}");
            Assert.IsTrue(uri.IsAbsoluteUri, "IsAbsoluteUri was false, but expected true");
            Assert.IsTrue(uri.IsDefaultPort, "IsDefaultPort was false, but expected true");
            Assert.IsFalse(uri.IsFile, "IsFile was true, but expected false");
            Assert.IsFalse(uri.IsLoopback, "IsLoopback was true, but expected false");
            Assert.IsFalse(uri.IsUnc, "IsUnc was true, but expected false");
            Assert.AreEqual(@"http://www.contoso.com/catalog/shownew.htm?date=today", uri.OriginalString, $"actual: {uri.OriginalString} expected: http://www.contoso.com/catalog/shownew.htm?date=today");
            Assert.AreEqual(@"/catalog/shownew.htm?date=today", uri.PathAndQuery, $"actual: {uri.PathAndQuery} expected: /catalog/shownew.htm?date=today");
            Assert.AreEqual(80, uri.Port, $"actual: {uri.Port} expected: {80}");
            Assert.AreEqual(@"?date=today", uri.Query, $"actual: {uri.Query} expected: ?date=today");
            Assert.AreEqual(@"http", uri.Scheme, $"actual: {uri.Scheme} expected: http");
            Assert.AreEqual(3, uri.Segments.Length, $"actual: {uri.Segments.Length} expected: 3");
            Assert.AreEqual(@"", uri.UserInfo, $"actual: {uri.UserInfo} expected: ''");
        }

        [TestMethod]
        public void UriCtor_Ctor_RelativeUri_Should_ParseIntoUri()
        {
            Uri uri = new("catalog/shownew.htm?date=today", UriKind.Relative);

            Console.WriteLine($"Fragment: {uri.Fragment}");
            Console.WriteLine($"HostNameType: {uri.HostNameType}");
            Console.WriteLine($"IsAbsoluteUri: {uri.IsAbsoluteUri}");
            Console.WriteLine($"IsDefaultPort: {uri.IsDefaultPort}");
            Console.WriteLine($"IsFile: {uri.IsFile}");
            Console.WriteLine($"OriginalString: {uri.OriginalString}");
            Console.WriteLine($"Query: {uri.Query}");
            Console.WriteLine($"UserInfo: {uri.UserInfo}");

            Assert.ThrowsException(typeof(InvalidOperationException), () => { _ = uri.AbsolutePath; }, "No exception thrown when accessing AbsolutePath");
            Assert.ThrowsException(typeof(InvalidOperationException), () => { _ = uri.AbsoluteUri; }, "No exception thrown when accessing AbsoluteUri");
            Assert.AreEqual(@"", uri.Fragment, $"actual: {uri.Fragment} expected: ''");
            Assert.ThrowsException(typeof(InvalidOperationException), () => { _ = uri.Host; }, "No exception thrown when accessing Host");
            Assert.AreEqual(UriHostNameType.Unknown, uri.HostNameType, $"actual: {uri.HostNameType} expected: {UriHostNameType.Unknown}");
            Assert.IsFalse(uri.IsAbsoluteUri, "IsAbsoluteUri was true, but expected false");
            Assert.IsTrue(uri.IsDefaultPort, "IsDefaultPort was true, but expected false");
            Assert.IsFalse(uri.IsFile, "IsFile was true, but expected false");
            Assert.ThrowsException(typeof(InvalidOperationException), () => { _ = uri.IsLoopback; }, "No exception thrown when accessing IsLoopback");
            Assert.ThrowsException(typeof(InvalidOperationException), () => { _ = uri.IsUnc; }, "No exception thrown when accessing IsUnc");
            Assert.AreEqual(@"catalog/shownew.htm?date=today", uri.OriginalString);
            Assert.ThrowsException(typeof(InvalidOperationException), () => { _ = uri.PathAndQuery; }, "No exception thrown when PathAndQuery");
            Assert.ThrowsException(typeof(InvalidOperationException), () => { _ = uri.Port; }, "No exception thrown when accessing Port");
            Assert.AreEqual(@"?date=today", uri.Query, $"actual: {uri.Query} expected: ?date=today");
            Assert.ThrowsException(typeof(InvalidOperationException), () => { _ = uri.Scheme; }, "No exception thrown when accessing Scheme");
            Assert.ThrowsException(typeof(InvalidOperationException), () => { _ = uri.Segments.Length; }, "No exception thrown when accessing Scheme");
            Assert.AreEqual(@"", uri.UserInfo, $"actual: {uri.UserInfo} expected: ''");
        }

        [TestMethod]
        [DataRow("file://", "/", "file:///", 2)]
        [DataRow("file:///", "/", "file:///", 2)]
        [DataRow("file:////", "//", "file:////", 3)]
        [DataRow("file:///c", "/c", "file:///c", 2)]
        public void UriCtor_Ctor_FileUri_Should_ParseIntoUri(string originalString, string expectedAbsolutePath, string expectedAbsoluteUri, int expectedSegmentCount)
        {
            Uri uri = new(originalString);
            PrintUriPropertiesToConsole(uri);

            Assert.AreEqual(expectedAbsolutePath, uri.AbsolutePath, $"actual: {uri.AbsolutePath} expected: {expectedAbsolutePath}");
            Assert.AreEqual(expectedAbsoluteUri, uri.AbsoluteUri, $"actual: {uri.AbsoluteUri} expected: {expectedAbsoluteUri}");
            Assert.AreEqual(@"", uri.Fragment, $"actual: {uri.Fragment} expected: ''");
            Assert.AreEqual(@"", uri.Host, $"actual: {uri.Host} expected: ''");
            Assert.AreEqual(UriHostNameType.Basic, uri.HostNameType, $"actual: {uri.HostNameType} expected: {UriHostNameType.Basic}");
            Assert.IsTrue(uri.IsAbsoluteUri, "IsAbsoluteUri was false, but expected true");
            Assert.IsTrue(uri.IsDefaultPort, "IsDefaultPort was false, but expected true");
            Assert.IsTrue(uri.IsFile, "IsFile was true, but expected false");
            Assert.IsTrue(uri.IsLoopback, "IsLoopback was true, but expected false");
            Assert.IsFalse(uri.IsUnc, "IsUnc was true, but expected false");
            Assert.AreEqual(originalString, uri.OriginalString, $"actual: {uri.OriginalString} expected: {originalString}");
            Assert.AreEqual(expectedAbsolutePath, uri.PathAndQuery, $"actual: {uri.PathAndQuery} expected: expectedAbsolutePath");
            Assert.AreEqual(-1, uri.Port, $"actual: {uri.Port} expected: {-1}");
            Assert.AreEqual(@"", uri.Query, $"actual: {uri.Query} expected: ''");
            Assert.AreEqual(@"file", uri.Scheme, $"actual: {uri.Scheme} expected: file");
            Assert.AreEqual(expectedSegmentCount, uri.Segments.Length, $"actual: {uri.Segments.Length} expected: {expectedSegmentCount}");
            Assert.AreEqual(@"", uri.UserInfo, $"actual: {uri.UserInfo} expected: ''");
        }

        [TestMethod]
        [DataRow("file://c")]
        [DataRow("file:////c")]
        public void UriCtor_Ctor_FileUri_Dns_Should_ParseIntoUri(string originalString)
        {
            Uri uri = new(originalString);
            PrintUriPropertiesToConsole(uri);

            Assert.AreEqual(@"/", uri.AbsolutePath, $"actual: {uri.AbsolutePath} expected: /");
            Assert.AreEqual(@"file://c/", uri.AbsoluteUri, $"actual: {uri.AbsoluteUri} expected: file://c/");
            Assert.AreEqual(@"", uri.Fragment, $"actual: {uri.Fragment} expected: ''");
            Assert.AreEqual(@"c", uri.Host, $"actual: {uri.Host} expected: 'c'");
            Assert.AreEqual(UriHostNameType.Dns, uri.HostNameType, $"actual: {uri.HostNameType} expected: {UriHostNameType.Dns}");
            Assert.IsTrue(uri.IsAbsoluteUri, "IsAbsoluteUri was false, but expected true");
            Assert.IsTrue(uri.IsDefaultPort, "IsDefaultPort was false, but expected true");
            Assert.IsTrue(uri.IsFile, "IsFile was true, but expected false");
            Assert.IsFalse(uri.IsLoopback, "IsLoopback was true, but expected false");
            Assert.IsTrue(uri.IsUnc, "IsUnc was true, but expected false");
            Assert.AreEqual(originalString, uri.OriginalString, $"actual: {uri.OriginalString} expected: {originalString}");
            Assert.AreEqual(@"/", uri.PathAndQuery, $"actual: {uri.PathAndQuery} expected: /");
            Assert.AreEqual(-1, uri.Port, $"actual: {uri.Port} expected: {-1}");
            Assert.AreEqual(@"", uri.Query, $"actual: {uri.Query} expected: ''");
            Assert.AreEqual(@"file", uri.Scheme, $"actual: {uri.Scheme} expected: file");
            Assert.AreEqual(2, uri.Segments.Length, $"actual: {uri.Segments.Length} expected: 2");
            Assert.AreEqual(@"", uri.UserInfo, $"actual: {uri.UserInfo} expected: ''");
        }

        [TestMethod]
        public void NullCtor_Should_Throw_Exception()
        {
            Assert.ThrowsException(typeof(ArgumentNullException),
                () => _ = new Uri(null), "Expected ArgumentNullException");
        }

        [TestMethod]
        public void UriCtor_ldap_Should_ParseIntoUri()
        {
            Uri uri = new("ldap://[2001:db8::7]/c=GB?objectClass?one");
            PrintUriPropertiesToConsole(uri);

            Assert.AreEqual(@"/c=GB", uri.AbsolutePath, $"actual: {uri.AbsolutePath} expected: /c=GB");
            Assert.AreEqual(@"ldap://[2001:db8::7]/c=GB?objectClass?one", uri.AbsoluteUri, $"actual: {uri.AbsoluteUri} expected: ldap://[2001:db8::7]/c=GB?objectClass?one");
            Assert.AreEqual(@"", uri.Fragment, $"actual: {uri.Fragment} expected: ");
            Assert.AreEqual(@"[2001:db8::7]", uri.Host, $"actual: {uri.Host} expected: [2001:db8::7]");
            Assert.AreEqual(UriHostNameType.IPv6, uri.HostNameType, $"actual: {uri.HostNameType} expected: {UriHostNameType.IPv6}");
            Assert.IsTrue(uri.IsAbsoluteUri, "IsAbsoluteUri was false, but expected true");
            Assert.IsTrue(uri.IsDefaultPort, "IsDefaultPort was false, but expected true");
            Assert.IsFalse(uri.IsFile, "IsFile was true, but expected false");
            Assert.IsFalse(uri.IsLoopback, "IsLoopback was true, but expected false");
            Assert.IsFalse(uri.IsUnc, "IsUnc was true, but expected false");
            Assert.AreEqual(@"ldap://[2001:db8::7]/c=GB?objectClass?one", uri.OriginalString, $"actual: {uri.OriginalString} expected: ldap://[2001:db8::7]/c=GB?objectClass?one");
            Assert.AreEqual(@"/c=GB?objectClass?one", uri.PathAndQuery, $"actual: {uri.PathAndQuery} expected: /c=GB?objectClass?one");
            Assert.AreEqual(389, uri.Port, $"actual: {uri.Port} expected: {389}");
            Assert.AreEqual(@"?objectClass?one", uri.Query, $"actual: {uri.Query} expected: ?objectClass?one");
            Assert.AreEqual(@"ldap", uri.Scheme, $"actual: {uri.Scheme} expected: ldap");
            Assert.AreEqual(2, uri.Segments.Length, $"actual: {uri.Segments.Length} expected: 2");
            Assert.AreEqual(@"", uri.UserInfo, $"actual: {uri.UserInfo} expected: ''");
        }

        [TestMethod]
        public void UriCtor_mailto_Should_ParseIntoUri()
        {
            Uri uri = new("mailto:John.Doe@example.com");
            PrintUriPropertiesToConsole(uri);

            Assert.AreEqual(@"", uri.AbsolutePath, $"actual: {uri.AbsolutePath} expected: ''");
            Assert.AreEqual(@"mailto:John.Doe@example.com", uri.AbsoluteUri, $"actual: {uri.AbsoluteUri} expected: mailto:John.Doe@example.com");
            Assert.AreEqual(@"", uri.Fragment, $"actual: {uri.Fragment} expected: ");
            Assert.AreEqual(@"example.com", uri.Host, $"actual: {uri.Host} expected: example.com");
            Assert.AreEqual(UriHostNameType.Dns, uri.HostNameType, $"actual: {uri.HostNameType} expected: {UriHostNameType.Dns}");
            Assert.IsTrue(uri.IsAbsoluteUri, "IsAbsoluteUri was false, but expected true");
            Assert.IsTrue(uri.IsDefaultPort, "IsDefaultPort was false, but expected true");
            Assert.IsFalse(uri.IsFile, "IsFile was true, but expected false");
            Assert.IsFalse(uri.IsLoopback, "IsLoopback was true, but expected false");
            Assert.IsFalse(uri.IsUnc, "IsUnc was true, but expected false");
            Assert.AreEqual(@"mailto:John.Doe@example.com", uri.OriginalString, $"actual: {uri.OriginalString} expected: mailto:John.Doe@example.com");
            Assert.AreEqual(@"", uri.PathAndQuery, $"actual: {uri.PathAndQuery} expected: ''");
            Assert.AreEqual(25, uri.Port, $"actual: {uri.Port} expected: {25}");
            Assert.AreEqual(@"", uri.Query, $"actual: {uri.Query} expected: ");
            Assert.AreEqual(@"mailto", uri.Scheme, $"actual: {uri.Scheme} expected: mailto");
            Assert.AreEqual(1, uri.Segments.Length, $"actual: {uri.Segments.Length} expected: 1");
            Assert.AreEqual(@"John.Doe", uri.UserInfo, $"actual: {uri.UserInfo} expected: John.Doe");
        }

        [TestMethod]
        public void UriCtor_news_basic_Should_ParseIntoUri()
        {
            Uri uri = new("news:comp.infosystems.www.servers.unix");
            PrintUriPropertiesToConsole(uri);

            Assert.AreEqual(@"comp.infosystems.www.servers.unix", uri.AbsolutePath, $"actual: {uri.AbsolutePath} expected: comp.infosystems.www.servers.unix");
            Assert.AreEqual(@"news:comp.infosystems.www.servers.unix", uri.AbsoluteUri, $"actual: {uri.AbsoluteUri} expected: news:comp.infosystems.www.servers.unix");
            Assert.AreEqual(@"", uri.Fragment, $"actual: {uri.Fragment} expected: ");
            Assert.AreEqual(@"", uri.Host, $"actual: {uri.Host} expected: ");
            Assert.AreEqual(UriHostNameType.Unknown, uri.HostNameType, $"actual: {uri.HostNameType} expected: {UriHostNameType.Unknown}");
            Assert.IsTrue(uri.IsAbsoluteUri, "IsAbsoluteUri was false, but expected true");
            Assert.IsTrue(uri.IsDefaultPort, "IsDefaultPort was false, but expected true");
            Assert.IsFalse(uri.IsFile, "IsFile was true, but expected false");
            Assert.IsFalse(uri.IsLoopback, "IsLoopback was true, but expected false");
            Assert.IsFalse(uri.IsUnc, "IsUnc was true, but expected false");
            Assert.AreEqual(@"news:comp.infosystems.www.servers.unix", uri.OriginalString, $"actual: {uri.OriginalString} expected: news:comp.infosystems.www.servers.unix");
            Assert.AreEqual(@"comp.infosystems.www.servers.unix", uri.PathAndQuery, $"actual: {uri.PathAndQuery} expected: comp.infosystems.www.servers.unix");
            Assert.AreEqual(-1, uri.Port, $"actual: {uri.Port} expected: {-1}");
            Assert.AreEqual(@"", uri.Query, $"actual: {uri.Query} expected: ");
            Assert.AreEqual(@"news", uri.Scheme, $"actual: {uri.Scheme} expected: news");
            Assert.AreEqual(1, uri.Segments.Length, $"actual: {uri.Segments.Length} expected: 1");
            Assert.AreEqual(@"", uri.UserInfo, $"actual: {uri.UserInfo} expected: ''");
        }

        [TestMethod]
        public void UriCtor_Telephone_Number_Should_ParseIntoUri()
        {
            Uri uri = new("tel:+1-816-555-1212");
            PrintUriPropertiesToConsole(uri);

            Assert.AreEqual(@"+1-816-555-1212", uri.AbsolutePath, $"actual: {uri.AbsolutePath} expected: +1-816-555-1212");
            Assert.AreEqual(@"tel:+1-816-555-1212", uri.AbsoluteUri, $"actual: {uri.AbsoluteUri} expected: tel:+1-816-555-1212");
            Assert.AreEqual(@"", uri.Fragment, $"actual: {uri.Fragment} expected: ");
            Assert.AreEqual(@"", uri.Host, $"actual: {uri.Host} expected: ");
            Assert.AreEqual(UriHostNameType.Unknown, uri.HostNameType, $"actual: {uri.HostNameType} expected: {UriHostNameType.Unknown}");
            Assert.IsTrue(uri.IsAbsoluteUri, "IsAbsoluteUri was false, but expected true");
            Assert.IsTrue(uri.IsDefaultPort, "IsDefaultPort was false, but expected true");
            Assert.IsFalse(uri.IsFile, "IsFile was true, but expected false");
            Assert.IsFalse(uri.IsLoopback, "IsLoopback was true, but expected false");
            Assert.IsFalse(uri.IsUnc, "IsUnc was true, but expected false");
            Assert.AreEqual(@"tel:+1-816-555-1212", uri.OriginalString, $"actual: {uri.OriginalString} expected: tel:+1-816-555-1212");
            Assert.AreEqual(@"+1-816-555-1212", uri.PathAndQuery, $"actual: {uri.PathAndQuery} expected: +1-816-555-1212");
            Assert.AreEqual(-1, uri.Port, $"actual: {uri.Port} expected: {-1}");
            Assert.AreEqual(@"", uri.Query, $"actual: {uri.Query} expected: ");
            Assert.AreEqual(@"tel", uri.Scheme, $"actual: {uri.Scheme} expected: tel");
            Assert.AreEqual(1, uri.Segments.Length, $"actual: {uri.Segments.Length} expected: 1");
            Assert.AreEqual(@"", uri.UserInfo, $"actual: {uri.UserInfo} expected: ''");
        }

        [TestMethod]
        public void UriCtor_Telnet_Should_ParseIntoUri()
        {
            Uri uri = new("telnet://192.0.2.16:80/");
            PrintUriPropertiesToConsole(uri);

            Assert.AreEqual(@"/", uri.AbsolutePath, $"actual: {uri.AbsolutePath} expected: /");
            Assert.AreEqual(@"telnet://192.0.2.16:80/", uri.AbsoluteUri, $"actual: {uri.AbsoluteUri} expected: telnet://192.0.2.16:80/");
            Assert.AreEqual(@"", uri.Fragment, $"actual: {uri.Fragment} expected: ");
            Assert.AreEqual(@"192.0.2.16", uri.Host, $"actual: {uri.Host} expected: 192.0.2.16");
            Assert.AreEqual(UriHostNameType.IPv4, uri.HostNameType, $"actual: {uri.HostNameType} expected: {UriHostNameType.IPv4}");
            Assert.IsTrue(uri.IsAbsoluteUri, "IsAbsoluteUri was false, but expected true");
            Assert.IsFalse(uri.IsDefaultPort, "IsDefaultPort was false, but expected true");
            Assert.IsFalse(uri.IsFile, "IsFile was true, but expected false");
            Assert.IsFalse(uri.IsLoopback, "IsLoopback was true, but expected false");
            Assert.IsFalse(uri.IsUnc, "IsUnc was true, but expected false");
            Assert.AreEqual(@"telnet://192.0.2.16:80/", uri.OriginalString, $"actual: {uri.OriginalString} expected: telnet://192.0.2.16:80/");
            Assert.AreEqual(@"/", uri.PathAndQuery, $"actual: {uri.PathAndQuery} expected: /");
            Assert.AreEqual(80, uri.Port, $"actual: {uri.Port} expected: {80}");
            Assert.AreEqual(@"", uri.Query, $"actual: {uri.Query} expected: ");
            Assert.AreEqual(@"telnet", uri.Scheme, $"actual: {uri.Scheme} expected: telnet");
            Assert.AreEqual(2, uri.Segments.Length, $"actual: {uri.Segments.Length} expected: 2");
            Assert.AreEqual(@"", uri.UserInfo, $"actual: {uri.UserInfo} expected: ''");
        }

        [TestMethod]
        public void UriCtor_h323_Should_ParseIntoUri()
        {
            Uri uri = new("h323:caller@192.168.1.100?codec=g729&bandwidth=256");
            PrintUriPropertiesToConsole(uri);

            Assert.AreEqual(@"caller@192.168.1.100", uri.AbsolutePath, $"actual: {uri.AbsolutePath} expected: caller@192.168.1.100");
            Assert.AreEqual(@"h323:caller@192.168.1.100?codec=g729&bandwidth=256", uri.AbsoluteUri, $"actual: {uri.AbsoluteUri} expected: h323:caller@192.168.1.100?codec=g729&bandwidth=256");
            Assert.AreEqual(@"", uri.Fragment, $"actual: {uri.Fragment} expected: ");
            Assert.AreEqual(@"", uri.Host, $"actual: {uri.Host} expected: ");
            Assert.AreEqual(UriHostNameType.Unknown, uri.HostNameType, $"actual: {uri.HostNameType} expected: {UriHostNameType.Unknown}");
            Assert.IsTrue(uri.IsAbsoluteUri, "IsAbsoluteUri was false, but expected true");
            Assert.IsTrue(uri.IsDefaultPort, "IsDefaultPort was false, but expected true");
            Assert.IsFalse(uri.IsFile, "IsFile was true, but expected false");
            Assert.IsFalse(uri.IsLoopback, "IsLoopback was true, but expected false");
            Assert.IsFalse(uri.IsUnc, "IsUnc was true, but expected false");
            Assert.AreEqual(@"h323:caller@192.168.1.100?codec=g729&bandwidth=256", uri.OriginalString, $"actual: {uri.OriginalString} expected: h323:caller@192.168.1.100?codec=g729&bandwidth=256");
            Assert.AreEqual(@"caller@192.168.1.100?codec=g729&bandwidth=256", uri.PathAndQuery, $"actual: {uri.PathAndQuery} expected: caller@192.168.1.100?codec=g729&bandwidth=256");
            Assert.AreEqual(-1, uri.Port, $"actual: {uri.Port} expected: {-1}");
            Assert.AreEqual(@"?codec=g729&bandwidth=256", uri.Query, $"actual: {uri.Query} expected: ?codec=g729&bandwidth=256");
            Assert.AreEqual(@"h323", uri.Scheme, $"actual: {uri.Scheme} expected: h323");
            Assert.AreEqual(1, uri.Segments.Length, $"actual: {uri.Segments.Length} expected: 1");
            Assert.AreEqual(@"", uri.UserInfo, $"actual: {uri.UserInfo} expected: ''");
        }

        private static void PrintUriPropertiesToConsole(Uri uri)
        {
            Console.WriteLine($"AbsolutePath: {uri.AbsolutePath}");
            Console.WriteLine($"AbsoluteUri: {uri.AbsoluteUri}");
            Console.WriteLine($"Fragment: {uri.Fragment}");
            Console.WriteLine($"Host: {uri.Host}");
            Console.WriteLine($"HostNameType: {uri.HostNameType}");
            Console.WriteLine($"IsAbsoluteUri: {uri.IsAbsoluteUri}");
            Console.WriteLine($"IsDefaultPort: {uri.IsDefaultPort}");
            Console.WriteLine($"IsFile: {uri.IsFile}");
            Console.WriteLine($"IsLoopback: {uri.IsLoopback}");
            Console.WriteLine($"IsUnc: {uri.IsUnc}");
            Console.WriteLine($"OriginalString: {uri.OriginalString}");
            Console.WriteLine($"PathAndQuery: {uri.PathAndQuery}");
            Console.WriteLine($"Port: {uri.Port}");
            Console.WriteLine($"Query: {uri.Query}");
            Console.WriteLine($"Scheme: {uri.Scheme}");
            Console.WriteLine($"Segments: {uri.Segments.Length}");
            Console.WriteLine($"UserInfo: {uri.UserInfo}");
        }
    }
}
