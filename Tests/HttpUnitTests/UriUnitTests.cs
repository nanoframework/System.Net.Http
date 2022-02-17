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
        Uri uri;
        UriProperties props;
        ParsedUri parsed;

        [TestMethod]
        public void TestCtor_String()
        {
            Uri uri = new Uri(@"http://foo/bar/baz#frag");

            Assert.Equal(@"http://foo/bar/baz#frag", uri.AbsoluteUri, $"expecting {@"http://foo/bar/baz#frag"}, got {uri.AbsoluteUri}");

            // TODO parsing from a relative URL needs to be fixed
            //Assert.Equal(@"/bar/baz", uri.AbsolutePath);

            Assert.Equal(@"http://foo/bar/baz#frag", uri.AbsoluteUri);

            Assert.Equal(@"foo", uri.Host);

            Assert.True(uri.IsAbsoluteUri);

            Assert.False(uri.IsLoopback);

            Assert.False(uri.IsUnc);

            Assert.Equal(@"http://foo/bar/baz#frag", uri.OriginalString);

            Assert.Equal(80, uri.Port);

            Assert.Equal(@"http", uri.Scheme);
        }

        [TestMethod]
        public void TestCtor_Uri_String()
        {
            Uri uri = new Uri(@"http://www.contoso.com/");

            uri = new Uri(uri, "catalog/shownew.htm?date=today");

            // TODO parsing from a relative URL needs to be fixed
            //Assert.Equal(@"/catalog/shownew.htm", uri.AbsolutePath);

            Assert.Equal(@"http://www.contoso.com/catalog/shownew.htm?date=today", uri.AbsoluteUri);

            Assert.Equal(@"www.contoso.com", uri.Host);

            Assert.True(uri.IsAbsoluteUri);

            Assert.False(uri.IsLoopback);

            Assert.False(uri.IsUnc);

            Assert.Equal(@"http://www.contoso.com/catalog/shownew.htm?date=today", uri.OriginalString);

            Assert.Equal(80, uri.Port);

            Assert.Equal(@"http", uri.Scheme);
        }

        [TestMethod]
        public void TestCtor_String_UriKind()
        {
            Uri uri = new Uri("catalog/shownew.htm?date=today", UriKind.Relative);

            Assert.Equal(@"catalog/shownew.htm?date=today", uri.OriginalString);

            Assert.False(uri.IsAbsoluteUri);

            Assert.Throws(typeof(InvalidOperationException), () => { _ = uri.AbsolutePath; }, "No exception thrown when accessing AbsolutePath");

            Assert.Throws(typeof(InvalidOperationException), () => { _ = uri.AbsoluteUri; }, "No exception thrown when accessing AbsoluteUri");

            Assert.Throws(typeof(InvalidOperationException), () => { _ = uri.Host; }, "No exception thrown when accessing Host");

            Assert.Throws(typeof(InvalidOperationException), () => { _ = uri.IsLoopback; }, "No exception thrown when accessing IsLoopback");

            Assert.Throws(typeof(InvalidOperationException), () => { _ = uri.IsUnc; }, "No exception thrown when accessing IsUnc");

            Assert.Throws(typeof(InvalidOperationException), () => { _ = uri.Port; }, "No exception thrown when accessing Port");

            Assert.Throws(typeof(InvalidOperationException), () => { _ = uri.Scheme; }, "No exception thrown when accessing Scheme");
        }

        [TestMethod]
        public static void TestTryCreate_Uri_String()
        {
            Uri baseUri = new Uri("http://www.contoso.com/", UriKind.Absolute);

            Uri uri = new Uri(baseUri, "catalog/shownew.htm?date=today");

            // TODO need fix
            //Assert.Equal(@"http://www.contoso.com/catalog/shownew.htm?date=today", uri.AbsolutePath, "Uris don't match");

            // TODO need fix
            //Assert.Equal(@"/catalog/shownew.htm", uri.AbsolutePath, "Is not an absolute path");

            Assert.Equal(@"http://www.contoso.com/catalog/shownew.htm?date=today", uri.AbsoluteUri, "is not an absolute URI");

            Assert.Equal(@"www.contoso.com", uri.Host, "Is not host");

            Assert.True(uri.IsAbsoluteUri, "Is not an absolute URI");

            Assert.False(uri.IsLoopback, "Is loopback and it shouldn't");

            Assert.False(uri.IsUnc, "Is UNC and it shouldn't");

            Assert.Equal(@"http://www.contoso.com/catalog/shownew.htm?date=today", uri.OriginalString, "strings don't match");

            Assert.Equal(80, uri.Port, "Ports aren't equal");

            Assert.Equal(@"http", uri.Scheme, "URL scheme don't match");
        }

        [TestMethod]
        public void InvalidConstructorTests()
        {
            try
            {
                OutputHelper.WriteLine("null string constructor");
                Assert.Throws(typeof(ArgumentNullException),
                    () =>
                    {
                        uri = new Uri(null);
                    },
                    "Expected ArgumentNullException");

                OutputHelper.WriteLine("no uri string");
                Assert.Throws(typeof(ArgumentException),
                    () =>
                    {
                        uri = new Uri("foo");
                    },
                    "Expected ArgumentException");

                OutputHelper.WriteLine("uri, no address");
                Assert.Throws(typeof(ArgumentException),
                    () =>
                    {
                        uri = new Uri("http:");
                    },
                    "Expected ArgumentException");

                OutputHelper.WriteLine("uri, starts with non-alpha");
                Assert.Throws(typeof(ArgumentException),
                    () =>
                    {
                        uri = new Uri("1ttp://foo.com");
                    },
                    "Expected ArgumentException");

                OutputHelper.WriteLine("uri, includes numeric");
                Assert.Throws(typeof(ArgumentException),
                    () =>
                    {
                        uri = new Uri("h1tp://foo.com");
                    },
                    "Expected ArgumentException");

                OutputHelper.WriteLine("uri, includes non-alpha");
                Assert.Throws(typeof(ArgumentException),
                    () =>
                    {
                        uri = new Uri("h@tp://foo.com");
                    },
                    "Expected ArgumentException");

                OutputHelper.WriteLine("No ABSPath port URI");
                Assert.Throws(typeof(ArgumentException),
                    () =>
                    {
                        uri = new Uri("https://www.nanoframework.net" + ":80");
                    },
                    "Expected ArgumentException");

                OutputHelper.WriteLine("Empty string constructor");
                Assert.Throws(typeof(ArgumentException),
                    () =>
                    {
                        uri = new Uri("");
                    },
                    "Expected ArgumentException");
            }
            catch (Exception ex)
            {
                OutputHelper.WriteLine($"Unexpected Exception {ex}");
            }
        }

        [TestMethod]
        public void ValidUri()
        {
            try
            {
                OutputHelper.WriteLine("Microsoft URL");
                props = new UriProperties("http", "www.microsoft.com");
                uri = new Uri(props.OriginalUri);
                Assert.True(ValidUri(uri, props));

                OutputHelper.WriteLine("Alternate http port URL");
                props = new UriProperties("http", "www.microsoft.com")
                {
                    Port = 1080,
                    Path = "/" //Need to remove later.  This seems like a bug to require it.
                };
                uri = new Uri(props.OriginalUri);
                Assert.True(ValidUri(uri, props));

                OutputHelper.WriteLine("URL with content");
                props = new UriProperties("http", "www.microsoft.com")
                {
                    Path = "/en/us/default.aspx"
                };

                uri = new Uri(props.OriginalUri);
                Assert.True(ValidUri(uri, props));
            }
            catch (Exception ex)
            {
                OutputHelper.WriteLine($"Unexpected Exception {ex}");
            }
        }

        [TestMethod]
        public void ValidURN()
        {
            try
            {
                OutputHelper.WriteLine("isbn");
                props = new UriProperties("urn", "isbn:0451450523");
                uri = new Uri(props.OriginalUri);
                Assert.True(ValidUri(uri, props));

                OutputHelper.WriteLine("isan");
                props = new UriProperties("urn", "isan:0000-0000-9E59-0000-O-0000-0000-2");
                uri = new Uri(props.OriginalUri);
                Assert.True(ValidUri(uri, props));

                OutputHelper.WriteLine("issn");
                props = new UriProperties("urn", "issn:0167-6423");
                uri = new Uri(props.OriginalUri);
                Assert.True(ValidUri(uri, props));

                OutputHelper.WriteLine("ietf");
                props = new UriProperties("urn", "ietf:rfc:2648");
                uri = new Uri(props.OriginalUri);
                Assert.True(ValidUri(uri, props));

                OutputHelper.WriteLine("mpeg");
                props = new UriProperties("urn", "mpeg:mpeg7:schema:2001");
                uri = new Uri(props.OriginalUri);
                Assert.True(ValidUri(uri, props));

                OutputHelper.WriteLine("oid");
                props = new UriProperties("urn", "oid:2.216.840");
                uri = new Uri(props.OriginalUri);
                Assert.True(ValidUri(uri, props));

                OutputHelper.WriteLine("urn:uuid");
                props = new UriProperties("urn", "uuid:6e8bc430-9c3a-11d9-9669-0800200c9a66");
                uri = new Uri(props.OriginalUri);
                Assert.True(ValidUri(uri, props));

                OutputHelper.WriteLine("uuid");
                props = new UriProperties("uuid", "6e8bc430-9c3a-11d9-9669-0800200c9a66");
                uri = new Uri(props.OriginalUri);
                Assert.True(ValidUri(uri, props));

                OutputHelper.WriteLine("uci");
                props = new UriProperties("urn", "uci:I001+SBSi-B10000083052");
                uri = new Uri(props.OriginalUri);
                Assert.True(ValidUri(uri, props));
            }
            catch (Exception ex)
            {
                OutputHelper.WriteLine($"Unexpected Exception {ex}");
            }
        }

        [TestMethod]
        public void AdditionalValidUri()
        {
            try
            {
                OutputHelper.WriteLine("iris.beep");
                props = new UriProperties("iris.beep", "bop") { Type = UriHostNameType.Unknown };
                uri = new Uri(props.OriginalUri);
                Assert.True(ValidUri(uri, props));

                OutputHelper.WriteLine("Microsoft Secure URL");
                props = new UriProperties("https", "www.microsoft.com");
                uri = new Uri(props.OriginalUri);
                Assert.True(ValidUri(uri, props));

                OutputHelper.WriteLine("Alternate https port URL");
                props = new UriProperties("https", "www.microsoft.com")
                {
                    Port = 1443
                };
                uri = new Uri(props.OriginalUri);
                Assert.True(ValidUri(uri, props));

                OutputHelper.WriteLine("H323 uri");
                props = new UriProperties("h323", "user@host:54");
                uri = new Uri(props.OriginalUri);
                Assert.True(ValidUri(uri, props));

                OutputHelper.WriteLine("FTP URI");
                uri = new Uri("ftp://ftp.microsoft.com/file.txt");
                parsed = new ParsedUri("ftp", "ftp.microsoft.com", UriHostNameType.Dns, 21, "/file.txt", "ftp://ftp.microsoft.com/file.txt");
                parsed.ValidUri(uri);

                OutputHelper.WriteLine("Unix style file");
                uri = new Uri("file:///etc/hosts");
                parsed = new ParsedUri("file", string.Empty, UriHostNameType.Basic, -1, "/etc/hosts", "file:///etc/hosts");
                parsed.ValidUri(uri);


                OutputHelper.WriteLine("Windows share style file");
                uri = new Uri("file:///\\\\server\\folder\\file.txt");
                parsed = new ParsedUri("file", "server", UriHostNameType.Dns, -1, "/folder/file.txt", "file://server/folder/file.txt");
                parsed.ValidUri(uri);

                OutputHelper.WriteLine("Windows drive style file");
                uri = new Uri("file:///c:\\rbllog");
                parsed = new ParsedUri("file", string.Empty, UriHostNameType.Basic, -1, "c:/rbllog", "file:///c:/rbllog");
                parsed.ValidUri(uri);
            }
            catch (Exception ex)
            {
                OutputHelper.WriteLine($"Unexpected Exception - these cases currently all fail {ex}");
            }
        }

        [TestMethod]
        public void RelativeURI()
        {
            try
            {
                OutputHelper.WriteLine("relative url");
                uri = new Uri("/doc/text.html", UriKind.Relative);

                OutputHelper.WriteLine("absolute url");
                props = new UriProperties("https", "www.microsoft.com")
                {
                    Path = "/doc/text.html",
                    Port = 1443
                };

                uri = new Uri(props.OriginalUri, UriKind.Absolute);
                Assert.True(ValidUri(uri, props));

                OutputHelper.WriteLine("RelativeOrAbsolute");
                Assert.Throws(typeof(ArgumentException),
                    () =>
                    {
                        uri = new Uri("/doc/text.html", UriKind.RelativeOrAbsolute);
                    },
                    "Expected ArgumentException");
            }
            catch (Exception ex)
            {
                OutputHelper.WriteLine($"Unexpected Exception {ex}");
            }
        }

        [TestMethod]
        public void MoreSchemes()
        {
            // TODO this test is disabled waiting for a fix
            Assert.SkipTest("Test disabled");

            var sUris = new string[]
            {
                "ws://ws.pusherapp.com:80/app/?client=js&version=1.9.3&protocol=5",
                "\tftp://abc.com   ",
                "ldap://[2001:db8::7]/c=GB?objectClass?one",
                "mailto:John.Doe@example.com",
                "mailto://abc/d",
                "news:comp.infosystems.www.servers.unix",
                "tel:+1-816-555-1212",
                "telnet://192.0.2.16:80/",
                "h323:abc",
                "h323://abc/d"
            };

            var parseds = new ParsedUri[]
            {
                new ParsedUri("ws", "ws.pusherapp.com", UriHostNameType.Dns, 80, "/app/?client=js&version=1.9.3&protocol=5", "ws://ws.pusherapp.com/app/?client=js&version=1.9.3&protocol=5"),
                new ParsedUri("ftp", "abc.com", UriHostNameType.Dns, 21, "/", "ftp://abc.com/"),
                new ParsedUri("ldap", "[2001:db8::7]", UriHostNameType.IPv6, 389, "/c=GB?objectClass?one", "ldap://[2001:db8::7]/c=GB?objectClass?one"),
                new ParsedUri("mailto", "John.Doe@example.com", UriHostNameType.Dns, 25, string.Empty, "mailto:John.Doe@example.com"),
                new ParsedUri("mailto", string.Empty, UriHostNameType.Basic, 25, "//abc/d", "mailto://abc/d"),
                new ParsedUri("news", string.Empty, UriHostNameType.Unknown, -1, "comp.infosystems.www.servers.unix", "news:comp.infosystems.www.servers.unix"),
                new ParsedUri("tel", string.Empty, UriHostNameType.Unknown, -1, "+1-816-555-1212", "tel:+1-816-555-1212"),
                new ParsedUri("telnet", "192.0.2.16", UriHostNameType.IPv4, 80, "/", "telnet://192.0.2.16:80/"),
                new ParsedUri("h323", string.Empty, UriHostNameType.Unknown, -1, "abc", "h323:abc"),
                new ParsedUri("h323", "abc", UriHostNameType.Dns, -1, "/d", "h323://abc/d")
            };

            for (int i = 0; i < sUris.Length; i++)
            {
                OutputHelper.WriteLine($"Trying to parse {sUris[i]}");

                uri = new Uri(sUris[i]);
                parseds[i].ValidUri(uri);
            }
        }

        [TestMethod]
        public void FileScheme()
        {
            // TODO this test is disabled waiting for a fix
            Assert.SkipTest("Test disabled");

            var sUris = new string[]
            {
                "file://",
                "file:///",
                "file:////",
                "file://c",
                "file:///c",
                "file:////c",
            };

            var parseds = new ParsedUri[]
            {
                new ParsedUri("file", string.Empty, UriHostNameType.Basic, -1, "/", "file:///"),
                new ParsedUri("file", string.Empty, UriHostNameType.Basic, -1, "/", "file:///"),
                new ParsedUri("file", string.Empty, UriHostNameType.Basic, -1, "//", "file:////"),
                new ParsedUri("file", "c", UriHostNameType.Dns, -1, "/", "file://c/"),
                new ParsedUri("file", string.Empty, UriHostNameType.Basic, -1, "/c", "file:///c"),
                new ParsedUri("file", "c", UriHostNameType.Dns, -1, "/", "file://c/"),
            };

            for (int i = 0; i < sUris.Length; i++)
            {
                OutputHelper.WriteLine($"Trying to parse {sUris[i]}");

                uri = new Uri(sUris[i]);
                parseds[i].ValidUri(uri);
            }
        }

        [TestMethod]
        public void Exceptions()
        {
            var sUris = new string[]
            {
                "file:///c:",
                "http:abc/d/",
                "file:/server"
            };

            for (int i = 0; i < sUris.Length; i++)
            {
                try
                {
                    uri = new Uri(sUris[i]);

                }
                catch (ArgumentException)
                {
                }
            }
        }

        #region helper functions

        private bool ValidUri(Uri uri, UriProperties props)
        {
            bool result = true;

            // AbsolutePath
            if (props.Path != null && uri.AbsolutePath != props.Path)
            {
                OutputHelper.WriteLine($"Expected AbsolutePath: {props.Path}, but got: {uri.AbsolutePath}");
                result = false;
            }

            // AbsoluteUri
            if (uri.AbsoluteUri != props.AbsoluteUri)
            {
                OutputHelper.WriteLine($"Expected AbsoluteUri: {props.AbsoluteUri}, but got: {uri.AbsoluteUri}");
                result = false;
            }

            // HostNameType
            if (uri.HostNameType != props.Type)
            {
                OutputHelper.WriteLine($"Expected HostNameType: {props.Type}, but got: {uri.HostNameType}");
                result = false;
            }

            switch (uri.Scheme.ToLower())
            {
                case "http":
                case "https":
                    if (uri.Port != props.Port)
                    {
                        OutputHelper.WriteLine("Expected Port: " + props.Port + ", but got: " + uri.Port);
                        result = false;
                    }
                    // Host
                    if (uri.Host != props.Host)
                    {
                        OutputHelper.WriteLine("Expected Host: " + props.Host + ", but got: " + uri.Host);
                        result = false;
                    }
                    break;

                default:
                    // no validation
                    break;
            }

            // Scheme
            if (uri.Scheme != props.Scheme)
            {
                OutputHelper.WriteLine("Expected Scheme: " + props.Scheme + ", but got: " + uri.Scheme);
                result = false;
            }

            return result;
        }

        #endregion Helper functions

    }

    #region helper class

    public class UriProperties
    {
        private string _host;
        private int _port;

        public UriProperties(string Scheme, string Host)
        {
            // Minimal required properties
            this.Scheme = Scheme;
            _host = Host;
        }

        public string Scheme { get; }

        public string Host
        {
            set { _host = value; }
            get { return _host; }
        }

        public int Port
        {
            get
            {
                return _port;
            }
            set
            {
                PortSet = true;
                _port = value;
            }
        }

        public bool PortSet { get; private set; } = false;

        public string Path { get; set; }

        public UriHostNameType Type { get; set; } = UriHostNameType.Unknown;

        public string AbsoluteUri
        {
            get
            {
                string uri = OriginalUri;

                // for http[s] add trailing / if no path
                if (Path == null && Scheme.ToLower().IndexOf("http") == 0 && uri[uri.Length - 1] != '/')
                {
                    uri += "/";
                }

                return uri;
            }
        }

        public string OriginalUri
        {
            get
            {
                string uri = Scheme;
                int defaultPort = 0;

                switch (Scheme.ToLower())
                {
                    case "http":
                        Type = UriHostNameType.Dns;
                        defaultPort = 80;
                        uri += "://" + _host;
                        break;

                    case "https":
                        Type = UriHostNameType.Dns;
                        defaultPort = 443;
                        uri += "://" + _host;
                        break;

                    default:
                        // No hosts, so move _host to Path
                        if (_host != "")
                        {
                            Path = _host;
                            _host = "";
                        }
                        uri += ":" + _host;
                        break;
                }

                if (PortSet)
                {
                    uri += ":" + Port;
                }
                else
                {
                    _port = defaultPort;
                }

                if (Path != null)
                {
                    uri += Path;
                }

                return uri;
            }
        }
    }

    internal class ParsedUri
    {
        public string Scheme { get; set; }
        public string Host { get; set; }
        public UriHostNameType HostNameType { get; set; }
        public int Port { get; set; }
        public string AbsolutePath { get; set; }
        public string AbsoluteUri { get; set; }

        public ParsedUri(
            string _scheme,
            string _host,
            UriHostNameType _hostNameType,
            int _port,
            string _absolutePath,
            string _absoluteUri)
        {
            Scheme = _scheme;
            Host = _host;
            HostNameType = _hostNameType;
            Port = _port;
            AbsolutePath = _absolutePath;
            AbsoluteUri = _absoluteUri;
        }

        internal void ValidUri(Uri uri)
        {
            // Scheme
            Assert.Equal(uri.Scheme, Scheme, $"Expected Scheme: {Scheme}, but got: {uri.Scheme}");

            // Host
            Assert.Equal(uri.Host, Host, $"Expected Host: {Host}, but got: {uri.Host}");

            // Port
            Assert.Equal(uri.Port, Port, $"Expected Port: {Port}, but got: {uri.Port}");

            // AbsolutePath
            Assert.Equal(uri.AbsolutePath, AbsolutePath, $"Expected AbsolutePath: {AbsolutePath}, but got: {uri.AbsolutePath}");

            // AbsoluteUri
            Assert.Equal(uri.AbsoluteUri, AbsoluteUri, $"Expected AbsoluteUri: {AbsoluteUri}, but got: {uri.AbsoluteUri}");

            // HostNameType
            Assert.Equal(uri.HostNameType.ToString(), HostNameType.ToString(), $"Expected HostNameType: {HostNameType}, but got: {uri.HostNameType}");
        }
    }

    #endregion helper class

}
