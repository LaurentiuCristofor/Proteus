////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
///
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;

namespace LaurentiuCristofor.Proteus.DataExtractors
{
    /// <summary>
    /// A column extractor that extracts all columns into a string array.
    /// </summary>
    public class ColumnStringsExtractor : IDataExtractor<ColumnStringsExtractionParameters, ExtractedColumnStrings>
    {
        protected ColumnStringsExtractionParameters Parameters { get; set; }

        public void Initialize(ColumnStringsExtractionParameters extractionParameters)
        {
            this.Parameters = extractionParameters;
        }

        public ExtractedColumnStrings ExtractData(ulong lineNumber, string line)
        {
            // Split the line into columns using the separator parameter.
            //
            string[] columns = line.Split(this.Parameters.Separators, StringSplitOptions.None);

            return new ExtractedColumnStrings(line, columns, this.Parameters.Separators[0]);
        }
    }
}
