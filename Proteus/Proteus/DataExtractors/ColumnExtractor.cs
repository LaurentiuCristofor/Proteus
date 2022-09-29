////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// 
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;

using LaurentiuCristofor.Proteus.Common;
using LaurentiuCristofor.Proteus.Common.DataHolders;

namespace LaurentiuCristofor.Proteus.DataExtractors
{
    /// <summary>
    /// A column extractor that does the following:
    /// - extracts a specific column from a line.
    /// - parses the column value into a specific type.
    /// - returns a ParsedLine instance that packs the original line parts along the parsed column value.
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
            // Split the line into columns using the separator parameter.
            //
            string[] columns = line.Split(this.Parameters.Separators, StringSplitOptions.None);

            if (this.Parameters.ColumnNumber > columns.Length)
            {
                OutputInterface.LogWarning($"\nLine {lineNumber} is too short for extracting column number {this.Parameters.ColumnNumber}!");
                return null;
            }

            // Extract the column value.
            //
            int columnIndex = this.Parameters.ColumnNumber - 1;
            string columnString = columns[columnIndex];

            // Parse the column value according to the data type parameter.
            //
            IDataHolder columnData = DataHolderOperations.BuildDataHolder(this.Parameters.ColumnDataType, columnString);
            if (columnData == null)
            {
                OutputInterface.LogWarning($"\nFound an invalid value for column {this.Parameters.ColumnNumber} of type {this.Parameters.ColumnDataType} in line {lineNumber}: '{columnString}'!");
                return null;
            }

            // Check if we need to extract a second column value.
            //
            IDataHolder secondColumnData = null;
            if (this.Parameters.SecondColumnNumber > 0)
            {
                int secondColumnIndex = this.Parameters.SecondColumnNumber - 1;
                string secondColumnString = columns[secondColumnIndex];

                // Parse the second column value according to the data type parameter.
                //
                secondColumnData = DataHolderOperations.BuildDataHolder(this.Parameters.SecondColumnDataType, secondColumnString);
                if (secondColumnData == null)
                {
                    OutputInterface.LogWarning($"\nFound an invalid value for column {this.Parameters.SecondColumnNumber} of type {this.Parameters.SecondColumnDataType} in line {lineNumber}: '{secondColumnString}'!");
                    return null;
                }
            }

            // Package all the information that we extracted from the line.
            //
            ParsedLine lineData = new ParsedLine(line, columnData, this.Parameters.ColumnNumber, columns, this.Parameters.Separators[0], secondColumnData);
            
            // Return the line data.
            //
            return lineData;
        }
    }
}
