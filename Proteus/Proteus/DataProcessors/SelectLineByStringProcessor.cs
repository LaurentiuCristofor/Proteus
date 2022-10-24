////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
///
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using LaurentiuCristofor.Proteus.Common;
using LaurentiuCristofor.Proteus.Common.Algorithms;
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

            // Validate arguments for the operation.
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

            if (Select(data))
            {
                OutputWriter.WriteLine(lineData.OriginalLine);
            }

            return true;
        }

        /// <summary>
        /// Performs an edit operation of a string using the initializationg parameters.
        /// </summary>
        /// <param name="data">The data to edit.</param>
        /// <param name="lineNumber">The line number of the data. It is produced internally.</param>
        /// <returns></returns>
        protected bool Select(string data)
        {
            switch (SelectionType)
            {
                case StringSelectionType.HasLengthBetween:
                    return data.Length >= FirstCharCount && data.Length <= SecondCharCount;

                case StringSelectionType.HasLengthNotBetween:
                    return data.Length < FirstCharCount || data.Length > SecondCharCount;

                case StringSelectionType.Includes:
                    return data.IndexOf(FirstString) != -1;

                case StringSelectionType.NotIncludes:
                    return data.IndexOf(FirstString) == -1;

                case StringSelectionType.StartsWith:
                    return data.StartsWith(FirstString);

                case StringSelectionType.NotStartsWith:
                    return !data.StartsWith(FirstString);

                case StringSelectionType.EndsWith:
                    return data.EndsWith(FirstString);

                case StringSelectionType.NotEndsWith:
                    return !data.EndsWith(FirstString);

                case StringSelectionType.StartsAndEndsWith:
                    return data.StartsWith(FirstString) && data.EndsWith(SecondString);

                case StringSelectionType.NotStartsAndEndsWith:
                    return !data.StartsWith(FirstString) || !data.EndsWith(SecondString);

                case StringSelectionType.Equals:
                    return data.Equals(FirstString);

                case StringSelectionType.NotEquals:
                    return !data.Equals(FirstString);

                case StringSelectionType.IncludesBefore:
                    return StringOperations.FindMarkers(data, FirstString, SecondString, out _, out _);

                case StringSelectionType.NotIncludesBefore:
                    return !StringOperations.FindMarkers(data, FirstString, SecondString, out _, out _);

                default:
                    throw new ProteusException($"Internal error: Proteus is not handling string selection type '{SelectionType}'!");
            }
        }
    }
}
