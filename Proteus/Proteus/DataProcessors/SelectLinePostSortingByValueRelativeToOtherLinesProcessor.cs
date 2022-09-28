////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// 
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using LaurentiuCristofor.Proteus.Common;
using LaurentiuCristofor.Proteus.Common.DataHolders;
using LaurentiuCristofor.Proteus.Common.Types;
using LaurentiuCristofor.Proteus.DataExtractors;
using LaurentiuCristofor.Proteus.DataProcessors.Parameters;
using LaurentiuCristofor.Proteus.FileOperations;

namespace LaurentiuCristofor.Proteus.DataProcessors
{
    /// <summary>
    /// A data processor that checks a value's relationship to that of other lines,
    /// to decide whether to output the line or not.
    /// </summary>
    public class SelectLinePostSortingByValueRelativeToOtherLinesProcessor : BaseOutputProcessor, IDataProcessor<OutputOperationParameters<RelativeValueSelectionType>, ParsedLine>
    {
        protected OutputOperationParameters<RelativeValueSelectionType> Parameters { get; set; }

        /// <summary>
        /// Last seen line data.
        /// </summary>
        protected ParsedLine LastSeenLineData { get; set; }

        public void Initialize(OutputOperationParameters<RelativeValueSelectionType> processingParameters)
        {
            this.Parameters = processingParameters;

            this.OutputWriter = new FileWriter(this.Parameters.OutputFilePath);
        }

        public bool Execute(ulong lineNumber, ParsedLine lineData)
        {
            DataProcessorValidation.ValidateExtractedDataIsString(lineData);

            // Verify that the input file is sorted on the extracted data.
            //
            if (this.LastSeenLineData != null
                && lineData.ExtractedData.CompareTo(this.LastSeenLineData.ExtractedData) < 0)
            {
                throw new ProteusException($"Input file is not sorted as expected! Value '{lineData.ExtractedData}' succeeds value '{this.LastSeenLineData.ExtractedData}'.");
            }

            // Determine whether to output the line based on the handling type.
            //
            bool shouldOutputLine = false;
            string lineToOutput = lineData.OriginalLine;
            switch (this.Parameters.OperationType)
            {
                case RelativeValueSelectionType.First:
                    if (this.LastSeenLineData == null
                        || !lineData.ExtractedData.Equals(this.LastSeenLineData.ExtractedData))
                    {
                        shouldOutputLine = true;
                    }
                    break;

                case RelativeValueSelectionType.NotFirst:
                    if (this.LastSeenLineData != null
                        && lineData.ExtractedData.Equals(this.LastSeenLineData.ExtractedData))
                    {
                        shouldOutputLine = true;
                    }
                    break;

                case RelativeValueSelectionType.Last:
                    if (this.LastSeenLineData != null
                        && !lineData.ExtractedData.Equals(this.LastSeenLineData.ExtractedData))
                    {
                        // We're seeing a new value, so the earlier line is the last one in which we saw the previous value.
                        //
                        lineToOutput = this.LastSeenLineData.OriginalLine;
                        shouldOutputLine = true;
                    }
                    break;

                case RelativeValueSelectionType.NotLast:
                    if (this.LastSeenLineData != null
                        && lineData.ExtractedData.Equals(this.LastSeenLineData.ExtractedData))
                    {
                        // We're seeing the value again, so the earlier line is not the last and can be printed now.
                        //
                        lineToOutput = this.LastSeenLineData.OriginalLine;
                        shouldOutputLine = true;
                    }
                    break;

                default:
                    throw new ProteusException($"Internal error: Proteus is not handling relative value selection type '{this.Parameters.OperationType}'!");
            }

            if (shouldOutputLine)
            {
                this.OutputWriter.WriteLine(lineToOutput);
            }

            // Update last seen line data.
            //
            this.LastSeenLineData = lineData;

            return true;
        }

        public override void CompleteExecution()
        {
            if (this.Parameters.OperationType == RelativeValueSelectionType.Last)
            {
                // The last seen line has to be printed.
                //
                if (this.LastSeenLineData != null)
                {
                    this.OutputWriter.WriteLine(this.LastSeenLineData.OriginalLine);
                }
            }

            base.CompleteExecution();
        }
    }
}
