////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;

using LaurentiuCristofor.Proteus.Common;
using LaurentiuCristofor.Proteus.DataExtractors;
using LaurentiuCristofor.Proteus.FileOperations;

namespace LaurentiuCristofor.Proteus.DataProcessors
{
    /// <summary>
    /// A data processor that checks a string to see if it's a repetition of a previously processed value,
    /// to decide whether to output the line or not.
    /// </summary>
    public class SelectLinePostSortingHandlingRepeteadStringsProcessor : BaseOutputProcessor, IDataProcessor<OperationTypeOutputParameters<RepetitionHandlingType>, ParsedLine>
    {
        /// <summary>
        /// Parameters of this operation.
        /// </summary>
        protected OperationTypeOutputParameters<RepetitionHandlingType> Parameters { get; set; }

        /// <summary>
        /// Last seen data.
        /// </summary>
        protected string LastSeenData { get; set; }

        public void Initialize(OperationTypeOutputParameters<RepetitionHandlingType> processingParameters)
        {
            this.Parameters = processingParameters;

            this.OutputWriter = new FileWriter(this.Parameters.OutputFilePath);
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

            DataProcessorValidation.ValidateExtractedDataIsString(lineData);

            string data = lineData.ExtractedData.ToString();
            bool isRepeatedData = false;

            // Verify that the input file is sorted on the extracted data.
            //
            if (data.CompareTo(this.LastSeenData) < 0)
            {
                throw new ProteusException($"Input file is not sorted as expected! Value '{data}' succeeds value '{this.LastSeenData}'.");
            }

            // Compare data with our last seen data;
            //
            if (data.Equals(this.LastSeenData))
            {
                isRepeatedData = true;
            }
            else
            {
                this.LastSeenData = data;
            }

            // Determine whether to output the line based on the handling type.
            //
            bool shouldOutputLine;
            switch (this.Parameters.OperationType)
            {
                case RepetitionHandlingType.Pick:
                    shouldOutputLine = isRepeatedData;
                    break;

                case RepetitionHandlingType.Skip:
                    shouldOutputLine = !isRepeatedData;
                    break;

                default:
                    throw new ProteusException($"Internal error: Proteus is not handling repetition handling type '{this.Parameters.OperationType}'!");
            }

            if (shouldOutputLine)
            {
                this.OutputWriter.WriteLine(lineData.OriginalLine);
            }

            return true;
        }
    }
}
