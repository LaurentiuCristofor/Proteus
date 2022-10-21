////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
///
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using LaurentiuCristofor.Proteus.Common;
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
    public class SelectLinePostSortingByValueRelativeToOtherLinesProcessor : BaseOutputProcessor, IDataProcessor<OutputOperationParameters<RelativeValueSelectionType>, OneExtractedValue>
    {
        protected RelativeValueSelectionType SelectionType { get; set; }

        /// <summary>
        /// Last seen line data.
        /// </summary>
        protected OneExtractedValue LastSeenLineData { get; set; }

        public void Initialize(OutputOperationParameters<RelativeValueSelectionType> processingParameters)
        {
            SelectionType = processingParameters.OperationType;

            OutputWriter = new FileWriter(processingParameters.OutputFilePath);
        }

        public bool Execute(ulong lineNumber, OneExtractedValue lineData)
        {
            // Verify that the input file is sorted on the extracted data.
            //
            if (LastSeenLineData != null
                && lineData.ExtractedData.CompareTo(LastSeenLineData.ExtractedData) < 0)
            {
                throw new ProteusException($"Input file is not sorted as expected! Value '{lineData.ExtractedData}' succeeds value '{LastSeenLineData.ExtractedData}'.");
            }

            // Determine whether to output the line based on the handling type.
            //
            bool shouldOutputLine = false;
            string lineToOutput = lineData.OriginalLine;
            switch (SelectionType)
            {
                case RelativeValueSelectionType.First:
                    if (LastSeenLineData == null
                        || !lineData.ExtractedData.Equals(LastSeenLineData.ExtractedData))
                    {
                        shouldOutputLine = true;
                    }
                    break;

                case RelativeValueSelectionType.NotFirst:
                    if (LastSeenLineData != null
                        && lineData.ExtractedData.Equals(LastSeenLineData.ExtractedData))
                    {
                        shouldOutputLine = true;
                    }
                    break;

                case RelativeValueSelectionType.Last:
                    if (LastSeenLineData != null
                        && !lineData.ExtractedData.Equals(LastSeenLineData.ExtractedData))
                    {
                        // We're seeing a new value, so the earlier line is the last one in which we saw the previous value.
                        //
                        lineToOutput = LastSeenLineData.OriginalLine;
                        shouldOutputLine = true;
                    }
                    break;

                case RelativeValueSelectionType.NotLast:
                    if (LastSeenLineData != null
                        && lineData.ExtractedData.Equals(LastSeenLineData.ExtractedData))
                    {
                        // We're seeing the value again, so the earlier line is not the last one and can be printed now.
                        //
                        lineToOutput = LastSeenLineData.OriginalLine;
                        shouldOutputLine = true;
                    }
                    break;

                default:
                    throw new ProteusException($"Internal error: Proteus is not handling relative value selection type '{SelectionType}'!");
            }

            if (shouldOutputLine)
            {
                OutputWriter.WriteLine(lineToOutput);
            }

            // Update last seen line data.
            //
            LastSeenLineData = lineData;

            return true;
        }

        public override void CompleteExecution()
        {
            if (SelectionType == RelativeValueSelectionType.Last)
            {
                // The last seen line has to be printed.
                //
                if (LastSeenLineData != null)
                {
                    OutputWriter.WriteLine(LastSeenLineData.OriginalLine);
                }
            }

            base.CompleteExecution();
        }
    }
}
