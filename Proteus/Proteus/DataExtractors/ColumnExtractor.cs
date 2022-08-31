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
    /// A column extractor that does the following:
    /// - extracts a specific column from a line.
    /// - parses the column value into a specific type.
    /// - returns a StringParts instance that packs the original line parts with the parsed column value.
    /// </summary>
    public class ColumnExtractor : IDataExtractor<ColumnExtractionParameters, StringParts>
    {
        protected ColumnExtractionParameters Parameters { get; set; }

        public void Initialize(ColumnExtractionParameters extractionParameters)
        {
            this.Parameters = extractionParameters;
        }

        public StringParts ExtractData(ulong lineNumber, string line)
        {
            DataExtractorValidation.ValidateLine(line);

            // Split the line into columns using the separator parameter.
            //
            string[] columns = line.Split(this.Parameters.Separators, StringSplitOptions.None);

            if (this.Parameters.ColumnNumber > columns.Length)
            {
                OutputInterface.LogWarning($"\nLine {lineNumber} is too short for column number {this.Parameters.ColumnNumber}!");
                return null;
            }

            // Extract the column value.
            //
            int columnIndex = this.Parameters.ColumnNumber - 1;
            string columnValue = columns[columnIndex];

            // Parse the column according to the data type parameter.
            //
            DataTypeContainer columnContainer = new DataTypeContainer(this.Parameters.ColumnDataType);
            if (!columnContainer.TryParseStringValue(columnValue))
            {
                OutputInterface.LogWarning($"\nInvalid value for column {this.Parameters.ColumnNumber} of line {lineNumber}: {columnValue}!");
                return null;
            }

            // Also extract the data before and after the column, so we can put the line back together, if needed.
            //
            string prefixString = string.Join(this.Parameters.Separators[0], columns, 0, columnIndex);
            if (prefixString != string.Empty)
            {
                prefixString = prefixString + this.Parameters.Separators[0];
            }

            string suffixString = string.Join(this.Parameters.Separators[0], columns, columnIndex + 1, columns.Length - 1 - columnIndex);
            if (suffixString != string.Empty)
            {
                suffixString = this.Parameters.Separators[0] + suffixString;
            }

            // Pack the line with the extra parts that preceded and followed the column.
            //
            StringParts stringParts = new StringParts(line, this.Parameters.Separators[0], columns, columnContainer, prefixString, suffixString);
            
            // Return the string parts.
            //
            return stringParts;
        }
    }
}
