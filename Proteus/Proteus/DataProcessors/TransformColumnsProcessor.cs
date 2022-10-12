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
    /// A data processor that transforms columns.
    /// </summary>
    public class TransformColumnsProcessor : BaseOutputProcessor, IDataProcessor<OutputOperationParameters<ColumnTransformationType>, ExtractedColumnStrings>
    {
        protected ColumnTransformationType TransformationType { get; set; }

        /// <summary>
        /// First column number argument, if expected.
        /// </summary>
        protected int FirstColumnNumber { get; set; }

        /// <summary>
        /// Second column number, if expected.
        /// </summary>
        protected int SecondColumnNumber { get; set; }

        /// <summary>
        /// Packing string separator.
        /// </summary>
        protected string PackingSeparator { get; set; }

        /// <summary>
        /// Unpacking string separator, in a one-element string array, if expected.
        /// </summary>
        protected string[] UnpackingSeparators { get; set; }

        public void Initialize(OutputOperationParameters<ColumnTransformationType> processingParameters)
        {
            this.TransformationType = processingParameters.OperationType;

            switch (this.TransformationType)
            {
                case ColumnTransformationType.Pack:
                    ArgumentChecker.CheckNotNull<string>(processingParameters.FirstArgument);
                    ArgumentChecker.CheckNotNull<string>(processingParameters.SecondArgument);
                    ArgumentChecker.CheckNotNullAndNotEmpty(processingParameters.ThirdArgument);

                    this.FirstColumnNumber = int.Parse(processingParameters.FirstArgument);
                    this.SecondColumnNumber = int.Parse(processingParameters.SecondArgument);
                    this.PackingSeparator = processingParameters.ThirdArgument;

                    ArgumentChecker.CheckStrictlyPositive(this.FirstColumnNumber);
                    ArgumentChecker.CheckStrictlyPositive(this.SecondColumnNumber);
                    ArgumentChecker.CheckInterval<int>(this.FirstColumnNumber, this.SecondColumnNumber);
                    break;

                case ColumnTransformationType.Unpack:
                    ArgumentChecker.CheckNotNull<string>(processingParameters.FirstArgument);
                    ArgumentChecker.CheckNotNullAndNotEmpty(processingParameters.SecondArgument);

                    this.FirstColumnNumber = int.Parse(processingParameters.FirstArgument);
                    this.UnpackingSeparators = new string[] { processingParameters.SecondArgument };

                    ArgumentChecker.CheckStrictlyPositive(this.FirstColumnNumber);
                    break;

                default:
                    throw new ProteusException($"Internal error: Proteus is not handling column transformation type '{this.TransformationType}'!");
            }

            this.OutputWriter = new FileWriter(processingParameters.OutputFilePath);
        }

        public bool Execute(ulong lineNumber, ExtractedColumnStrings lineData)
        {
            string outputLine;

            int countColumns = lineData.Columns.Length;

            switch (this.TransformationType)
            {
                case ColumnTransformationType.Pack:
                    {
                        int startColumnNumber = this.FirstColumnNumber;
                        int endColumnNumber = this.SecondColumnNumber;

                        string packedData = null;
                        if (startColumnNumber <= countColumns)
                        {
                            if (endColumnNumber > countColumns)
                            {
                                endColumnNumber = countColumns;
                            }

                            // Pack the column range.
                            //
                            packedData = string.Join(this.PackingSeparator, lineData.Columns, startColumnNumber - 1, endColumnNumber - startColumnNumber + 1);
                        }

                        // Assemble the output line.
                        //
                        outputLine = lineData.AssembleWithColumnRangeReplacement(startColumnNumber, endColumnNumber, packedData);
                        break;
                    }

                case ColumnTransformationType.Unpack:
                    {
                        int columnNumber = this.FirstColumnNumber;

                        string columnData = null;
                        if (columnNumber <= countColumns)
                        {
                            // Unpack column value.
                            //
                            string[] unpackedColumns = lineData.Columns[columnNumber - 1].Split(this.UnpackingSeparators, StringSplitOptions.None);
                            columnData = string.Join(lineData.ColumnSeparator, unpackedColumns);
                        }

                        // Assemble the output line.
                        //
                        outputLine = lineData.AssembleWithColumnRangeReplacement(columnNumber, columnNumber, columnData);
                        break;
                    }

                default:
                    throw new ProteusException($"Internal error: Proteus is not handling column transformation type '{this.TransformationType}'!");
            }

            this.OutputWriter.WriteLine(outputLine);

            return true;
        }
    }
}
