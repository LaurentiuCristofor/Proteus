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
    public class TransformColumnsProcessor : BaseOutputProcessor, IDataProcessor<OperationTypeOutputParameters<ColumnTransformationType>, ParsedLine>
    {
        /// <summary>
        /// Parameters of this operation.
        /// </summary>
        protected OperationTypeOutputParameters<ColumnTransformationType> Parameters { get; set; }

        /// <summary>
        /// First line number comparison argument, as an integer value.
        /// </summary>
        protected int FirstArgumentAsInt { get; set; }

        /// <summary>
        /// Second line number comparison argument, as an integer value.
        /// </summary>
        protected int SecondArgumentAsInt { get; set; }

        public void Initialize(OperationTypeOutputParameters<ColumnTransformationType> processingParameters)
        {
            this.Parameters = processingParameters;

            switch (this.Parameters.OperationType)
            {
                case ColumnTransformationType.Pack:
                    this.Parameters.CheckFirstArgumentIsAvailable();
                    this.Parameters.CheckSecondArgumentIsAvailable();
                    this.Parameters.CheckThirdArgumentIsAvailableAndNotEmpty();

                    this.FirstArgumentAsInt = int.Parse(this.Parameters.FirstArgument);
                    this.SecondArgumentAsInt = int.Parse(this.Parameters.SecondArgument);

                    ArgumentChecker.CheckPositive(this.FirstArgumentAsInt);
                    ArgumentChecker.CheckPositive(this.SecondArgumentAsInt);
                    ArgumentChecker.CheckInterval(this.FirstArgumentAsInt, this.SecondArgumentAsInt);
                    break;

                case ColumnTransformationType.Unpack:
                    this.Parameters.CheckFirstArgumentIsAvailable();
                    this.Parameters.CheckSecondArgumentIsAvailableAndNotEmpty();

                    this.FirstArgumentAsInt = int.Parse(this.Parameters.FirstArgument);

                    ArgumentChecker.CheckPositive(this.FirstArgumentAsInt);
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

            string line;
            string[] lineParts = new string[3];

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
                            line = lineData.OriginalLine;
                            break;
                        }
                        else if (endColumnRangeIndex >= countColumns)
                        {
                            endColumnRangeIndex = countColumns - 1;
                        }

                        // Build the line parts - the packed columns and the preceding and succeeding line parts.
                        //
                        lineParts[0] = string.Join(lineData.ColumnSeparator, lineData.Columns, 0, beginColumnRangeIndex);
                        lineParts[1] = string.Join(this.Parameters.ThirdArgument, lineData.Columns, beginColumnRangeIndex, endColumnRangeIndex - beginColumnRangeIndex + 1);
                        lineParts[2] = string.Join(lineData.ColumnSeparator, lineData.Columns, endColumnRangeIndex + 1, countColumns - 1 - endColumnRangeIndex);

                        // Put together all parts.
                        //
                        line = string.Join(lineData.ColumnSeparator, lineParts);
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
                            line = lineData.OriginalLine;
                            break;
                        }

                        // Place packing separator in a string array.
                        //
                        string[] packingColumnSeparators = new string[1];
                        packingColumnSeparators[0] = this.Parameters.SecondArgument;

                        // Unpack column value.
                        //
                        string[] columnParts = lineData.Columns[columnIndex].Split(packingColumnSeparators, StringSplitOptions.None);

                        // Build the line parts - the unpacked columns and the preceding and succeeding line parts.
                        //
                        lineParts[0] = string.Join(lineData.ColumnSeparator, lineData.Columns, 0, columnIndex);
                        lineParts[1] = string.Join(lineData.ColumnSeparator, columnParts);
                        lineParts[2] = string.Join(lineData.ColumnSeparator, lineData.Columns, columnIndex + 1, countColumns - 1 - columnIndex);

                        line = string.Join(lineData.ColumnSeparator, lineParts);
                        break;
                    }

                default:
                    throw new ProteusException($"Internal error: Proteus is not handling column transformation type '{this.Parameters.OperationType}'!");
            }

            this.OutputWriter.WriteLine(line);

            return true;
        }
    }
}
