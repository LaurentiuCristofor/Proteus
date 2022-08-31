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
    /// An extractor that packages each input line in a StringParts instance.
    /// </summary>
    public class LineAsStringPartsExtractor : IDataExtractor<UnusedType, StringParts>
    {
        public void Initialize(UnusedType unusedExtractionParameters)
        {
        }

        public StringParts ExtractData(ulong lineNumber, string line)
        {
            DataExtractorValidation.ValidateLine(line);

            DataTypeContainer lineContainer = new DataTypeContainer(DataType.String, line);
            StringParts lineParts = new StringParts(line, null, null, lineContainer, string.Empty, string.Empty);
            return lineParts;
        }
    }
}
