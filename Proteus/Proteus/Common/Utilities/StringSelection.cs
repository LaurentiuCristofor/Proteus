////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
///
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using LaurentiuCristofor.Proteus.Common.Algorithms;
using LaurentiuCristofor.Proteus.Common.Types;

namespace LaurentiuCristofor.Proteus.Common.Utilities
{
    public abstract class StringSelection
    {
        /// <summary>
        /// Performs an edit operation of a string using the initializationg parameters.
        /// </summary>
        /// <param name="data">The data to select.</param>
        /// <param name="selectionType">The selection criterion.</param>
        /// <param name="firstString">A string selection argument, if expected.</param>
        /// <param name="secondString">A second string selection argument, if expected.</param>
        /// <param name="firstCharCount">A character count selection argument, if expected.</param>
        /// <param name="secondCharCount">A second character count selection argument, if expected.</param>
        /// <returns></returns>
        public static bool Select(string data, StringSelectionType selectionType, string firstString, string secondString = null, int firstCharCount = 0, int secondCharCount = 0)
        {
            switch (selectionType)
            {
                case StringSelectionType.HasLengthBetween:
                    return data.Length >= firstCharCount && data.Length <= secondCharCount;

                case StringSelectionType.HasLengthNotBetween:
                    return data.Length < firstCharCount || data.Length > secondCharCount;

                case StringSelectionType.Includes:
                    return data.IndexOf(firstString) != -1;

                case StringSelectionType.NotIncludes:
                    return data.IndexOf(firstString) == -1;

                case StringSelectionType.StartsWith:
                    return data.StartsWith(firstString);

                case StringSelectionType.NotStartsWith:
                    return !data.StartsWith(firstString);

                case StringSelectionType.EndsWith:
                    return data.EndsWith(firstString);

                case StringSelectionType.NotEndsWith:
                    return !data.EndsWith(firstString);

                case StringSelectionType.StartsAndEndsWith:
                    return data.StartsWith(firstString) && data.EndsWith(secondString);

                case StringSelectionType.NotStartsAndEndsWith:
                    return !data.StartsWith(firstString) || !data.EndsWith(secondString);

                case StringSelectionType.Equals:
                    return data.Equals(firstString);

                case StringSelectionType.NotEquals:
                    return !data.Equals(firstString);

                case StringSelectionType.IncludesBefore:
                    return StringOperations.FindMarkers(data, firstString, secondString, out _, out _);

                case StringSelectionType.NotIncludesBefore:
                    return !StringOperations.FindMarkers(data, firstString, secondString, out _, out _);

                default:
                    throw new ProteusException($"Internal error: Proteus is not handling string selection type '{selectionType}'!");
            }
        }
    }
}
