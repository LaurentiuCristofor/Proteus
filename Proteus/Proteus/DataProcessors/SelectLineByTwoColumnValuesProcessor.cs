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
    /// A data processor that checks the value of a column against the value of a second column,
    /// to decide whether to output the line or not.
    /// </summary>
    public class SelectLineByTwoColumnValuesProcessor : BaseOutputProcessor, IDataProcessor<OutputOperationParameters<ComparisonType>, TwoExtractedValues>
    {
        protected ComparisonType ComparisonType { get; set; }

        public void Initialize(OutputOperationParameters<ComparisonType> processingParameters)
        {
            ComparisonType = processingParameters.OperationType;

            switch (ComparisonType)
            {
                // We only support one-threshold comparisons, because the second column is a single threshold.
                //
                case ComparisonType.LessThan:
                case ComparisonType.LessThanOrEqual:
                case ComparisonType.Equal:
                case ComparisonType.GreaterThanOrEqual:
                case ComparisonType.GreaterThan:
                case ComparisonType.NotEqual:
                    break;

                default:
                    throw new ProteusException($"Internal error: Proteus is not handling comparison type '{ComparisonType}'!");
            }

            OutputWriter = new FileWriter(processingParameters.OutputFilePath);
        }

        public bool Execute(ulong lineNumber, TwoExtractedValues lineData)
        {
            // Perform the comparison of the two column values to decide whether to output the line.
            //
            if (DataHolderOperations.Compare(lineData.ExtractedData, ComparisonType, lineData.SecondExtractedData, null))
            {
                OutputWriter.WriteLine(lineData.OriginalLine);
            }

            return true;
        }
    }
}
