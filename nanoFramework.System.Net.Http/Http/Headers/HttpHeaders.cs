//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright (c) Microsoft Corporation.  All rights reserved.
// See LICENSE file in the project root for full license information.
//

using System;
using System.Diagnostics;

namespace System.Net.Http.Headers
{
    /// <summary>
    /// Key/value pairs of headers. The value is either a raw <see cref="string"/> or a <see cref="HttpHeaders.HeaderStoreItemInfo"/>.
    /// </summary>
    internal struct HeaderEntry
    {
        public HeaderDescriptor Key;
        public object Value;

        public HeaderEntry(HeaderDescriptor key, object value)
        {
            Key = key;
            Value = value;
        }
    }

    public abstract class HttpHeaders
    {
        internal WebHeaderCollection _headerStore = new WebHeaderCollection(true);

        private readonly HttpHeaderType _allowedHeaderTypes;
        private readonly HttpHeaderType _treatAsCustomHeaderTypes;

        protected HttpHeaders()
          : this(HttpHeaderType.All, HttpHeaderType.None)
        {
        }

        internal HttpHeaders(HttpHeaderType allowedHeaderTypes, HttpHeaderType treatAsCustomHeaderTypes)
        {
            // Should be no overlap
            Debug.Assert((allowedHeaderTypes & treatAsCustomHeaderTypes) == 0);

            _allowedHeaderTypes = allowedHeaderTypes & ~HttpHeaderType.NonTrailing;
            _treatAsCustomHeaderTypes = treatAsCustomHeaderTypes & ~HttpHeaderType.NonTrailing;
        }

        internal virtual void AddHeaders(HttpHeaders sourceHeaders)
        {
            if(_headerStore.Count == 0)
            {
                foreach (var headerKey in sourceHeaders._headerStore.AllKeys)
                {
                    _headerStore.Add(headerKey, sourceHeaders._headerStore[headerKey]);
                }
            }
            else
            {

            }
    
        }

        internal bool ContainsParsedValue(HeaderDescriptor descriptor, object value)
        {
            Debug.Assert(value != null);

            //// If we have a value for this header, then verify if we have a single value. If so, compare that
            //// value with 'item'. If we have a list of values, then compare each item in the list with 'item'.
            //if (TryGetAndParseHeaderInfo(descriptor, out HeaderStoreItemInfo info))
            //{
            //    Debug.Assert(descriptor.Parser != null, "Can't add parsed value if there is no parser available.");
            //    Debug.Assert(descriptor.Parser.SupportsMultipleValues,
            //        "This method should not be used for single-value headers. Use equality comparer instead.");

            //    // If there is no entry, just return.
            //    if (info.ParsedValue == null)
            //    {
            //        return false;
            //    }

            //    List<object>? parsedValues = info.ParsedValue as List<object>;

            //    IEqualityComparer? comparer = descriptor.Parser.Comparer;

            //    if (parsedValues == null)
            //    {
            //        Debug.Assert(info.ParsedValue.GetType() == value.GetType(),
            //            "Stored value does not have the same type as 'value'.");

            //        return AreEqual(value, info.ParsedValue, comparer);
            //    }
            //    else
            //    {
            //        foreach (object item in parsedValues)
            //        {
            //            Debug.Assert(item.GetType() == value.GetType(),
            //                "One of the stored values does not have the same type as 'value'.");

            //            if (AreEqual(value, item, comparer))
            //            {
            //                return true;
            //            }
            //        }

            //        return false;
            //    }
            //}

            return false;
        }

    }
}
