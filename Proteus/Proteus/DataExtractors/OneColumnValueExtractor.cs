////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// 
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;

using LaurentiuCristofor.Proteus.Common;
using LaurentiuCristofor.Proteus.Common.Types;
using LaurentiuCristofor.Proteus.Common.DataHolders;

namespace LaurentiuCristofor.Proteus.DataExtractors
{
    /// <summary>
    /// A column extractor that extracts a column value in addition to the extraction of columns as strings.
    /// </summary>
    public class OneColumnValueExtractor : IDataExtractor<OneColumnValueExtractionParameters, OneExtractedValue>
    {
        protected OneColumnValueExtractionParameters Parameters { get; set; }

        public void Initialize(OneColumnValueExtractionParameters extractionParameters)
        {
            this.Parameters = extractionParameters;
        }

        public OneExtractedValue ExtractData(ulong lineNumber, string line)
        {
            // Split the line into columns using the separator parameter.
            //
            string[] columns = line.Split(this.Parameters.Separators, StringSplitOptions.None);

            IDataHolder columnData = ExtractColumn(lineNumber, columns, this.Parameters.ColumnNumber, this.Parameters.ColumnDataType);
            if (columnData == null)
            {
                return null;
            }

            // Package and return all the information that we extracted from the line.
            //
            return new OneExtractedValue(line, columns, this.Parameters.Separators[0], columnData, this.Parameters.ColumnNumber);
        }

        /// <summary>
        /// Extracts a column value from a columns string array according to the data type passed as argument.
        /// </summary>
        /// <param name="lineNumber">The number of the line that we are processing.</param>
        /// <param name="columns">The columns string array.</param>
        /// <param name="columnNumber">The number of the column to extract.</param>
        /// <param name="columnDataType">The type of the column to extract.</param>
        /// <returns>An IDataHolder instance containing the column's value or null if the column didn't exist or if its value didn't match the specified data type.</returns>
        internal static IDataHolder ExtractColumn(ulong lineNumber, string[] columns, int columnNumber, DataType columnDataType)
        {
            if (columnNumber > columns.Length)
            {
                OutputInterface.LogWarning($"\nLine {lineNumber} is too short for extracting column number {columnNumber}!");
                return null;
            }

            int columnIndex = columnNumber - 1;
            string columnString = columns[columnIndex];

            // Parse the column value according to the data type parameter.
            //
            IDataHolder columnData = DataHolderOperations.BuildDataHolder(columnDataType, columnString);
            if (columnData == null)
            {
                OutputInterface.LogWarning($"\nFound an invalid value for column {columnNumber} of type {columnDataType} in line {lineNumber}: '{columnString}'!");
                return null;
            }
            return columnData;
        }
    }
}
