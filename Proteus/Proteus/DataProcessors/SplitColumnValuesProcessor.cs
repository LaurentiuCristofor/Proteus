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
    /// OutputExtraParameters is expected to contain:
    /// StringParameters[0] - output file extension
    /// </summary>
    public class SplitColumnValuesProcessor : BaseOutputProcessor, IDataProcessor<OutputExtraParameters, OneExtractedValue>
    {
        protected const int OutputFileExtensionIndex = 0;

        protected string OutputFilePath { get; set; }

        /// <summary>
        /// The file extension that should be used for the output files.
        /// </summary>
        protected string OutputFileExtension { get; set; }

        /// <summary>
        /// A dictionary to help us manage the file writers that we will use for each column value.
        /// </summary>
        protected Dictionary<IDataHolder, FileWriter> MapColumnValueToFileWriter { get; set; }

        public void Initialize(OutputExtraParameters processingParameters)
        {
            OutputFilePath = processingParameters.OutputFilePath;

            ArgumentChecker.CheckPresence(processingParameters.StringParameters, OutputFileExtensionIndex);
            OutputFileExtension = processingParameters.StringParameters[OutputFileExtensionIndex];
            ArgumentChecker.CheckNotNullAndNotEmpty(OutputFileExtension);

            MapColumnValueToFileWriter = new Dictionary<IDataHolder, FileWriter>();
        }

        public bool Execute(ulong lineNumber, OneExtractedValue lineData)
        {
            IDataHolder data = lineData.ExtractedData;

            // If we don't have a file created for this column already, create one now.
            //
            // File names must be unique and because column values may contain characters that are forbidden in file names,
            // we will instead generate an identifier value for each unique column value.
            //
            if (!MapColumnValueToFileWriter.ContainsKey(data))
            {
                // Assign as identifier the number of the unique values seen so far, including this one.
                //
                int columnValueIdentifier = MapColumnValueToFileWriter.Count + 1;

                MapColumnValueToFileWriter.Add(
                    data,
                    new FileWriter(OutputFilePath + $".{columnValueIdentifier}{OutputFileExtension}"));
            }

            MapColumnValueToFileWriter[data].WriteLine(lineData.OriginalLine);

            return true;
        }

        public override void CompleteExecution()
        {
            foreach (IDataHolder columnValue in MapColumnValueToFileWriter.Keys)
            {
                MapColumnValueToFileWriter[columnValue].CloseAndReport();
            }

            base.CompleteExecution();
        }
    }
}
