﻿////////////////////////////////////////////////////////////////////////////////////////////////////
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
    public class LineExtractor : IDataExtractor<UnusedType, StringParts>
    {
        public void Initialize(UnusedType unusedExtractionParameters)
        {
        }

        public StringParts ExtractData(ulong lineNumber, string inputLine)
        {
            DataTypeContainer lineContainer = new DataTypeContainer(DataType.String, inputLine);
            StringParts lineParts = new StringParts(inputLine, lineContainer, string.Empty, string.Empty);
            return lineParts;
        }
    }
}
