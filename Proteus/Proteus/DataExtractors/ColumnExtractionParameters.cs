////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using LaurentiuCristofor.Proteus.Common;

namespace LaurentiuCristofor.Proteus.DataExtractors
{
    /// <summary>
    /// Specifies the operating parameters for a column extraction.
    /// </summary>
    public class ColumnExtractionParameters
    {
        /// <summary>
        /// The string that should be used as column separator.
        /// Despite this being an array, only one separator is ever provided.
        /// </summary>
        public string[] Separators { get; protected set; }

        /// <summary>
        /// The number of the column, starting from 1.
        /// Subtract 1 to obtain its index.
        /// </summary>
        public int ColumnNumber { get; protected set; }

        /// <summary>
        /// The number of a second column, starting from 1.
        /// Subtract 1 to obtain its index.
        /// </summary>
        public int SecondColumnNumber { get; protected set; }

        /// <summary>
        /// The data type that should be parsed from the column value.
        /// </summary>
        public DataType ColumnDataType { get; protected set; }

        /// <summary>
        /// The data type that should be parsed from the second column value.
        /// </summary>
        public DataType SecondColumnDataType { get; protected set; }

        public bool ConstructLinePrefixAndSuffix { get; protected set; }

        public ColumnExtractionParameters(
            string separator,
            int columnNumber,
            DataType dataType,
            bool constructLinePrefixAndSuffix)
        {
            ArgumentChecker.CheckPositive(columnNumber);
            ArgumentChecker.CheckDataType(dataType);

            this.Separators = new string[1];
            this.Separators[0] = separator;
            this.ColumnNumber = columnNumber;
            this.ColumnDataType = dataType;
            this.SecondColumnNumber = 0;
            this.ColumnDataType = DataType.NotSet;
            this.ConstructLinePrefixAndSuffix = constructLinePrefixAndSuffix;
        }

        public ColumnExtractionParameters(
            string separator,
            int columnNumber,
            DataType dataType,
            int secondColumnNumber,
            DataType secondDataType)
        {
            ArgumentChecker.CheckPositive(columnNumber);
            ArgumentChecker.CheckDataType(dataType);
            ArgumentChecker.CheckPositive(secondColumnNumber);
            ArgumentChecker.CheckDataType(secondDataType);

            this.Separators = new string[1];
            this.Separators[0] = separator;
            this.ColumnNumber = columnNumber;
            this.ColumnDataType = dataType;
            this.SecondColumnNumber = secondColumnNumber;
            this.SecondColumnDataType = secondDataType;
            this.ConstructLinePrefixAndSuffix = false;
        }
    }
}
