////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// 
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using LaurentiuCristofor.Proteus.Common.Types;

namespace LaurentiuCristofor.Proteus.Common.Utilities
{
    /// <summary>
    /// A class that performs specific string selection operations.
    /// </summary>
    public class StringSelector
    {
        /// <summary>
        /// The type of selection operation.
        /// </summary>
        protected StringSelectionType SelectionType { get; set; }

        /// <summary>
        /// First operation argument.
        /// </summary>
        protected string FirstArgument { get; set; }

        /// <summary>
        /// Second operation argument.
        /// </summary>
        protected string SecondArgument { get; set; }

        /// <summary>
        /// First operation argument, as an integer value (set only if the operation expects an integer argument).
        /// </summary>
        protected int FirstArgumentAsInt { get; set; }

        /// <summary>
        /// Second operation argument, as an integer value (set only if the operation expects an integer argument).
        /// </summary>
        protected int SecondArgumentAsInt { get; set; }

        /// <summary>
        /// Initializes the selection operation parameters.
        /// </summary>
        /// <param name="selectionType">Type of operation.</param>
        /// <param name="firstArgument">First operation argument.</param>
        /// <param name="secondArgument">Second operation argument.</param>
        public void Initialize(StringSelectionType selectionType, string firstArgument, string secondArgument)
        {
            this.SelectionType = selectionType;
            this.FirstArgument = firstArgument;
            this.SecondArgument = secondArgument;

            // Validate arguments for the operation.
            switch (selectionType)
            {
                case StringSelectionType.Includes:
                case StringSelectionType.NotIncludes:
                case StringSelectionType.StartsWith:
                case StringSelectionType.NotStartsWith:
                case StringSelectionType.EndsWith:
                case StringSelectionType.NotEndsWith:
                case StringSelectionType.Equals:
                case StringSelectionType.NotEquals:
                    ArgumentChecker.CheckNotNullAndNotEmpty(this.FirstArgument);
                    break;

                case StringSelectionType.StartsAndEndsWith:
                case StringSelectionType.NotStartsAndEndsWith:
                    ArgumentChecker.CheckNotNullAndNotEmpty(this.FirstArgument);
                    ArgumentChecker.CheckNotNullAndNotEmpty(this.SecondArgument);
                    break;

                case StringSelectionType.HasLengthBetween:
                case StringSelectionType.HasLengthNotBetween:
                    ArgumentChecker.CheckNotNull(this.FirstArgument);
                    ArgumentChecker.CheckNotNull(this.SecondArgument);

                    this.FirstArgumentAsInt = int.Parse(this.FirstArgument);
                    this.SecondArgumentAsInt = int.Parse(this.SecondArgument);

                    ArgumentChecker.CheckPositive(this.FirstArgumentAsInt);
                    ArgumentChecker.CheckPositive(this.SecondArgumentAsInt);
                    ArgumentChecker.CheckInterval(this.FirstArgumentAsInt, this.SecondArgumentAsInt);
                    break;

                default:
                    throw new ProteusException($"Internal error: Proteus is not handling string selection type '{this.SelectionType}'!");
            }
        }

        /// <summary>
        /// Perform an edit operation of a string using the initializationg parameters.
        /// </summary>
        /// <param name="data">The data to edit.</param>
        /// <param name="lineNumber">The line number of the data. It is produced internally.</param>
        /// <returns></returns>
        public bool Select(string data)
        {
            if (data == null)
            {
                throw new ProteusException("StringSelector has been called on null data!");
            }

            switch (this.SelectionType)
            {
                case StringSelectionType.HasLengthBetween:
                    return data.Length >= this.FirstArgumentAsInt && data.Length <= this.SecondArgumentAsInt;

                case StringSelectionType.HasLengthNotBetween:
                    return data.Length < this.FirstArgumentAsInt || data.Length > this.SecondArgumentAsInt;

                case StringSelectionType.Includes:
                    return data.IndexOf(this.FirstArgument) != -1;

                case StringSelectionType.NotIncludes:
                    return data.IndexOf(this.FirstArgument) == -1;

                case StringSelectionType.StartsWith:
                    return data.StartsWith(this.FirstArgument);

                case StringSelectionType.NotStartsWith:
                    return !data.StartsWith(this.FirstArgument);

                case StringSelectionType.EndsWith:
                    return data.EndsWith(this.FirstArgument);

                case StringSelectionType.NotEndsWith:
                    return !data.EndsWith(this.FirstArgument);

                case StringSelectionType.StartsAndEndsWith:
                    return data.StartsWith(this.FirstArgument) && data.EndsWith(this.SecondArgument);

                case StringSelectionType.NotStartsAndEndsWith:
                    return !data.StartsWith(this.FirstArgument) || !data.EndsWith(this.SecondArgument);

                case StringSelectionType.Equals:
                    return data.Equals(this.FirstArgument);

                case StringSelectionType.NotEquals:
                    return !data.Equals(this.FirstArgument);

                default:
                    throw new ProteusException($"Internal error: Proteus is not handling string selection type '{this.SelectionType}'!");
            }
        }
    }
}
