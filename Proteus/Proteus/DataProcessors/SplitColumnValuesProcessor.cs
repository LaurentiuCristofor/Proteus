////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
///
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;

using LaurentiuCristofor.Proteus.Common;
using LaurentiuCristofor.Proteus.Common.DataHolders;
using LaurentiuCristofor.Proteus.DataExtractors;
using LaurentiuCristofor.Proteus.DataProcessors.Parameters;
using LaurentiuCristofor.Proteus.FileOperations;

namespace LaurentiuCristofor.Proteus.DataProcessors
{
    /// <summary>
    /// A data processor that splits a file into multiple ones based on the value of a specified column.
    /// 
    /// OutputParameters is expected to contain:
    /// StringParameters[0] - output file extension
    /// </summary>
    public class SplitColumnValuesProcessor : BaseOutputProcessor, IDataProcessor<OutputParameters, OneExtractedValue>
    {
        protected OutputParameters Parameters { get; set; }

        /// <summary>
        /// The file extension that should be used for the output files.
        /// </summary>
        protected string OutputFileExtension { get; set; }

        /// <summary>
        /// A dictionary to help us manage the file writers that we will use for each column value.
        /// </summary>
        protected Dictionary<IDataHolder, FileWriter> MapColumnValueToFileWriter { get; set; }

        public void Initialize(OutputParameters processingParameters)
        {
            this.Parameters = processingParameters;

            ArgumentChecker.CheckPresence<string>(this.Parameters.StringParameters, 0);
            ArgumentChecker.CheckNotNullAndNotEmpty(this.Parameters.StringParameters[0]);
            this.OutputFileExtension = this.Parameters.StringParameters[0];

            this.MapColumnValueToFileWriter = new Dictionary<IDataHolder, FileWriter>();
        }

        public bool Execute(ulong lineNumber, OneExtractedValue lineData)
        {
            IDataHolder data = lineData.ExtractedData;

            // If we don't have a file created for this column already, create one now.
            //
            // File names must be unique and because column values may contain characters that are forbidden in file names,
            // we will instead generate an identifier value for each unique column value.
            //
            if (!this.MapColumnValueToFileWriter.ContainsKey(data))
            {
                // Assign as identifier the number of the unique values seen so far, including this one.
                //
                int columnValueIdentifier = this.MapColumnValueToFileWriter.Count + 1;

                this.MapColumnValueToFileWriter.Add(
                    data,
                    new FileWriter(this.Parameters.OutputFilePath + $".{columnValueIdentifier}{this.OutputFileExtension}"));
            }

            this.MapColumnValueToFileWriter[data].WriteLine(lineData.OriginalLine);

            return true;
        }

        public override void CompleteExecution()
        {
            foreach (IDataHolder columnValue in this.MapColumnValueToFileWriter.Keys)
            {
                this.MapColumnValueToFileWriter[columnValue].CloseAndReport();
            }

            base.CompleteExecution();
        }
    }
}
