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
    public class TwoColumnValuesExtractionParameters : OneColumnValueExtractionParameters
    {
        /// <summary>
        /// The number of a second column, starting from 1.
        /// Subtract 1 to obtain its index.
        /// </summary>
        public int SecondColumnNumber { get; protected set; }

        /// <summary>
        /// The data type that should be parsed from the second column value.
        /// </summary>
        public DataType SecondColumnDataType { get; protected set; }

        public TwoColumnValuesExtractionParameters(
            string separator,
            int columnNumber,
            DataType dataType,
            int secondColumnNumber,
            DataType secondDataType)
            : base (separator, columnNumber, dataType)
        {
            ArgumentChecker.CheckStrictlyPositive(secondColumnNumber);
            ArgumentChecker.CheckDataType(secondDataType);
            ArgumentChecker.CheckDifferent(columnNumber, secondColumnNumber);

            this.SecondColumnNumber = secondColumnNumber;
            this.SecondColumnDataType = secondDataType;
        }
    }
}
