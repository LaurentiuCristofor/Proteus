﻿////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
///
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;

using LaurentiuCristofor.Proteus.Common;
using LaurentiuCristofor.Proteus.Common.DataHolders;
using LaurentiuCristofor.Proteus.Common.Types;
using LaurentiuCristofor.Proteus.DataExtractors;
using LaurentiuCristofor.Proteus.DataProcessors.Parameters;
using LaurentiuCristofor.Proteus.FileOperations;

namespace LaurentiuCristofor.Proteus.DataProcessors.Dual
{
    /// <summary>
    /// A data processor that joins lines from two files based on the values of two columns.
    /// 
    /// OutputExtraOperationParameters is expected to contain:
    /// StringParameters[0] - outer join default value (if required)
    /// </summary>
    public class JoinPostSortingProcessor: BaseOutputProcessor, IDualDataProcessor<OutputExtraOperationParameters<JoinType>, OneExtractedValue>
    {
        protected const int OuterJoinDefaultValueIndex = 0;

        protected JoinType JoinType { get; set; }

        /// <summary>
        /// The default value to use for LeftOuterJoin, when there is no matching row "on the right".
        /// </summary>
        protected string OuterJoinDefaultValue { get; set; }

        /// <summary>
        /// The last key we matched on.
        /// </summary>
        protected IDataHolder LastMatchedKey { get; set; }

        /// <summary>
        /// The lines from the second file that should be joined on the last matched key value.
        /// </summary>
        protected List<string> LinesToJoinOnLastMatchedKey { get; set; }

        public void Initialize(OutputExtraOperationParameters<JoinType> processingParameters)
        {
            JoinType = processingParameters.OperationType;

            if (JoinType == JoinType.LeftOuter)
            {
                ArgumentChecker.CheckPresence(processingParameters.StringParameters, OuterJoinDefaultValueIndex);
                OuterJoinDefaultValue = processingParameters.StringParameters[OuterJoinDefaultValueIndex];
                ArgumentChecker.CheckNotNull(OuterJoinDefaultValue);
            }

            LinesToJoinOnLastMatchedKey = new List<string>();

            OutputWriter = new FileWriter(processingParameters.OutputFilePath);
        }

        public ProcessingActionType Execute(
            bool hasProcessedFirstFile, ulong firstLineNumber, OneExtractedValue firstLineData,
            bool hasProcessedSecondFile, ulong secondLineNumber, OneExtractedValue secondLineData)
        {
            if (hasProcessedFirstFile)
            {
                // We're done. No need to continue processing the second file.
                //
                return ProcessingActionType.Terminate;
            }
            else if (hasProcessedSecondFile)
            {
                // We can complete processing the second file while collecting lines to join with the first.
                // Check if the line from the first file can be joined with the last matched key.
                //
                if (firstLineData.ExtractedData.Equals(LastMatchedKey))
                {
                    JoinLineAndOutput(firstLineData);
                    return ProcessingActionType.AdvanceFirst;
                }

                // None of the remaining lines in the first file will be able to match anything in the second file.
                // Proceed according to the type of join.
                //
                switch (JoinType)
                {
                    case JoinType.Inner:
                        // We don't output anything for an inner join.
                        //
                        return ProcessingActionType.Terminate;

                    case JoinType.LeftOuter:
                        // For a left outer join, we'll output the first line combined with the string provided by the user.
                        //
                        OutputWriter.WriteLine(firstLineData.OriginalLine + firstLineData.ColumnSeparator + OuterJoinDefaultValue);
                        return ProcessingActionType.AdvanceFirst;

                    default:
                        throw new ProteusException($"Internal error: Proteus is not handling join type '{JoinType}'!");
                }
            }

            // Initialize LastMatchedKey.
            //
            if (LastMatchedKey == null)
            {
                LastMatchedKey = secondLineData.ExtractedData;
            }

            // Compare the extracted data from both lines.
            //
            int comparison = firstLineData.ExtractedData.CompareTo(secondLineData.ExtractedData);
            if (comparison == 0)
            {
                // If the key has changed, reset the information about it.
                //
                if (!secondLineData.ExtractedData.Equals(LastMatchedKey))
                {
                    LastMatchedKey = secondLineData.ExtractedData;
                    LinesToJoinOnLastMatchedKey.Clear();
                }

                // The line matched.
                // We now want to iterate over the second file and collect all the lines we should join with.
                // We will stop on the first row with a different (greater) value.
                // Hence, processing will continue in the "< 0" branch.
                //
                string lineToJoin = secondLineData.AssembleWithoutColumn(secondLineData.ExtractedColumnNumber);
                LinesToJoinOnLastMatchedKey.Add(lineToJoin);

                return ProcessingActionType.AdvanceSecond;
            }
            else if (comparison < 0)
            {
                // Check if we should join this line.
                //
                if (firstLineData.ExtractedData.Equals(LastMatchedKey))
                {
                    JoinLineAndOutput(firstLineData);
                }

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

        /// <summary>
        /// Joins the line from the first file with all the join lines we collected from the second file,
        /// and outputs each one of them.
        /// </summary>
        /// <param name="firstLineData">The data from the first file.</param>
        protected void JoinLineAndOutput(OneExtractedValue firstLineData)
        {
            foreach (string joinLine in LinesToJoinOnLastMatchedKey)
            {
                // Special case: if there is no join line (the line in the join file only contained the join key and no other columns),
                // then we will still output the current line as is.
                //
                string outputLine = firstLineData.OriginalLine;

                // Join the line with our line.
                //
                if (joinLine != null)
                {
                    outputLine += firstLineData.ColumnSeparator + joinLine;
                }

                OutputWriter.WriteLine(outputLine);
            }
        }
    }
}
