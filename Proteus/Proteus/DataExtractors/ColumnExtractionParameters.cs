////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// 
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using LaurentiuCristofor.Proteus.Common;
using LaurentiuCristofor.Proteus.Common.Types;

namespace LaurentiuCristofor.Proteus.DataExtractors
{
    /// <summary>
    /// Specifies the operating parameters for a column extraction.
    /// </summary>
    public class ColumnExtractionParameters
    {
        /// <summary>
        /// The string that should be used as column separator.
        /// 
        /// The String.Split() API expects a string array, so we store our column separator in an array of one element.
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

        /// <summary>
        /// This constructor is used when we just want to extract all columns as string values,
        /// but we are not interested in any specific column.
        /// </summary>
        /// <param name="separator">The column separator.</param>
        public ColumnExtractionParameters(
            string separator)
        {
            this.Separators = new string[1];
            this.Separators[0] = separator;
            this.ColumnNumber = 0;
            this.ColumnDataType = DataType.NotSet;
            this.SecondColumnNumber = 0;
            this.SecondColumnDataType = DataType.NotSet;
        }

        /// <summary>
        /// This constructor is used when we want to extract a specific column.
        /// </summary>
        /// <param name="separator">The column separator.</param>
        /// <param name="columnNumber">The number of the column to extract.</param>
        /// <param name="dataType">The data type of the column to extract.</param>
        public ColumnExtractionParameters(
            string separator,
            int columnNumber,
            DataType dataType)
        {
            ArgumentChecker.CheckStrictlyPositive(columnNumber);
            ArgumentChecker.CheckDataType(dataType);

            this.Separators = new string[1];
            this.Separators[0] = separator;
            this.ColumnNumber = columnNumber;
            this.ColumnDataType = dataType;
            this.SecondColumnNumber = 0;
            this.SecondColumnDataType = DataType.NotSet;
        }

        /// <summary>
        /// This constructor is used when we want to extract two specific columns.
        /// </summary>
        /// <param name="separator">The column separator.</param>
        /// <param name="columnNumber">The number of the first column to extract.</param>
        /// <param name="dataType">The data type of the first column to extract.</param>
        /// <param name="secondColumnNumber">The number of the second column to extract.</param>
        /// <param name="secondDataType">The data type of the second column to extract.</param>
        public ColumnExtractionParameters(
            string separator,
            int columnNumber,
            DataType dataType,
            int secondColumnNumber,
            DataType secondDataType)
        {
            ArgumentChecker.CheckStrictlyPositive(columnNumber);
            ArgumentChecker.CheckDataType(dataType);
            ArgumentChecker.CheckStrictlyPositive(secondColumnNumber);
            ArgumentChecker.CheckDataType(secondDataType);

            this.Separators = new string[1];
            this.Separators[0] = separator;
            this.ColumnNumber = columnNumber;
            this.ColumnDataType = dataType;
            this.SecondColumnNumber = secondColumnNumber;
            this.SecondColumnDataType = secondDataType;
        }
    }
}
