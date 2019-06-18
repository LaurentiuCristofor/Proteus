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
    /// - returns a Tuple that packs the original line parts with the parsed column value.
    /// </summary>
    public class ColumnExtractor : IDataExtractor<ColumnExtractionParameters, Tuple<LineParts, DataTypeContainer>>
    {
        protected ColumnExtractionParameters Parameters { get; set; }

        public void Initialize(ColumnExtractionParameters extractionParameters)
        {
            this.Parameters = extractionParameters;
        }

        public Tuple<LineParts, DataTypeContainer> ExtractData(ulong lineNumber, string inputLine)
        {
            // Split the line into columns using the separator parameter.
            //
            string[] columns = inputLine.Split(this.Parameters.Separators, StringSplitOptions.None);

            if (this.Parameters.ColumnNumber > columns.Length)
            {
                IOStream.LogWarning($"\nLine {lineNumber} is too short for column number {this.Parameters.ColumnNumber}!");
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
                IOStream.LogWarning($"\nInvalid value for column {this.Parameters.ColumnNumber} of line {lineNumber}: {columnValue}!");
                return null;
            }

            // Also extract the data before and after the column, so we can keep all line parts.
            //
            string dataBeforeColumn = string.Join(this.Parameters.Separators[0], columns, 0, columnIndex);
            if (dataBeforeColumn != string.Empty)
            {
                dataBeforeColumn = dataBeforeColumn + this.Parameters.Separators[0];
            }

            string dataAfterColumn = string.Join(this.Parameters.Separators[0], columns, columnIndex + 1, columns.Length - 1 - columnIndex);
            if (dataAfterColumn != string.Empty)
            {
                dataAfterColumn = this.Parameters.Separators[0] + dataAfterColumn;
            }

            // Pack the line with the extra parts that preceded and followed the column.
            //
            LineParts lineParts = new LineParts(inputLine, dataBeforeColumn, dataAfterColumn);
            
            // Pack the parts with the column data container.
            //
            var tuple = new Tuple<LineParts, DataTypeContainer>(lineParts, columnContainer);

            // Return the tuple package.
            //
            return tuple;
        }
    }
}
