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
    ///
    /// OutputExtraOperationParameters is expected to contain:
    /// StringParameters[0] - packing/unpacking separator
    /// IntParameters[0] - first column number
    /// IntParameters[1] - second column number (if required)
    /// </summary>
    public class TransformColumnsProcessor : BaseOutputProcessor, IDataProcessor<OutputExtraOperationParameters<ColumnTransformationType>, ExtractedColumnStrings>
    {
        protected const int SeparatorIndex = 0;
        protected const int FirstColumnNumberIndex = 0;
        protected const int SecondColumnNumberIndex = 1;

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

        public void Initialize(OutputExtraOperationParameters<ColumnTransformationType> processingParameters)
        {
            TransformationType = processingParameters.OperationType;

            switch (TransformationType)
            {
                case ColumnTransformationType.Pack:
                    ArgumentChecker.CheckPresence(processingParameters.IntParameters, FirstColumnNumberIndex);
                    ArgumentChecker.CheckPresence(processingParameters.IntParameters, SecondColumnNumberIndex);
                    ArgumentChecker.CheckPresence(processingParameters.StringParameters, SeparatorIndex);

                    FirstColumnNumber = processingParameters.IntParameters[FirstColumnNumberIndex];
                    SecondColumnNumber = processingParameters.IntParameters[SecondColumnNumberIndex];
                    PackingSeparator = processingParameters.StringParameters[SeparatorIndex];

                    ArgumentChecker.CheckGreaterThanOrEqualTo(FirstColumnNumber, 1);
                    ArgumentChecker.CheckGreaterThanOrEqualTo(SecondColumnNumber, 1);
                    ArgumentChecker.CheckNotNull(PackingSeparator);
                    ArgumentChecker.CheckInterval(FirstColumnNumber, SecondColumnNumber);
                    break;

                case ColumnTransformationType.Unpack:
                    ArgumentChecker.CheckPresence(processingParameters.IntParameters, FirstColumnNumberIndex);
                    ArgumentChecker.CheckPresence(processingParameters.StringParameters, SeparatorIndex);

                    FirstColumnNumber = processingParameters.IntParameters[FirstColumnNumberIndex];
                    UnpackingSeparators = new string[] { processingParameters.StringParameters[SeparatorIndex] };

                    ArgumentChecker.CheckGreaterThanOrEqualTo(FirstColumnNumber, 1);
                    ArgumentChecker.CheckNotNullAndNotEmpty(UnpackingSeparators[0]);
                    break;

                default:
                    throw new ProteusException($"Internal error: Proteus is not handling column transformation type '{TransformationType}'!");
            }

            OutputWriter = new FileWriter(processingParameters.OutputFilePath);
        }

        public bool Execute(ulong lineNumber, ExtractedColumnStrings lineData)
        {
            string outputLine;

            int countColumns = lineData.Columns.Length;

            switch (TransformationType)
            {
                case ColumnTransformationType.Pack:
                    {
                        int startColumnNumber = FirstColumnNumber;
                        int endColumnNumber = SecondColumnNumber;

                        string packedData = null;
                        if (startColumnNumber <= countColumns)
                        {
                            if (endColumnNumber > countColumns)
                            {
                                endColumnNumber = countColumns;
                            }

                            // Pack the column range.
                            //
                            packedData = string.Join(PackingSeparator, lineData.Columns, startColumnNumber - 1, endColumnNumber - startColumnNumber + 1);
                        }

                        // Assemble the output line.
                        //
                        outputLine = lineData.AssembleWithColumnRangeReplacement(startColumnNumber, endColumnNumber, packedData);
                        break;
                    }

                case ColumnTransformationType.Unpack:
                    {
                        int columnNumber = FirstColumnNumber;

                        string columnData = null;
                        if (columnNumber <= countColumns)
                        {
                            // Unpack column value.
                            //
                            string[] unpackedColumns = lineData.Columns[columnNumber - 1].Split(UnpackingSeparators, StringSplitOptions.None);
                            columnData = string.Join(lineData.ColumnSeparator, unpackedColumns);
                        }

                        // Assemble the output line.
                        //
                        outputLine = lineData.AssembleWithColumnRangeReplacement(columnNumber, columnNumber, columnData);
                        break;
                    }

                default:
                    throw new ProteusException($"Internal error: Proteus is not handling column transformation type '{TransformationType}'!");
            }

            OutputWriter.WriteLine(outputLine);

            return true;
        }
    }
}
