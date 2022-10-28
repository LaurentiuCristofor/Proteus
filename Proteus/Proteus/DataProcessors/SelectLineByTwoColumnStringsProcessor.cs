////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
///
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using LaurentiuCristofor.Proteus.Common;
using LaurentiuCristofor.Proteus.Common.Utilities;
using LaurentiuCristofor.Proteus.Common.Types;
using LaurentiuCristofor.Proteus.DataExtractors;
using LaurentiuCristofor.Proteus.DataProcessors.Parameters;
using LaurentiuCristofor.Proteus.FileOperations;

namespace LaurentiuCristofor.Proteus.DataProcessors
{
    /// <summary>
    /// A data processor that checks a column's string value against another column's string value,
    /// to decide whether to output the line or not.
    /// </summary>
    public class SelectLineByTwoColumnStringsProcessor : BaseOutputProcessor, IDataProcessor<OutputOperationParameters<StringSelectionType>, TwoExtractedValues>
    {
        /// <summary>
        /// The type of selection operation.
        /// </summary>
        protected StringSelectionType SelectionType { get; set; }

        public void Initialize(OutputOperationParameters<StringSelectionType> processingParameters)
        {
            SelectionType = processingParameters.OperationType;

            switch (SelectionType)
            {
                // We only support selections that use a single string argument,
                // because we can only compare against the second column string.
                //
                case StringSelectionType.Includes:
                case StringSelectionType.NotIncludes:
                case StringSelectionType.StartsWith:
                case StringSelectionType.NotStartsWith:
                case StringSelectionType.EndsWith:
                case StringSelectionType.NotEndsWith:
                case StringSelectionType.Equals:
                case StringSelectionType.NotEquals:
                    break;

                default:
                    throw new ProteusException($"Internal error: Proteus is not handling string selection type '{SelectionType}'!");
            }

            OutputWriter = new FileWriter(processingParameters.OutputFilePath);
        }

        public bool Execute(ulong lineNumber, TwoExtractedValues lineData)
        {
            string firstColumnString = lineData.ExtractedData.ToString();
            string secondColumnString = lineData.SecondExtractedData.ToString();

            if (StringSelection.Select(firstColumnString, SelectionType, secondColumnString))
            {
                OutputWriter.WriteLine(lineData.OriginalLine);
            }

            return true;
        }
    }
}
