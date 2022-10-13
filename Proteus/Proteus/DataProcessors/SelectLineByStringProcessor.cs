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
    /// A data processor that checks a string against a selection criterion,
    /// to decide whether to output the line or not.
    ///
    /// OutputExtraOperationParameters is expected to contain:
    /// StringParameters[0] - first string (optional)
    /// StringParameters[1] - second string (optional)
    /// IntParameters[0] - first char count (optional)
    /// IntParameters[1] - second char count (optional)
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
            this.SelectionType = processingParameters.OperationType;

            // Validate arguments for the operation.
            switch (this.SelectionType)
            {
                case StringSelectionType.HasLengthBetween:
                case StringSelectionType.HasLengthNotBetween:
                    ArgumentChecker.CheckPresence(processingParameters.IntParameters, FirstCharCountIndex);
                    ArgumentChecker.CheckPresence(processingParameters.IntParameters, SecondCharCountIndex);

                    this.FirstCharCount = processingParameters.IntParameters[FirstCharCountIndex];
                    this.SecondCharCount = processingParameters.IntParameters[SecondCharCountIndex];

                    ArgumentChecker.CheckGreaterThanOrEqualTo(this.FirstCharCount, 0);
                    ArgumentChecker.CheckGreaterThanOrEqualTo(this.SecondCharCount, 0);
                    ArgumentChecker.CheckInterval(this.FirstCharCount, this.SecondCharCount);
                    break;

                case StringSelectionType.Includes:
                case StringSelectionType.NotIncludes:
                case StringSelectionType.StartsWith:
                case StringSelectionType.NotStartsWith:
                case StringSelectionType.EndsWith:
                case StringSelectionType.NotEndsWith:
                    ArgumentChecker.CheckPresence(processingParameters.StringParameters, FirstStringIndex);
                    this.FirstString = processingParameters.StringParameters[FirstStringIndex];
                    ArgumentChecker.CheckNotNullAndNotEmpty(this.FirstString);
                    break;

                case StringSelectionType.StartsAndEndsWith:
                case StringSelectionType.NotStartsAndEndsWith:
                    ArgumentChecker.CheckPresence(processingParameters.StringParameters, FirstStringIndex);
                    ArgumentChecker.CheckPresence(processingParameters.StringParameters, SecondStringIndex);

                    this.FirstString = processingParameters.StringParameters[FirstStringIndex];
                    this.SecondString = processingParameters.StringParameters[SecondStringIndex];

                    ArgumentChecker.CheckNotNullAndNotEmpty(this.FirstString);
                    ArgumentChecker.CheckNotNullAndNotEmpty(this.SecondString);
                    break;

                case StringSelectionType.Equals:
                case StringSelectionType.NotEquals:
                    ArgumentChecker.CheckPresence(processingParameters.StringParameters, FirstStringIndex);
                    this.FirstString = processingParameters.StringParameters[FirstStringIndex];
                    ArgumentChecker.CheckNotNull(this.FirstString);
                    break;

                default:
                    throw new ProteusException($"Internal error: Proteus is not handling string selection type '{this.SelectionType}'!");
            }

            this.OutputWriter = new FileWriter(processingParameters.OutputFilePath);
        }

        public bool Execute(ulong lineNumber, OneExtractedValue lineData)
        {
            string data = lineData.ExtractedData.ToString();

            if (this.Select(data))
            {
                this.OutputWriter.WriteLine(lineData.OriginalLine);
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
            switch (this.SelectionType)
            {
                case StringSelectionType.HasLengthBetween:
                    return data.Length >= this.FirstCharCount && data.Length <= this.SecondCharCount;

                case StringSelectionType.HasLengthNotBetween:
                    return data.Length < this.FirstCharCount || data.Length > this.SecondCharCount;

                case StringSelectionType.Includes:
                    return data.IndexOf(this.FirstString) != -1;

                case StringSelectionType.NotIncludes:
                    return data.IndexOf(this.FirstString) == -1;

                case StringSelectionType.StartsWith:
                    return data.StartsWith(this.FirstString);

                case StringSelectionType.NotStartsWith:
                    return !data.StartsWith(this.FirstString);

                case StringSelectionType.EndsWith:
                    return data.EndsWith(this.FirstString);

                case StringSelectionType.NotEndsWith:
                    return !data.EndsWith(this.FirstString);

                case StringSelectionType.StartsAndEndsWith:
                    return data.StartsWith(this.FirstString) && data.EndsWith(this.SecondString);

                case StringSelectionType.NotStartsAndEndsWith:
                    return !data.StartsWith(this.FirstString) || !data.EndsWith(this.SecondString);

                case StringSelectionType.Equals:
                    return data.Equals(this.FirstString);

                case StringSelectionType.NotEquals:
                    return !data.Equals(this.FirstString);

                default:
                    throw new ProteusException($"Internal error: Proteus is not handling string selection type '{this.SelectionType}'!");
            }
        }
    }
}
