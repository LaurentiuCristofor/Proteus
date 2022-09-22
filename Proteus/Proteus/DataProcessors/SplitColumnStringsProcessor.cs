////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

using LaurentiuCristofor.Proteus.Common;
using LaurentiuCristofor.Proteus.DataExtractors;
using LaurentiuCristofor.Proteus.FileOperations;

namespace LaurentiuCristofor.Proteus.DataProcessors
{
    /// <summary>
    /// A data processor that splits a file into multiple ones based on the value of a specified column.
    /// </summary>
    public class SplitColumnStringsProcessor : BaseOutputProcessor, IDataProcessor<StringAndIntegerOutputParameters, ParsedLine>
    {
        /// <summary>
        /// Parameters of this operation.
        /// </summary>
        protected StringAndIntegerOutputParameters Parameters { get; set; }

        /// <summary>
        /// A dictionary to help us manage the file writers that we will use for each column value.
        /// </summary>
        protected Dictionary<string, FileWriter> MapColumnValueToFileWriter { get; set; }

        public void Initialize(StringAndIntegerOutputParameters processingParameters)
        {
            this.Parameters = processingParameters;

            ArgumentChecker.CheckNotNullAndNotEmpty(this.Parameters.StringValue);
            ArgumentChecker.CheckPositive(this.Parameters.IntegerValue);

            this.MapColumnValueToFileWriter = new Dictionary<string, FileWriter>();
        }

        public bool Execute(ulong lineNumber, ParsedLine lineData)
        {
            DataProcessorValidation.ValidateLineData(lineData);
            DataProcessorValidation.ValidateColumnInformation(lineData);

            // Lines that don't have the column present will be written to a special file.
            // We'll use the OutputWriter to keep track of that.
            //
            if (this.Parameters.IntegerValue > lineData.Columns.Length)
            {
                if (this.OutputWriter == null)
                {
                    this.OutputWriter = new FileWriter(this.Parameters.OutputFilePath + $".0{this.Parameters.StringValue}");
                }

                this.OutputWriter.WriteLine(lineData.OriginalLine);
            }
            else
            {
                string columnValue = lineData.Columns[this.Parameters.IntegerValue - 1];

                // If we don't have a file created for this column already, create one now.
                // File names must be unique and because column values may contain characters that are forbidden in file names,
                // we will instead generate an identifier value for each unique column value.
                //
                if (!this.MapColumnValueToFileWriter.ContainsKey(columnValue))
                {
                    // Assign as identifier, the number of the unique values seen so far, including this one.
                    //
                    int columnValueIdentifier = this.MapColumnValueToFileWriter.Count + 1;

                    this.MapColumnValueToFileWriter.Add(
                        columnValue,
                        new FileWriter(this.Parameters.OutputFilePath + $".{columnValueIdentifier}{this.Parameters.StringValue}"));
                }

                this.MapColumnValueToFileWriter[columnValue].WriteLine(lineData.OriginalLine);
            }

            return true;
        }

        public override void CompleteExecution()
        {
            foreach (string columnValue in this.MapColumnValueToFileWriter.Keys)
            {
                this.MapColumnValueToFileWriter[columnValue].CloseAndReport();
            }

            base.CompleteExecution();
        }
    }
}
