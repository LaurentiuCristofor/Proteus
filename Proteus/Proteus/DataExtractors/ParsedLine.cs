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
        /// </summary>
        public DataTypeContainer ExtractedData { get; protected set;}

        // The string prefix that preceded the extracted string.
        //
        public string LinePrefix { get; protected set; }

        // The string suffix that followed the extracted string.
        public string LineSuffix { get; protected set; }

        public ParsedLine(string originalLine, string columnSeparator, string[] columns, DataTypeContainer extractedData, string linePrefix, string lineSuffix)
        {
            this.OriginalLine = originalLine;
            this.ColumnSeparator = columnSeparator;
            this.Columns = columns;
            this.ExtractedData = extractedData;
            this.LinePrefix = linePrefix;
            this.LineSuffix = lineSuffix;
        }
    }
}
