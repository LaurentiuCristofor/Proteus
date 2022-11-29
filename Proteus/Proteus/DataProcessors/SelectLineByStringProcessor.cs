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
    /// A data processor that checks a string against a selection criterion,
    /// to decide whether to output the line or not.
    ///
    /// OutputExtraOperationParameters is expected to contain:
    /// StringParameters[0] - first string (if required)
    /// StringParameters[1] - second string (if required)
    /// IntParameters[0] - first char count (if required)
    /// IntParameters[1] - second char count (if required)
    /// </summary>
    public class SelectLineByStringProcessor : BaseOutputProcessor, IDataProcessor<OutputExtraOperationParameters<StringSelectionType>, OneExtractedValue>
    {
        protected const int FirstStringIndex = 0;
        protected const int SecondStringIndex = 1;
        protected const int FirstCharCountIndex = 0;
        protected const int SecondCharCountIndex = 1;

        /// <summary>
        /// The type of selection operation.
        /// </summary>
        protected StringSelectionType SelectionType { get; set; }

        /// <summary>
        /// First string argument, if expected.
        /// </summary>
        protected string FirstString { get; set; }

        /// <summary>
        /// Second string argument, if expected.
        /// </summary>
        protected string SecondString { get; set; }

        /// <summary>
        /// First char count argument, if expected.
        /// </summary>
        protected int FirstCharCount { get; set; }

        /// <summary>
        /// Second char count argument, if expected.
        /// </summary>
        protected int SecondCharCount { get; set; }

        public void Initialize(OutputExtraOperationParameters<StringSelectionType> processingParameters)
        {
            SelectionType = processingParameters.OperationType;

            switch (SelectionType)
            {
                case StringSelectionType.HasLengthBetween:
                case StringSelectionType.HasLengthNotBetween:
                    ArgumentChecker.CheckPresence(processingParameters.IntParameters, FirstCharCountIndex);
                    ArgumentChecker.CheckPresence(processingParameters.IntParameters, SecondCharCountIndex);

                    FirstCharCount = processingParameters.IntParameters[FirstCharCountIndex];
                    SecondCharCount = processingParameters.IntParameters[SecondCharCountIndex];

                    ArgumentChecker.CheckGreaterThanOrEqualTo(FirstCharCount, 0);
                    ArgumentChecker.CheckGreaterThanOrEqualTo(SecondCharCount, 0);
                    ArgumentChecker.CheckInterval(FirstCharCount, SecondCharCount);
                    break;

                case StringSelectionType.Includes:
                case StringSelectionType.NotIncludes:
                case StringSelectionType.StartsWith:
                case StringSelectionType.NotStartsWith:
                case StringSelectionType.EndsWith:
                case StringSelectionType.NotEndsWith:
                    ArgumentChecker.CheckPresence(processingParameters.StringParameters, FirstStringIndex);
                    FirstString = processingParameters.StringParameters[FirstStringIndex];
                    ArgumentChecker.CheckNotNullAndNotEmpty(FirstString);
                    break;

                case StringSelectionType.StartsAndEndsWith:
                case StringSelectionType.NotStartsAndEndsWith:
                case StringSelectionType.IncludesBefore:
                case StringSelectionType.NotIncludesBefore:
                    ArgumentChecker.CheckPresence(processingParameters.StringParameters, FirstStringIndex);
                    ArgumentChecker.CheckPresence(processingParameters.StringParameters, SecondStringIndex);

                    FirstString = processingParameters.StringParameters[FirstStringIndex];
                    SecondString = processingParameters.StringParameters[SecondStringIndex];

                    ArgumentChecker.CheckNotNullAndNotEmpty(FirstString);
                    ArgumentChecker.CheckNotNullAndNotEmpty(SecondString);
                    break;

                case StringSelectionType.Equals:
                case StringSelectionType.NotEquals:
                    ArgumentChecker.CheckPresence(processingParameters.StringParameters, FirstStringIndex);
                    FirstString = processingParameters.StringParameters[FirstStringIndex];
                    ArgumentChecker.CheckNotNull(FirstString);
                    break;

                default:
                    throw new ProteusException($"Internal error: Proteus is not handling string selection type '{SelectionType}'!");
            }

            OutputWriter = new FileWriter(processingParameters.OutputFilePath);
        }

        public bool Execute(ulong lineNumber, OneExtractedValue lineData)
        {
            string data = lineData.ExtractedData.ToString();

            if (StringSelection.Select(data, SelectionType, FirstString, SecondString, FirstCharCount, SecondCharCount))
            {
                OutputWriter.WriteLine(lineData.OriginalLine);
            }

            return true;
        }
    }
}
