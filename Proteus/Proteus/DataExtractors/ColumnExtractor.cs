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
    public class ColumnExtractor : IDataExtractor<ColumnExtractionParameters, ParsedLine>
    {
        protected ColumnExtractionParameters Parameters { get; set; }

        public void Initialize(ColumnExtractionParameters extractionParameters)
        {
            this.Parameters = extractionParameters;
        }

        public ParsedLine ExtractData(ulong lineNumber, string line)
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
            string columnString = columns[columnIndex];

            // Parse the column according to the data type parameter.
            //
            DataTypeContainer columnData = new DataTypeContainer(this.Parameters.ColumnDataType);
            if (!columnData.TryParseStringValue(columnString))
            {
                OutputInterface.LogWarning($"\nInvalid value for column {this.Parameters.ColumnNumber} of line {lineNumber}: {columnString}!");
                return null;
            }

            // Check if we need to extract a second column value.
            //
            DataTypeContainer secondColumnData = null;
            if (this.Parameters.SecondColumnNumber > 0)
            {
                int secondColumnIndex = this.Parameters.SecondColumnNumber - 1;
                string secondColumnString = columns[secondColumnIndex];

                secondColumnData = new DataTypeContainer(this.Parameters.SecondColumnDataType);
                if (!secondColumnData.TryParseStringValue(secondColumnString))
                {
                    OutputInterface.LogWarning($"\nInvalid value for column {this.Parameters.SecondColumnNumber} of line {lineNumber}: {secondColumnString}!");
                    return null;
                }
            }

            string linePrefix = null;
            string lineSuffix = null;
            if (this.Parameters.ConstructLinePrefixAndSuffix)
            {
                // Also extract the data before and after the column, so we can put the line back together, if needed.
                //
                linePrefix = string.Join(this.Parameters.Separators[0], columns, 0, columnIndex);
                if (linePrefix != string.Empty)
                {
                    linePrefix = linePrefix + this.Parameters.Separators[0];
                }

                lineSuffix = string.Join(this.Parameters.Separators[0], columns, columnIndex + 1, columns.Length - 1 - columnIndex);
                if (lineSuffix != string.Empty)
                {
                    lineSuffix = this.Parameters.Separators[0] + lineSuffix;
                }
            }

            // Package all the information that we processed for the line.
            //
            ParsedLine parsedLine = new ParsedLine(line, this.Parameters.Separators[0], columns, columnData, secondColumnData, linePrefix, lineSuffix);
            
            // Return the string parts.
            //
            return parsedLine;
        }
    }
}
