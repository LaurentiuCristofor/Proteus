////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;

namespace LaurentiuCristofor.Proteus.Common
{
    /// <summary>
    /// A class that helps assemble lines.
    /// </summary>
    public abstract class LineAssembler
    {
        /// <summary>
        /// Builds a line from a column array, excluding one column.
        /// </summary>
        /// <param name="columnSeparator">The column separator.</param>
        /// <param name="columns">The column array.</param>
        /// <param name="columnNumber">The column number to omit.</param>
        /// <returns>The new line, or null if the original line only consisted of the removed column.</returns>
        public static string AssembleWithoutColumn(string columnSeparator, string[] columns, int columnNumber)
        {
            if (columnNumber > columns.Length)
            {
                return string.Join(columnSeparator, columns);
            }
            else if (columnNumber == 1 && columns.Length == columnNumber)
            {
                return null;
            }

            // Simply build a new column array, ommitting the entry for our column,
            // and then build the line from it.
            //
            string[] newColumns = new string[columns.Length - 1];
            int columnIndex = columnNumber - 1;

            for (int readIndex = 0, writeIndex = 0; readIndex < columns.Length; ++readIndex)
            {
                if (readIndex == columnIndex)
                {
                    continue;
                }

                newColumns[writeIndex++] = columns[readIndex];
            }

            return string.Join(columnSeparator, newColumns);
        }

        /// <summary>
        /// Build a line from column array, using the specified data instead of a range of columns.
        /// 
        /// This is marked internal, because it relies on the caller to ensure that the range is valid and fits in the column array.
        /// </summary>
        /// <param name="columnSeparator">The column separator.</param>
        /// <param name="columns">The column array.</param>
        /// <param name="data">The data to use instead of a range of columns.</param>
        /// <param name="startColumnIndex">The starting column index of the range to replace.</param>
        /// <param name="endColumnIndex">The end column index of the range to replace.</param>
        /// <returns></returns>
        internal static string AssembleWithData(string columnSeparator, string[] columns, string data, int startColumnIndex, int endColumnIndex)
        {
            if (startColumnIndex == 0 && endColumnIndex == columns.Length - 1)
            {
                return data;
            }

            string[] newColumns = new string[columns.Length - endColumnIndex + startColumnIndex];

            for (int readIndex = 0, writeIndex = 0; readIndex < columns.Length; ++readIndex)
            {
                if (readIndex == startColumnIndex)
                {
                    newColumns[writeIndex++] = data;
                    continue;
                }
                else if (readIndex > startColumnIndex && readIndex <= endColumnIndex)
                {
                    continue;
                }

                newColumns[writeIndex++] = columns[readIndex];
            }

            return string.Join(columnSeparator, newColumns);
        }
    }
}
