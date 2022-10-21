////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
///
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;

using LaurentiuCristofor.Proteus.Common.DataHolders;

namespace LaurentiuCristofor.Proteus.DataExtractors
{
    /// <summary>
    /// A column extractor that extracts two column values in addition to the extraction of columns as strings.
    /// </summary>
    public class TwoColumnValuesExtractor : IDataExtractor<TwoColumnValuesExtractionParameters, TwoExtractedValues>
    {
        protected TwoColumnValuesExtractionParameters Parameters { get; set; }

        public void Initialize(TwoColumnValuesExtractionParameters extractionParameters)
        {
            Parameters = extractionParameters;
        }

        public TwoExtractedValues ExtractData(ulong lineNumber, string line)
        {
            // Split the line into columns using the separator parameter.
            //
            string[] columns = line.Split(Parameters.Separators, StringSplitOptions.None);

            // Try to extract the first column value.
            //
            IDataHolder columnData = OneColumnValueExtractor.ExtractColumn(lineNumber, columns, Parameters.ColumnNumber, Parameters.ColumnDataType);
            if (columnData == null)
            {
                return null;
            }

            // Try to extract the second column value.
            //
            IDataHolder secondColumnData = OneColumnValueExtractor.ExtractColumn(lineNumber, columns, Parameters.SecondColumnNumber, Parameters.SecondColumnDataType);
            if (secondColumnData == null)
            {
                return null;
            }

            // Package and return all the information that we extracted from the line.
            //
            return new TwoExtractedValues(line, columns, Parameters.Separators[0], columnData, Parameters.ColumnNumber, secondColumnData);
        }
    }
}
