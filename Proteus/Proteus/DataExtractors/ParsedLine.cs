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

        // The column separator.
        //
        public string ColumnSeparator { get; protected set; }

        // The column strings.
        //
        public string[] Columns { get; protected set; }

        /// <summary>
        /// The extracted data, packaged in a data type container.
        /// 
        /// This can be a column or a line.
        /// </summary>
        public DataTypeContainer ExtractedData { get; protected set;}

        /// <summary>
        /// A second extracted data, packaged in a data type container.
        /// 
        /// This is typically a second column value.
        /// </summary>
        public DataTypeContainer SecondExtractedData { get; protected set; }

        // The string prefix that preceded the extracted string.
        // This is used when editing columns, to construct the edited line.
        //
        public string LinePrefix { get; protected set; }

        // The string suffix that followed the extracted string.
        // This is used when editing columns, to construct the edited line.
        //
        public string LineSuffix { get; protected set; }

        public ParsedLine(
            string originalLine,
            string columnSeparator,
            string[] columns,
            DataTypeContainer extractedData,
            DataTypeContainer secondExtractedData,
            string linePrefix,
            string lineSuffix)
        {
            this.OriginalLine = originalLine;
            this.ColumnSeparator = columnSeparator;
            this.Columns = columns;
            this.ExtractedData = extractedData;
            this.SecondExtractedData = secondExtractedData;
            this.LinePrefix = linePrefix;
            this.LineSuffix = lineSuffix;
        }
    }
}
