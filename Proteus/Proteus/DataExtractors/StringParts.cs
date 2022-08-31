////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;

using LaurentiuCristofor.Proteus.Common;

namespace LaurentiuCristofor.Proteus.DataExtractors
{
    /// <summary>
    /// A packaging of the string parts that result from the extraction of a substring part.
    /// </summary>
    public class StringParts
    {
        // The original string, unchanged.
        //
        public string OriginalString { get; protected set; }

        // The original string separator.
        //
        public string OriginalStringSeparator { get; protected set; }

        // The parts of the original string; typically, column values.
        //
        public string[] OriginalStringParts { get; protected set; }

        /// <summary>
        /// The extracted string, packaged as a data type container.
        /// </summary>
        public DataTypeContainer ExtractedData { get; protected set;}

        // The string prefix that preceded the extracted string.
        //
        public string PrefixString { get; protected set; }

        // The string suffix that followed the extracted string.
        public string SuffixString { get; protected set; }

        public StringParts(string originalString, string originalStringSeparator, string[] originalStringParts, DataTypeContainer extractedData, string prefixString, string suffixString)
        {
            this.OriginalString = originalString;
            this.OriginalStringSeparator = originalStringSeparator;
            this.OriginalStringParts = originalStringParts;
            this.ExtractedData = extractedData;
            this.PrefixString = prefixString;
            this.SuffixString = suffixString;
        }
    }
}
