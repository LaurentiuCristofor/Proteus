////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
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
    public class TransformColumnsProcessor : BaseOutputProcessor, IDataProcessor<OutputOperationParameters<ColumnTransformationType>, ParsedLine>
    {
        protected OutputOperationParameters<ColumnTransformationType> Parameters { get; set; }

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

        public void Initialize(OutputOperationParameters<ColumnTransformationType> processingParameters)
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
            DataProcessorValidation.ValidateColumnInformation(lineData);

            string outputLine;

            int countColumns = lineData.Columns.Length;

            switch (this.Parameters.OperationType)
            {
                case ColumnTransformationType.Pack:
                    {
                        int startColumnNumber = this.FirstArgumentAsInt;
                        int endColumnNumber = this.SecondArgumentAsInt;

                        string packedData = null;
                        if (startColumnNumber <= countColumns)
                        {
                            if (endColumnNumber > countColumns)
                            {
                                endColumnNumber = countColumns;
                            }

                            // Pack the column range.
                            //
                            packedData = string.Join(this.Parameters.ThirdArgument, lineData.Columns, startColumnNumber - 1, endColumnNumber - startColumnNumber + 1);
                        }

                        // Assemble the output line.
                        //
                        outputLine = lineData.AssembleWithColumnRangeReplacement(startColumnNumber, endColumnNumber, packedData);
                        break;
                    }

                case ColumnTransformationType.Unpack:
                    {
                        int columnNumber = this.FirstArgumentAsInt;

                        string columnData = null;
                        if (columnNumber <= countColumns)
                        {
                            // Unpack column value.
                            //
                            string[] unpackedColumns = lineData.Columns[columnNumber - 1].Split(this.SecondArgumentInStringArray, StringSplitOptions.None);
                            columnData = string.Join(lineData.ColumnSeparator, unpackedColumns);
                        }

                        // Assemble the output line.
                        //
                        outputLine = lineData.AssembleWithColumnRangeReplacement(columnNumber, columnNumber, columnData);
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
