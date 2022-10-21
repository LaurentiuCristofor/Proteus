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
    ///
    /// OutputExtraOperationParameters is expected to contain:
    /// IntParameters[0] - first column count
    /// IntParameters[1] - second column count (if required)
    /// </summary>
    public class SelectColumnByNumberProcessor : BaseOutputProcessor, IDataProcessor<OutputExtraOperationParameters<PositionSelectionType>, ExtractedColumnStrings>
    {
        protected const int FirstColumnCountIndex = 0;
        protected const int SecondColumnCountIndex = 1;

        protected PositionSelectionType SelectionType { get; set; }

        /// <summary>
        /// First column count argument, if expected.
        /// </summary>
        protected int FirstColumnCount { get; set; }

        /// <summary>
        /// Second column count argument, if expected.
        /// </summary>
        protected int SecondColumnCount { get; set; }

        public void Initialize(OutputExtraOperationParameters<PositionSelectionType> processingParameters)
        {
            this.SelectionType = processingParameters.OperationType;

            switch (this.SelectionType)
            {
                case PositionSelectionType.Last:
                case PositionSelectionType.NotLast:
                case PositionSelectionType.Each:
                case PositionSelectionType.NotEach:
                    ArgumentChecker.CheckPresence(processingParameters.IntParameters, FirstColumnCountIndex);
                    this.FirstColumnCount = processingParameters.IntParameters[FirstColumnCountIndex];
                    ArgumentChecker.CheckGreaterThanOrEqualTo(this.FirstColumnCount, 1);
                    break;

                case PositionSelectionType.Between:
                case PositionSelectionType.NotBetween:
                    ArgumentChecker.CheckPresence(processingParameters.IntParameters, FirstColumnCountIndex);
                    ArgumentChecker.CheckPresence(processingParameters.IntParameters, SecondColumnCountIndex);

                    this.FirstColumnCount = processingParameters.IntParameters[FirstColumnCountIndex];
                    this.SecondColumnCount = processingParameters.IntParameters[SecondColumnCountIndex];

                    ArgumentChecker.CheckGreaterThanOrEqualTo(this.FirstColumnCount, 1);
                    ArgumentChecker.CheckGreaterThanOrEqualTo(this.SecondColumnCount, 1);
                    ArgumentChecker.CheckInterval(this.FirstColumnCount, this.SecondColumnCount);
                    break;

                default:
                    throw new ProteusException($"Internal error: Proteus is not handling position selection type '{this.SelectionType}'!");
            }

            this.OutputWriter = new FileWriter(processingParameters.OutputFilePath);
        }

        public bool Execute(ulong lineNumber, ExtractedColumnStrings lineData)
        {
            string line = String.Empty;

            int countColumns = lineData.Columns.Length;

            switch (this.SelectionType)
            {
                case PositionSelectionType.Between:
                    {
                        // Columns numbers start from 1 - convert them to indexes in the column array.
                        //
                        int beginColumnRangeIndex = this.FirstColumnCount - 1;
                        int endColumnRangeIndex = this.SecondColumnCount - 1;

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
                        int beginColumnRangeIndex = this.FirstColumnCount - 1;
                        int endColumnRangeIndex = this.SecondColumnCount - 1;

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
                        int countLast = this.FirstColumnCount;

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
                        int countLast = this.FirstColumnCount;

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
                        int countEach = this.FirstColumnCount;

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
                        int countEach = this.FirstColumnCount;

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
                    throw new ProteusException($"Internal error: Proteus is not handling position selection type '{this.SelectionType}'!");
            }

            this.OutputWriter.WriteLine(line);

            return true;
        }
    }
}
