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
    /// A packaging of the line parts that result from the extraction of a column.
    /// </summary>
    public class LineParts
    {
        // The original line, unchanged.
        //
        public string OriginalLine { get; set; }

        // The line data before the column.
        //
        public string DataBeforeColumn { get; set; }

        // The line data after the column.
        public string DataAfterColumn { get; set; }

        public LineParts(string originalLine, string dataBeforeColumn, string dataAfterColumn)
        {
            this.OriginalLine = originalLine;
            this.DataBeforeColumn = dataBeforeColumn;
            this.DataAfterColumn = dataAfterColumn;
        }
    }
}
