//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright (c) Microsoft Corporation.  All rights reserved.
// See LICENSE file in the project root for full license information.
//

using System.Diagnostics;

namespace System.Net.Http.Headers
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class HttpRequestHeaders : HttpHeaders
    {
        private HttpGeneralHeaders _generalHeaders;
        private HttpGeneralHeaders _expect;
        //private bool _expectContinueSet;

        #region Request Headers

        #endregion

        #region General Headers

        public string Connection
        {
            get 
            { 
                var connectionHeader = _headerStore.GetValues(HttpKnownHeaderNames.Connection);
                if (connectionHeader is not null)
                {
                    return connectionHeader[0];
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public bool ConnectionClose
        {
            get
            {
                return Connection.Contains("close");
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public bool TransferEncodingChunked
        {
            get
            {
                return _headerStore[HttpKnownHeaderNames.TransferEncoding].Contains("chunked");
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        internal HttpRequestHeaders()
            : base()
        {
        }

        internal override void AddHeaders(HttpHeaders sourceHeaders)
        {
            base.AddHeaders(sourceHeaders);

            HttpRequestHeaders sourceRequestHeaders = sourceHeaders as HttpRequestHeaders;
            
            Debug.Assert(sourceRequestHeaders != null);

            //// Copy special values but do not overwrite.
            //if (sourceRequestHeaders._generalHeaders != null)
            //{
            //    GeneralHeaders.AddSpecialsFrom(sourceRequestHeaders._generalHeaders);
            //}

            //bool expectContinue = ExpectContinue;
            //if (expectContinue)
            //{
            //    ExpectContinue = sourceRequestHeaders.ExpectContinue;
            //}
        }

        private HttpGeneralHeaders GeneralHeaders => _generalHeaders ?? (_generalHeaders = new HttpGeneralHeaders(this));
    }
}
