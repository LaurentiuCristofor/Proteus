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
    public class TransformColumnsProcessor : BaseOutputProcessor, IDataProcessor<OperationOutputParameters<ColumnTransformationType>, ParsedLine>
    {
        protected OperationOutputParameters<ColumnTransformationType> Parameters { get; set; }

        /// <summary>
        /// First argument, as an integer value, if expected.
        /// </summary>
        protected int FirstArgumentAsInt { get; set; }

        /// <summary>
        /// Second argument, as an integer value, if expected.
        /// </summary>
        protected int SecondArgumentAsInt { get; set; }

        /// <summary>
        /// Second argument, in a one-element string array, if expected.
        /// </summary>
        protected string[] SecondArgumentInStringArray { get; set; }

        public void Initialize(OperationOutputParameters<ColumnTransformationType> processingParameters)
        {
            this.Parameters = processingParameters;

            switch (this.Parameters.OperationType)
            {
                case ColumnTransformationType.Pack:
                    ArgumentChecker.CheckNotNull(this.Parameters.FirstArgument);
                    ArgumentChecker.CheckNotNull(this.Parameters.SecondArgument);
                    ArgumentChecker.CheckNotNullAndNotEmpty(this.Parameters.ThirdArgument);

                    this.FirstArgumentAsInt = int.Parse(this.Parameters.FirstArgument);
                    this.SecondArgumentAsInt = int.Parse(this.Parameters.SecondArgument);

                    ArgumentChecker.CheckStrictlyPositive(this.FirstArgumentAsInt);
                    ArgumentChecker.CheckStrictlyPositive(this.SecondArgumentAsInt);
                    ArgumentChecker.CheckInterval(this.FirstArgumentAsInt, this.SecondArgumentAsInt);
                    break;

                case ColumnTransformationType.Unpack:
                    ArgumentChecker.CheckNotNull(this.Parameters.FirstArgument);
                    ArgumentChecker.CheckNotNullAndNotEmpty(this.Parameters.SecondArgument);

                    this.FirstArgumentAsInt = int.Parse(this.Parameters.FirstArgument);
                    this.SecondArgumentInStringArray = new string[1] { this.Parameters.SecondArgument };

                    ArgumentChecker.CheckStrictlyPositive(this.FirstArgumentAsInt);
                    break;

                default:
                    throw new ProteusException($"Internal error: Proteus is not handling column transformation type '{this.Parameters.OperationType}'!");
            }

            this.OutputWriter = new FileWriter(this.Parameters.OutputFilePath);
        }

        public bool Execute(ulong lineNumber, ParsedLine lineData)
        {
            DataProcessorValidation.ValidateLineData(lineData);
            DataProcessorValidation.ValidateColumnInformation(lineData);

            string outputLine;

            int countColumns = lineData.Columns.Length;

            switch (this.Parameters.OperationType)
            {
                case ColumnTransformationType.Pack:
                    {
                        // Columns numbers start from 1 - convert them to indexes in the column array.
                        //
                        int beginColumnRangeIndex = this.FirstArgumentAsInt - 1;
                        int endColumnRangeIndex = this.SecondArgumentAsInt - 1;

                        if (beginColumnRangeIndex >= countColumns)
                        {
                            // Nothing to pack for current line, just output original line.
                            //
                            outputLine = lineData.OriginalLine;
                            break;
                        }
                        else if (endColumnRangeIndex >= countColumns)
                        {
                            endColumnRangeIndex = countColumns - 1;
                        }

                        // Pack the column range.
                        //
                        string packedData = string.Join(this.Parameters.ThirdArgument, lineData.Columns, beginColumnRangeIndex, endColumnRangeIndex - beginColumnRangeIndex + 1);

                        // Assemble the output line.
                        //
                        outputLine = LineAssembler.AssembleWithData(lineData.ColumnSeparator, lineData.Columns, packedData, beginColumnRangeIndex, endColumnRangeIndex);
                        break;
                    }

                case ColumnTransformationType.Unpack:
                    {
                        // Columns numbers start from 1 - convert them to indexes in the column array.
                        //
                        int columnIndex = this.FirstArgumentAsInt - 1;

                        if (columnIndex >= countColumns)
                        {
                            // Nothing to unpack for current line, just output original line.
                            //
                            outputLine = lineData.OriginalLine;
                            break;
                        }

                        // Unpack column value.
                        //
                        string[] unpackedColumns = lineData.Columns[columnIndex].Split(this.SecondArgumentInStringArray, StringSplitOptions.None);
                        string columnData = string.Join(lineData.ColumnSeparator, unpackedColumns);

                        // Assemble the output line.
                        //
                        outputLine = LineAssembler.AssembleWithData(lineData.ColumnSeparator, lineData.Columns, columnData, columnIndex, columnIndex);
                        break;
                    }

                default:
                    throw new ProteusException($"Internal error: Proteus is not handling column transformation type '{this.Parameters.OperationType}'!");
            }

            this.OutputWriter.WriteLine(outputLine);

            return true;
        }
    }
}
