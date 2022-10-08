////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
///
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using LaurentiuCristofor.Proteus.Common;

namespace LaurentiuCristofor.Proteus.DataExtractors
{
    /// <summary>
    /// A packaging of a line and its column strings.
    /// </summary>
    public class ExtractedColumnStrings
    {
        // The original line, unchanged.
        //
        public string OriginalLine { get; protected set; }

        // The column strings.
        //
        public string[] Columns { get; protected set; }

        // The column separator.
        //
        public string ColumnSeparator { get; protected set; }

        public ExtractedColumnStrings(
            string originalLine,
            string[] columns,
            string columnSeparator)
        {
            this.OriginalLine = originalLine;
            this.Columns = columns;
            this.ColumnSeparator = columnSeparator;
        }

        /// <summary>
        /// Builds a line, excluding one column.
        /// </summary>
        /// <param name="columnNumber">The column number to omit.</param>
        /// <returns>The new line, or null if the original line only consisted of the removed column.</returns>
        public string AssembleWithoutColumn(int columnNumber)
        {
            if (columnNumber > this.Columns.Length)
            {
                return string.Join(this.ColumnSeparator, this.Columns);
            }
            else if (columnNumber == 1 && columnNumber == this.Columns.Length)
            {
                return null;
            }

            // Simply build a new column array, ommitting the entry for our column,
            // and then build the line from it.
            //
            string[] newColumns = new string[this.Columns.Length - 1];
            int columnIndex = columnNumber - 1;

            for (int readIndex = 0, writeIndex = 0; readIndex < this.Columns.Length; ++readIndex)
            {
                if (readIndex == columnIndex)
                {
                    continue;
                }

                newColumns[writeIndex++] = this.Columns[readIndex];
            }

            return string.Join(this.ColumnSeparator, newColumns);
        }

        /// <summary>
        /// Builds a line, using the specified data instead of a range of columns.
        /// </summary>
        /// <param name="replacementData">The data to use instead of a range of columns.</param>
        /// <param name="startColumnNumber">The starting column number of the range to replace.</param>
        /// <param name="endColumnNumber">The end column number of the range to replace.</param>
        /// <returns>The new line, or null if the range does not fit within the column array.</returns>
        public string AssembleWithColumnRangeReplacement(int startColumnNumber, int endColumnNumber, string replacementData)
        {
            ArgumentChecker.CheckInterval<int>(startColumnNumber, endColumnNumber);

            if (startColumnNumber > this.Columns.Length)
            {
                return string.Join(this.ColumnSeparator, this.Columns);
            }
            else if (startColumnNumber == 1 && endColumnNumber == this.Columns.Length)
            {
                return replacementData;
            }
            else if (endColumnNumber > this.Columns.Length)
            {
                return null;
            }

            // Build a new array of columns, replacing a range of columns with a specific data.
            //
            string[] newColumns = new string[this.Columns.Length - endColumnNumber + startColumnNumber];

            int startColumnIndex = startColumnNumber - 1;
            int endColumnIndex = endColumnNumber - 1;
            for (int readIndex = 0, writeIndex = 0; readIndex < this.Columns.Length; ++readIndex)
            {
                if (readIndex == startColumnIndex)
                {
                    // Found the start of the range.
                    // Write our replacement data instead of the first range column data.
                    //
                    newColumns[writeIndex++] = replacementData;
                    continue;
                }
                else if (readIndex > startColumnIndex && readIndex <= endColumnIndex)
                {
                    // Skip all other columns in the range.
                    //
                    continue;
                }

                // Copy data for all other columns outside the range.
                //
                newColumns[writeIndex++] = this.Columns[readIndex];
            }

            return string.Join(this.ColumnSeparator, newColumns);
        }
    }
}
