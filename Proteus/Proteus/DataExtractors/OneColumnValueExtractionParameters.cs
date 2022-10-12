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
    /// Specifies the operating parameters for the extraction of a column value in addition to the extraction of columns as strings.
    /// </summary>
    public class OneColumnValueExtractionParameters : ColumnStringsExtractionParameters
    {
        /// <summary>
        /// The number of the column, starting from 1.
        /// Subtract 1 to obtain its index.
        /// </summary>
        public int ColumnNumber { get; protected set; }

        /// <summary>
        /// The data type that should be parsed from the column value.
        /// </summary>
        public DataType ColumnDataType { get; protected set; }

        public OneColumnValueExtractionParameters(
            string separator,
            int columnNumber,
            DataType dataType)
            : base(separator)
        {
            ArgumentChecker.CheckGreaterThanOrEqualTo(columnNumber, 1);
            ArgumentChecker.CheckDataType(dataType);

            this.ColumnNumber = columnNumber;
            this.ColumnDataType = dataType;
        }
    }
}
