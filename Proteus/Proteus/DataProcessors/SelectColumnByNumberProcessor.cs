////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// 
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;

using LaurentiuCristofor.Proteus.Common;
using LaurentiuCristofor.Proteus.Common.Types;
using LaurentiuCristofor.Proteus.DataExtractors;
using LaurentiuCristofor.Proteus.DataProcessors.Parameters;
using LaurentiuCristofor.Proteus.FileOperations;

namespace LaurentiuCristofor.Proteus.DataProcessors
{
    /// <summary>
    /// A data processor that selects a subset of columns to output.
    /// </summary>
    public class SelectColumnByNumberProcessor : BaseOutputProcessor, IDataProcessor<OutputOperationParameters<PositionSelectionType>, ExtractedColumnStrings>
    {
        protected OutputOperationParameters<PositionSelectionType> Parameters { get; set; }

        /// <summary>
        /// First line number comparison argument, as an integer value.
        /// </summary>
        protected int FirstArgumentAsInt { get; set; }

        /// <summary>
        /// Second line number comparison argument, as an integer value.
        /// </summary>
        protected int SecondArgumentAsInt { get; set; }

        public void Initialize(OutputOperationParameters<PositionSelectionType> processingParameters)
        {
            this.Parameters = processingParameters;

            switch (this.Parameters.OperationType)
            {
                case PositionSelectionType.Last:
                case PositionSelectionType.NotLast:
                case PositionSelectionType.Each:
                case PositionSelectionType.NotEach:
                    ArgumentChecker.CheckNotNull(this.Parameters.FirstArgument);

                    this.FirstArgumentAsInt = int.Parse(this.Parameters.FirstArgument);

                    ArgumentChecker.CheckStrictlyPositive(this.FirstArgumentAsInt);
                    break;

                case PositionSelectionType.Between:
                case PositionSelectionType.NotBetween:
                    ArgumentChecker.CheckNotNull(this.Parameters.FirstArgument);
                    ArgumentChecker.CheckNotNull(this.Parameters.SecondArgument);

                    this.FirstArgumentAsInt = int.Parse(this.Parameters.FirstArgument);
                    this.SecondArgumentAsInt = int.Parse(this.Parameters.SecondArgument);

                    ArgumentChecker.CheckStrictlyPositive(this.FirstArgumentAsInt);
                    ArgumentChecker.CheckStrictlyPositive(this.SecondArgumentAsInt);
                    ArgumentChecker.CheckInterval(this.FirstArgumentAsInt, this.SecondArgumentAsInt);
                    break;

                default:
                    throw new ProteusException($"Internal error: Proteus is not handling number selection type '{this.Parameters.OperationType}'!");
            }

            this.OutputWriter = new FileWriter(this.Parameters.OutputFilePath);
        }

        public bool Execute(ulong lineNumber, ExtractedColumnStrings lineData)
        {
            string line = String.Empty;

            int countColumns = lineData.Columns.Length;

            switch (this.Parameters.OperationType)
            {
                case PositionSelectionType.Between:
                    {
                        // Columns numbers start from 1 - convert them to indexes in the column array.
                        //
                        int beginColumnRangeIndex = this.FirstArgumentAsInt - 1;
                        int endColumnRangeIndex = this.SecondArgumentAsInt - 1;

                        if (beginColumnRangeIndex >= countColumns)
                        {
                            // Nothing to output for this line.
                            //
                            return true;
                        }
                        else if (endColumnRangeIndex >= countColumns)
                        {
                            endColumnRangeIndex = countColumns - 1;
                        }

                        // Extract the columns that fall within the requested range.
                        //
                        line = string.Join(lineData.ColumnSeparator, lineData.Columns, beginColumnRangeIndex, endColumnRangeIndex - beginColumnRangeIndex + 1);
                        break;
                    }

                case PositionSelectionType.NotBetween:
                    {
                        // Columns numbers start from 1 - convert them to indexes in the column array.
                        //
                        int beginColumnRangeIndex = this.FirstArgumentAsInt - 1;
                        int endColumnRangeIndex = this.SecondArgumentAsInt - 1;

                        if (beginColumnRangeIndex >= countColumns)
                        {
                            // The columns in this line fall outside the specified column range,
                            // so we can output the entire line.
                            //
                            line = lineData.OriginalLine;
                        }
                        else if (endColumnRangeIndex >= countColumns - 1)
                        {
                            // Only the first columns of this line fall outside the range.
                            //
                            line = string.Join(lineData.ColumnSeparator, lineData.Columns, 0, beginColumnRangeIndex);
                        }
                        else
                        {
                            string linePrefix = string.Join(lineData.ColumnSeparator, lineData.Columns, 0, beginColumnRangeIndex);
                            string lineSuffix = string.Join(lineData.ColumnSeparator, lineData.Columns, endColumnRangeIndex + 1, countColumns - endColumnRangeIndex - 1);
                            if (String.IsNullOrEmpty(linePrefix))
                            {
                                line = lineSuffix;
                            }
                            else
                            {
                                line = linePrefix + lineData.ColumnSeparator + lineSuffix;
                            }
                        }
                        break;
                    }

                case PositionSelectionType.Last:
                    {
                        int countLast = this.FirstArgumentAsInt;

                        if (countLast >= countColumns)
                        {
                            // If line has fewer columns than requested, return them all.
                            //
                            line = lineData.OriginalLine;
                        }
                        else
                        {
                            // Otherwise, concatenate only the last requested columns.
                            //
                            line = string.Join(lineData.ColumnSeparator, lineData.Columns, countColumns - countLast, countLast);
                        }
                        break;
                    }

                case PositionSelectionType.NotLast:
                    {
                        int countLast = this.FirstArgumentAsInt;

                        // If there are more columns than those that need to be removed, output them.
                        // Otherwise, the line will be removed entirely.
                        //
                        if (countColumns > countLast)
                        {
                            line = string.Join(lineData.ColumnSeparator, lineData.Columns, 0, countColumns - countLast);
                        }
                        break;
                    }

                case PositionSelectionType.Each:
                    {
                        int countEach = this.FirstArgumentAsInt;

                        // Iterate over all columns and output each Nth one.
                        //
                        for (int columnIndex = 0; columnIndex < countColumns; ++columnIndex)
                        {
                            if ((columnIndex + 1) % countEach == 0)
                            {
                                // Add the separator before all columns but the first.
                                //
                                if (!String.IsNullOrEmpty(line))
                                {
                                    line += lineData.ColumnSeparator;
                                }

                                line += lineData.Columns[columnIndex];
                            }
                        }
                        break;
                    }

                case PositionSelectionType.NotEach:
                    {
                        int countEach = this.FirstArgumentAsInt;

                        // Iterate over all columns and output each one that isn't an Nth one.
                        //
                        for (int columnIndex = 0; columnIndex < countColumns; ++columnIndex)
                        {
                            if ((columnIndex + 1) % countEach > 0)
                            {
                                // Add the separator before all columns but the first.
                                //
                                if (!String.IsNullOrEmpty(line))
                                {
                                    line += lineData.ColumnSeparator;
                                }

                                line += lineData.Columns[columnIndex];
                            }
                        }
                        break;
                    }

                default:
                    throw new ProteusException($"Internal error: Proteus is not handling number selection type '{this.Parameters.OperationType}'!");
            }

            this.OutputWriter.WriteLine(line);

            return true;
        }
    }
}
