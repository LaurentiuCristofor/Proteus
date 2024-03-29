﻿////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
///
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;

using LaurentiuCristofor.Proteus.Common;
using LaurentiuCristofor.Proteus.DataExtractors;
using LaurentiuCristofor.Proteus.DataProcessors.Parameters;
using LaurentiuCristofor.Proteus.FileOperations;

namespace LaurentiuCristofor.Proteus.DataProcessors
{
    /// <summary>
    /// A data processor that splits each column into its own file.
    /// 
    /// OutputExtraParameters is expected to contain:
    /// StringParameters[0] - output file extension
    /// </summary>
    public class SplitColumnsProcessor : BaseOutputProcessor, IDataProcessor<OutputExtraParameters, ExtractedColumnStrings>
    {
        protected const int OutputFileExtensionIndex = 0;

        protected string OutputFilePath { get; set; }

        /// <summary>
        /// The file extension that should be used for the output files.
        /// </summary>
        protected string OutputFileExtension { get; set; }

        /// <summary>
        /// A dictionary to help us manage the file writers that we will use for each column.
        /// </summary>
        protected Dictionary<int, FileWriter> MapColumnNumberToFileWriter { get; set; }

        public void Initialize(OutputExtraParameters processingParameters)
        {
            OutputFilePath = processingParameters.OutputFilePath;

            ArgumentChecker.CheckPresence(processingParameters.StringParameters, OutputFileExtensionIndex);
            OutputFileExtension = processingParameters.StringParameters[OutputFileExtensionIndex];
            ArgumentChecker.CheckNotNullAndNotEmpty(OutputFileExtension);

            MapColumnNumberToFileWriter = new Dictionary<int, FileWriter>();
        }

        public bool Execute(ulong lineNumber, ExtractedColumnStrings lineData)
        {
            // Output each column value as a line in its own file.
            //
            for (int columnIndex = 0; columnIndex < lineData.Columns.Length; ++columnIndex)
            {
                int columnNumber = columnIndex + 1;

                // If we don't have a file created for this column already, create one now.
                //
                if (!MapColumnNumberToFileWriter.ContainsKey(columnNumber))
                {
                    MapColumnNumberToFileWriter.Add(
                        columnNumber,
                        new FileWriter(OutputFilePath + $".{columnNumber}{OutputFileExtension}"));
                }

                MapColumnNumberToFileWriter[columnNumber].WriteLine(lineData.Columns[columnIndex]);
            }

            return true;
        }

        public override void CompleteExecution()
        {
            foreach (int columnNumber in MapColumnNumberToFileWriter.Keys)
            {
                MapColumnNumberToFileWriter[columnNumber].CloseAndReport();
            }

            base.CompleteExecution();
        }
    }
}
