//
// Copyright (c) 2018 The nanoFramework project contributors
// Portions Copyright (c) Microsoft Corporation.  All rights reserved.
// See LICENSE file in the project root for full license information.
//

namespace System.Net
{
    internal class WebRequestPrefixElement
    {
        public string Prefix;
        public IWebRequestCreate Creator;

        public WebRequestPrefixElement(string P, IWebRequestCreate C)
        {
            Prefix = P;
            Creator = C;
        }

    } // class PrefixListElement

} // namespace System.Net


