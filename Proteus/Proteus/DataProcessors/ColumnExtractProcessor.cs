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
    /// A data processor that extracts columns.
    /// </summary>
    public class ColumnExtractProcessor : BaseOutputProcessor, IDataProcessor<StringParameters, ParsedLine>
    {
        /// <summary>
        /// Parameters of this operation.
        /// </summary>
        protected StringParameters Parameters { get; set; }

        /// <summary>
        /// A dictionary to help us manage the file writers that we will use for each column.
        /// </summary>
        protected Dictionary<int, TextFileWriter> MapColumnNumberToFileWriter { get; set; }

        public void Initialize(StringParameters processingParameters)
        {
            this.Parameters = processingParameters;

            this.MapColumnNumberToFileWriter = new Dictionary<int, TextFileWriter>();
        }

        public bool Execute(ulong lineNumber, ParsedLine lineData)
        {
            // We may not always be able to extract a column.
            // Ignore these cases; the extractor will already have printed a warning message.
            //
            if (lineData == null)
            {
                return true;
            }

            if (String.IsNullOrEmpty(lineData.ColumnSeparator))
            {
                throw new ProteusException("ColumnExtractProcessor was called without a column separator value!");
            }

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
                        new TextFileWriter(this.Parameters.OutputFilePath + $".{columnNumber}{this.Parameters.StringValue}"));
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
