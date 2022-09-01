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
    /// An extractor that packages each input line in a ParsedLine instance without doing any actual parsing.
    /// </summary>
    public class LineAsParsedLineExtractor : IDataExtractor<UnusedType, ParsedLine>
    {
        public void Initialize(UnusedType unusedExtractionParameters)
        {
        }

        public ParsedLine ExtractData(ulong lineNumber, string line)
        {
            DataExtractorValidation.ValidateLine(line);

            DataTypeContainer lineContainer = new DataTypeContainer(DataType.String, line);
            ParsedLine lineParts = new ParsedLine(line, null, null, lineContainer, string.Empty, string.Empty);
            return lineParts;
        }
    }
}
