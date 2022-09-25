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
    /// A data processor that splits each column into its own file.
    /// </summary>
    public class SplitColumnsProcessor : BaseOutputProcessor, IDataProcessor<StringOutputParameters, ParsedLine>
    {
        protected StringOutputParameters Parameters { get; set; }

        /// <summary>
        /// A dictionary to help us manage the file writers that we will use for each column.
        /// </summary>
        protected Dictionary<int, FileWriter> MapColumnNumberToFileWriter { get; set; }

        public void Initialize(StringOutputParameters processingParameters)
        {
            this.Parameters = processingParameters;

            ArgumentChecker.CheckNotNullAndNotEmpty(this.Parameters.StringValue);

            this.MapColumnNumberToFileWriter = new Dictionary<int, FileWriter>();
        }

        public bool Execute(ulong lineNumber, ParsedLine lineData)
        {
            DataProcessorValidation.ValidateColumnInformation(lineData);

            // Output each column value as a line in its own file.
            //
            for (int columnIndex = 0; columnIndex < lineData.Columns.Length; ++columnIndex)
            {
                int columnNumber = columnIndex + 1;

                // If we don't have a file created for this column already, create one now.
                //
                if (!this.MapColumnNumberToFileWriter.ContainsKey(columnNumber))
                {
                    this.MapColumnNumberToFileWriter.Add(
                        columnNumber,
                        new FileWriter(this.Parameters.OutputFilePath + $".{columnNumber}{this.Parameters.StringValue}"));
                }

                this.MapColumnNumberToFileWriter[columnNumber].WriteLine(lineData.Columns[columnIndex]);
            }

            return true;
        }

        public override void CompleteExecution()
        {
            foreach (int columnNumber in this.MapColumnNumberToFileWriter.Keys)
            {
                this.MapColumnNumberToFileWriter[columnNumber].CloseAndReport();
            }

            base.CompleteExecution();
        }
    }
}
