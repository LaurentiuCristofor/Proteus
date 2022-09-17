////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using LaurentiuCristofor.Proteus.Common;

namespace LaurentiuCristofor.Proteus.DataExtractors
{
    /// <summary>
    /// A packaging of the data extracted from parsing a line.
    /// </summary>
    public class ParsedLine
    {
        // The original line, unchanged.
        //
        public string OriginalLine { get; protected set; }

        /// <summary>
        /// The extracted data, packaged in a data type container.
        /// 
        /// This can be a column or a line.
        /// </summary>
        public DataTypeContainer ExtractedData { get; protected set;}

        /// <summary>
        /// The number of the extracted column, if ExtractedData contains a column.
        /// </summary>
        public int ExtractedColumnNumber { get; protected set; }

        // The column strings.
        //
        public string[] Columns { get; protected set; }

        // The column separator.
        //
        public string ColumnSeparator { get; protected set; }

        /// <summary>
        /// A second extracted data, packaged in a data type container.
        /// 
        /// This is typically a second column value.
        /// </summary>
        public DataTypeContainer SecondExtractedData { get; protected set; }

        public ParsedLine(
            string originalLine,
            DataTypeContainer extractedData,
            int extractedColumnNumber = 0,
            string[] columns = null,
            string columnSeparator = null,
            DataTypeContainer secondExtractedData = null)
        {
            this.OriginalLine = originalLine;
            this.ExtractedData = extractedData;
            this.ExtractedColumnNumber = extractedColumnNumber;
            this.Columns = columns;
            this.ColumnSeparator = columnSeparator;
            this.SecondExtractedData = secondExtractedData;
        }
    }
}
