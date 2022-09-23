////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using LaurentiuCristofor.Proteus.Common;
using LaurentiuCristofor.Proteus.DataExtractors;
using LaurentiuCristofor.Proteus.FileOperations;

namespace LaurentiuCristofor.Proteus.DataProcessors.Dual
{
    /// <summary>
    /// A data processor that selects lines from a first file based on whether they match a line from a second file.
    /// </summary>
    public class LookupProcessor: BaseOutputProcessor, IDualDataProcessor<OperationOutputParameters<LookupType>, ParsedLine>
    {
        protected OperationOutputParameters<LookupType> Parameters { get; set; }

        public void Initialize(OperationOutputParameters<LookupType> processingParameters)
        {
            this.Parameters = processingParameters;

            this.OutputWriter = new FileWriter(this.Parameters.OutputFilePath);
        }

        public ProcessingActionType Execute(
            bool hasProcessedFirstFile, ulong firstLineNumber, ParsedLine firstLineData,
            bool hasProcessedSecondFile, ulong secondLineNumber, ParsedLine secondLineData)
        {
            if (hasProcessedFirstFile)
            {
                // We're done. No need to continue processing the second file.
                //
                return ProcessingActionType.Terminate;
            }
            else if (hasProcessedSecondFile)
            {
                // None of the remaining lines in the first file will be able to match anything in the second file.
                // If we're writing them out (if ProcessLine() returns true), then continue writing them all;
                // otherwise, terminate processing.
                //
                if (ProcessLine(firstLineData.OriginalLine, this.Parameters.OperationType, isIncluded: false))
                {
                    return ProcessingActionType.AdvanceFirst;
                }
                else
                {
                    return ProcessingActionType.Terminate;
                }
            }

            // Compare the extracted data from both lines.
            //
            int comparison = firstLineData.ExtractedData.CompareTo(secondLineData.ExtractedData);
            if (comparison < 0)
            {
                // The line did not match. Process it and move to the next one.
                //
                ProcessLine(firstLineData.OriginalLine, this.Parameters.OperationType, isIncluded: false);

                return ProcessingActionType.AdvanceFirst;
            }
            else if (comparison == 0)
            {
                // The line matched. Process it and move to the next one.
                //
                ProcessLine(firstLineData.OriginalLine, this.Parameters.OperationType, isIncluded: true);

                return ProcessingActionType.AdvanceFirst;
            }
            else // if (comparison > 0)
            {
                // The line did not match the line from the second file, but it may still match a subsequent line,
                // hence move to the next line in the second file and retry the comparison.
                //
                return ProcessingActionType.AdvanceSecond;
            }
        }

        private bool ProcessLine(string line, LookupType lookupType, bool isIncluded)
        {
            bool shouldOutputLine;
            switch (lookupType)
            {
                case LookupType.Included:
                    shouldOutputLine = isIncluded;
                    break;

                case LookupType.NotIncluded:
                    shouldOutputLine = !isIncluded;
                    break;

                default:
                    throw new ProteusException($"Internal error: Proteus is not handling lookup type '{this.Parameters.OperationType}'!");
            }

            if (shouldOutputLine)
            {
                this.OutputWriter.WriteLine(line);
            }

            return shouldOutputLine;
        }
    }
}
