////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;

using LaurentiuCristofor.Proteus.Common;
using LaurentiuCristofor.Proteus.DataExtractors;
using LaurentiuCristofor.Proteus.FileOperations;

namespace LaurentiuCristofor.Proteus.DataProcessors
{
    /// <summary>
    /// A data processor that selects a subset of columns to output.
    /// </summary>
    public class ColumnNumberSelectProcessor : BaseOutputProcessor, IDataProcessor<OperationTypeParameters<NumberSelectionType>, StringParts>
    {
        /// <summary>
        /// Parameters of this operation.
        /// </summary>
        protected OperationTypeParameters<NumberSelectionType> Parameters { get; set; }

        /// <summary>
        /// First line number comparison argument, as an integer value.
        /// </summary>
        protected int FirstArgumentAsInt { get; set; }

        /// <summary>
        /// Second line number comparison argument, as an integer value.
        /// </summary>
        protected int SecondArgumentAsInt { get; set; }

        public void Initialize(OperationTypeParameters<NumberSelectionType> processingParameters)
        {
            this.Parameters = processingParameters;

            this.OutputWriter = new TextFileWriter(this.Parameters.OutputFilePath);

            switch (this.Parameters.OperationType)
            {
                case NumberSelectionType.Last:
                case NumberSelectionType.NotLast:
                case NumberSelectionType.Each:
                case NumberSelectionType.NotEach:
                    ArgumentChecker.CheckPresence(this.Parameters.FirstArgument);

                    this.FirstArgumentAsInt = int.Parse(this.Parameters.FirstArgument);

                    ArgumentChecker.CheckPositive(this.FirstArgumentAsInt);
                    break;

                case NumberSelectionType.Between:
                case NumberSelectionType.NotBetween:
                    ArgumentChecker.CheckPresence(this.Parameters.FirstArgument);
                    ArgumentChecker.CheckPresence(this.Parameters.SecondArgument);

                    this.FirstArgumentAsInt = int.Parse(this.Parameters.FirstArgument);
                    this.SecondArgumentAsInt = int.Parse(this.Parameters.SecondArgument);

                    ArgumentChecker.CheckPositive(this.FirstArgumentAsInt);
                    ArgumentChecker.CheckPositive(this.SecondArgumentAsInt);
                    ArgumentChecker.CheckSmallerOrEqual(this.FirstArgumentAsInt, this.SecondArgumentAsInt);
                    break;

                default:
                    throw new ProteusException($"Internal error: Proteus is not handling number selection type '{this.Parameters.OperationType}'!");
            }
        }

        public bool Execute(ulong lineNumber, StringParts inputData)
        {
            // We may not always be able to extract a column.
            // Ignore these cases; the extractor will already have printed a warning message.
            //
            if (inputData == null)
            {
                return true;
            }

            if (String.IsNullOrEmpty(inputData.OriginalStringSeparator))
            {
                throw new ProteusException("Internal error: ColumnRangeSelectProcessor was called without a separator value!");
            }

            string line = String.Empty;

            int countColumns = inputData.OriginalStringParts.Length;

            switch (this.Parameters.OperationType)
            {
                case NumberSelectionType.Between:
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
                        line = string.Join(inputData.OriginalStringSeparator, inputData.OriginalStringParts, beginColumnRangeIndex, endColumnRangeIndex - beginColumnRangeIndex + 1);
                        break;
                    }

                case NumberSelectionType.NotBetween:
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
                            line = inputData.OriginalString;
                        }
                        else if (endColumnRangeIndex >= countColumns - 1)
                        {
                            // Only the first columns of this line fall outside the range.
                            //
                            line = string.Join(inputData.OriginalStringSeparator, inputData.OriginalStringParts, 0, beginColumnRangeIndex);
                        }
                        else
                        {
                            string linePrefix = string.Join(inputData.OriginalStringSeparator, inputData.OriginalStringParts, 0, beginColumnRangeIndex);
                            string lineSuffix = string.Join(inputData.OriginalStringSeparator, inputData.OriginalStringParts, endColumnRangeIndex + 1, countColumns - endColumnRangeIndex - 1);
                            if (String.IsNullOrEmpty(linePrefix))
                            {
                                line = lineSuffix;
                            }
                            else
                            {
                                line = linePrefix + inputData.OriginalStringSeparator + lineSuffix;
                            }

                        }
                        break;
                    }

                case NumberSelectionType.Last:
                    {
                        int countLast = this.FirstArgumentAsInt;

                        if (countLast >= countColumns)
                        {
                            // If line has fewer columns than requested, return them all.
                            //
                            line = inputData.OriginalString;
                        }
                        else
                        {
                            // Otherwise, concatenate only the last requested columns.
                            //
                            line = string.Join(inputData.OriginalStringSeparator, inputData.OriginalStringParts, countColumns - countLast, countLast);
                        }

                        break;
                    }

                case NumberSelectionType.NotLast:
                    {
                        int countLast = this.FirstArgumentAsInt;

                        // If there are more columns than those that need to be removed, output them.
                        // Otherwise, the line will be removed entirely.
                        //
                        if (countColumns > countLast)
                        {
                            line = string.Join(inputData.OriginalStringSeparator, inputData.OriginalStringParts, 0, countColumns - countLast);
                        }

                        break;
                    }

                case NumberSelectionType.Each:
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
                                    line += inputData.OriginalStringSeparator;
                                }

                                line += inputData.OriginalStringParts[columnIndex];
                            }
                        }

                        break;
                    }

                case NumberSelectionType.NotEach:
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
                                    line += inputData.OriginalStringSeparator;
                                }

                                line += inputData.OriginalStringParts[columnIndex];
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
