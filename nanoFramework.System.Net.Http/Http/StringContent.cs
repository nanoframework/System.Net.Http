using System;
using System.Text;

namespace System.Net.Http
{
    public class StringContent : ByteArrayContent
    {
        private const string DefaultMediaType = "text/plain";

        public StringContent(string content)
            : this(content, null, null)
        {
        }

        public StringContent(string content, Encoding encoding)
            : this(content, encoding, null)
        {
        }

        public StringContent(string content, Encoding encoding, string mediaType)
            : base(GetContentByteArray(content, encoding))
        {
            // Initialize the 'Content-Type' header with information provided by parameters.
            //MediaTypeHeaderValue headerValue = new MediaTypeHeaderValue((mediaType == null) ? DefaultMediaType : mediaType);
            //headerValue.CharSet = (encoding == null) ? HttpContent.DefaultStringEncoding.WebName : encoding.WebName;

            // TODO
            //Headers.ContentType = headerValue;
        }

        // A StringContent is essentially a ByteArrayContent. We serialize the string into a byte-array in the
        // constructor using encoding information provided by the caller (if any). When this content is sent, the
        // Content-Length can be retrieved easily (length of the array).
        private static byte[] GetContentByteArray(string content, Encoding? encoding)
        {
            if(content is null)
            {
                throw new ArgumentNullException();
            }

            // In this case we treat 'null' strings different from string.Empty in order to be consistent with our
            // other *Content constructors: 'null' throws, empty values are allowed.

            encoding ??= DefaultStringEncoding;

            return encoding.GetBytes(content);
        }
    }
}
