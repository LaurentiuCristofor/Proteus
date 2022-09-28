﻿////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// 
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using LaurentiuCristofor.Proteus.Common;
using LaurentiuCristofor.Proteus.Common.DataHolders;

namespace LaurentiuCristofor.Proteus.DataExtractors
{
    /// <summary>
    /// An extractor that packages each input line in a ParsedLine instance without doing any actual parsing.
    /// </summary>
    public class LineAsParsedLineExtractor : IDataExtractor<Unused, ParsedLine>
    {
        public void Initialize(Unused unusedExtractionParameters)
        {
        }

        public ParsedLine ExtractData(ulong lineNumber, string line)
        {
            IDataHolder lineContainer = new StringDataHolder(line);
            ParsedLine lineData = new ParsedLine(line, lineContainer);
            return lineData;
        }
    }
}
