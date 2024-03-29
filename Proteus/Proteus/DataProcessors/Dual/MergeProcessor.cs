﻿////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
///
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using LaurentiuCristofor.Proteus.DataExtractors;
using LaurentiuCristofor.Proteus.DataProcessors.Parameters;
using LaurentiuCristofor.Proteus.FileOperations;

namespace LaurentiuCristofor.Proteus.DataProcessors.Dual
{
    /// <summary>
    /// A data processor that merges lines from two sorted files.
    /// </summary>
    public class MergeProcessor: BaseOutputProcessor, IDualDataProcessor<BaseOutputParameters, OneExtractedValue>
    {
        public void Initialize(BaseOutputParameters processingParameters)
        {
            OutputWriter = new FileWriter(processingParameters.OutputFilePath);
        }

        public ProcessingActionType Execute(
            bool hasProcessedFirstFile, ulong firstLineNumber, OneExtractedValue firstLineData,
            bool hasProcessedSecondFile, ulong secondLineNumber, OneExtractedValue secondLineData)
        {
            if (hasProcessedFirstFile)
            {
                if (hasProcessedSecondFile)
                {
                    // If we finished processing both files, we're done.
                    //
                    return ProcessingActionType.Terminate;
                }

                // If we finished with the first file, just output the remaining lines from the second one.
                //
                OutputWriter.WriteLine(secondLineData.OriginalLine);

                return ProcessingActionType.AdvanceSecond;
            }
            else if (hasProcessedSecondFile)
            {
                // If we finished with the second file, just output the remaining lines from the first one.
                //
                OutputWriter.WriteLine(firstLineData.OriginalLine);

                return ProcessingActionType.AdvanceFirst;
            }

            // Compare the extracted data from both lines, to decide which one to output first.
            //
            if (firstLineData.ExtractedData.CompareTo(secondLineData.ExtractedData) <= 0)
            {
                OutputWriter.WriteLine(firstLineData.OriginalLine);

                return ProcessingActionType.AdvanceFirst;
            }
            else
            {
                OutputWriter.WriteLine(secondLineData.OriginalLine);

                return ProcessingActionType.AdvanceSecond;
            }
        }
    }
}
