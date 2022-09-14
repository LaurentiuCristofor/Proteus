////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// This file is part of the Cabeiro Software and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;

using LaurentiuCristofor.Proteus.Common;

namespace LaurentiuCristofor.Cabeiro.Common
{
    /// <summary>
    /// A collection of methods for parsing command line arguments.
    /// </summary>
    public abstract class ArgumentParser
    {
        /// <summary>
        /// Checks if argument value equals the given command name.
        /// </summary>
        /// <param name="argument">The argument value to check.</param>
        /// <param name="commandName">The command name to check against.</param>
        /// <returns>True if the comparison succeeded; false otherwise.</returns>
        public static bool IsCommand(string argument, string commandName)
        {
            return argument.ToLower().Equals(commandName);
        }

        /// <summary>
        /// Checks if the argument number matches the expected argument numbers.
        /// </summary>
        /// <param name="argumentNumber">The argument number to check.</param>
        /// <param name="minimumArgumentNumber">The minimum number of expected arguments.</param>
        /// <param name="maximumArgumentNumber">The maximum number of expected arguments.</param>
        /// <returns>True if the argumentNumber is as expected; false otherwise.</returns>
        public static bool HasExpectedArgumentNumber(int argumentNumber, int minimumArgumentNumber, int maximumArgumentNumber)
        {
            if (maximumArgumentNumber < minimumArgumentNumber
                || minimumArgumentNumber < 0)
            {
                throw new ProteusException($"Internal error: invalid argument number range: {minimumArgumentNumber}-{maximumArgumentNumber}!");
            }

            return argumentNumber >= minimumArgumentNumber
                && argumentNumber <= maximumArgumentNumber;
        }

        /// <summary>
        /// Checks if the argument number matches the expected argument number.
        /// </summary>
        /// <param name="argumentNumber">The argument number to check.</param>
        /// <param name="expectedArgumentNumber">The number of expected arguments.</param>
        /// <returns>True if the argumentNumber is as expected; false otherwise.</returns>
        public static bool HasExpectedArgumentNumber(int argumentNumber, int expectedArgumentNumber)
        {
            if (expectedArgumentNumber < 0)
            {
                throw new ProteusException($"Internal error: invalid expected argument number: {expectedArgumentNumber}!");
            }

            return argumentNumber == expectedArgumentNumber;
        }

        /// <summary>
        /// Interprets an argument as a positive integer value.
        /// </summary>
        /// <param name="argument">The argument value to process.</param>
        /// <param name="acceptZero">Whether a zero value is accepted.</param>
        /// <returns>An integer value or an exception if the value is not a valid integer.</returns>
        public static int GetPositiveInteger(string argument, bool acceptZero = false)
        {
            int argumentValue = int.Parse(argument);

            if (argumentValue < 0)
            {
                throw new CabeiroException($"Invalid positive integer value: {argumentValue}!");
            }

            if (!acceptZero && argumentValue == 0)
            {
                throw new CabeiroException("A non-zero integer value was expected!");
            }

            return argumentValue;
        }

        /// <summary>
        /// Interprets an argument as an unsigned long integer value.
        /// </summary>
        /// <param name="argument">The argument value to process.</param>
        /// <param name="acceptZero">Whether a zero value is accepted.</param>
        /// <returns>An unsigned long integer value or an exception if the value is not a valid unsigned long integer.</returns>
        public static ulong GetUnsignedLongInteger(string argument, bool acceptZero = false)
        {
            ulong argumentValue = ulong.Parse(argument);

            if (!acceptZero && argumentValue == 0)
            {
                throw new CabeiroException("A non-zero unsigned long integer value was expected!");
            }

            return argumentValue;
        }

        /// <summary>
        /// Look for the presence of an expected argument.
        /// </summary>
        /// <param name="arguments">The list of arguments.</param>
        /// <param name="optionalArgumentIndex">The index of the expected argument.</param>
        /// <returns>The value of the argument if one is present; otherwise an exception will be thrown.</returns>
        public static string GetExpectedArgument(string[] arguments, int expectedArgumentIndex)
        {
            if (expectedArgumentIndex >= arguments.Length)
            {
                throw new ProteusException($"Command is missing arguments!");
            }

            return arguments[expectedArgumentIndex];
        }

        /// <summary>
        /// Look for the presence of an optional argument.
        /// </summary>
        /// <param name="arguments">The list of arguments.</param>
        /// <param name="optionalArgumentIndex">The index of the optional argument.</param>
        /// <returns>The value of the argument if one is present; otherwise null.</returns>
        public static string GetOptionalArgument(string[] arguments, int optionalArgumentIndex)
        {
            if (optionalArgumentIndex >= arguments.Length)
            {
                return null;
            }

            return arguments[optionalArgumentIndex];
        }

        /// <summary>
        /// Extract the last arguments of a command.
        /// </summary>
        /// <param name="expectedOperationArguments">The number or arguments required for the current operation.</param>
        /// <param name="nextArgumentIndex">The index from where to extract the next argument.</param>
        /// <param name="arguments">The arguments that we work on.</param>
        /// <param name="firstArgument">Will get set to the first argument, or null if no argument was expected.</param>
        /// <param name="secondArgument">Will get set to the second argument, or null if no argument was expected.</param>
        /// <param name="outputFilePath">Will get set to the output file path, or null if no argument was available.</param>
        public static void ExtractLastArguments(
            int expectedOperationArguments,
            int nextArgumentIndex,
            string[] arguments,
            out string firstArgument,
            out string secondArgument,
            out string outputFilePath)
        {
            firstArgument = null;
            secondArgument = null;

            if (expectedOperationArguments >= 1)
            {
                firstArgument = GetExpectedArgument(arguments, nextArgumentIndex);
                nextArgumentIndex++;
            }

            if (expectedOperationArguments >= 2)
            {
                secondArgument = GetExpectedArgument(arguments, nextArgumentIndex);
                nextArgumentIndex++;
            }

            outputFilePath = GetOptionalArgument(arguments, nextArgumentIndex);
        }

        /// <summary>
        /// Parses a separator argument value to replace reserved names with their corresponding separator strings.
        /// </summary>
        /// <param name="argument">The string to parse.</param>
        /// <returns>The value to use as separator.</returns>
        public static string ParseSeparator(string argument)
        {
            if (argument.Equals(Constants.Commands.Arguments.Tab))
            {
                return Constants.Formatting.TabSeparator;
            }

            return argument;
        }

        /// <summary>
        /// Parses argument value as a DataType indicator.
        /// </summary>
        /// <param name="argument">The argument value to parse.</param>
        /// <returns>A DataType if the parsing was successful; an exception will be thrown otherwise.</returns>
        public static DataType ParseDataType(string argument)
        {
            string lowercaseValue = argument.ToLower();

            if (lowercaseValue.Equals(Constants.Commands.Arguments.DataTypeString))
            {
                return DataType.String;
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.DataTypeInteger))
            {
                return DataType.Integer;
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.DataTypeUnsignedInteger))
            {
                return DataType.UnsignedInteger;
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.DataTypeFloatingPoint))
            {
                return DataType.FloatingPoint;
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.DataTypeDateTime))
            {
                return DataType.DateTime;
            }
            else
            {
                throw new CabeiroException($"Invalid data type argument: {argument}!");
            }
        }

        /// <summary>
        /// Parses argument value as a ComparisonType indicator.
        /// </summary>
        /// <param name="argument">The argument value to parse.</param>
        /// <returns>A tuple containing the ComparisonType and its number of associated arguments if the parsing was successful; an exception will be thrown otherwise.</returns>
        public static Tuple<ComparisonType, int> ParseComparisonType(string argument)
        {
            string lowercaseValue = argument.ToLower();

            if (lowercaseValue.Equals(Constants.Commands.Arguments.ComparisonLessThan))
            {
                return new Tuple<ComparisonType, int>(ComparisonType.LessThan, 1);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.ComparisonLessThanOrEqual))
            {
                return new Tuple<ComparisonType, int>(ComparisonType.LessThanOrEqual, 1);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.ComparisonEqual))
            {
                return new Tuple<ComparisonType, int>(ComparisonType.Equal, 1);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.ComparisonGreaterThanOrEqual))
            {
                return new Tuple<ComparisonType, int>(ComparisonType.GreaterThanOrEqual, 1);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.ComparisonGreaterThan))
            {
                return new Tuple<ComparisonType, int>(ComparisonType.GreaterThan, 1);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.ComparisonNotEqual))
            {
                return new Tuple<ComparisonType, int>(ComparisonType.NotEqual, 1);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.ComparisonBetween))
            {
                return new Tuple<ComparisonType, int>(ComparisonType.Between, 2);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.ComparisonStrictlyBetween))
            {
                return new Tuple<ComparisonType, int>(ComparisonType.StrictlyBetween, 2);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.ComparisonNotBetween))
            {
                return new Tuple<ComparisonType, int>(ComparisonType.NotBetween, 2);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.ComparisonNotStrictlyBetween))
            {
                return new Tuple<ComparisonType, int>(ComparisonType.NotStrictlyBetween, 2);
            }
            else
            {
                throw new CabeiroException($"Invalid comparison type argument: {argument}!");
            }
        }

        /// <summary>
        /// Parses argument value as a PositionSelectionType indicator.
        /// </summary>
        /// <param name="argument">The argument value to parse.</param>
        /// <returns>A tuple containing the PositionSelectionType and its number of associated arguments if the parsing was successful; an exception will be thrown otherwise.</returns>
        public static Tuple<PositionSelectionType, int> ParsePositionSelectionType(string argument)
        {
            string lowercaseValue = argument.ToLower();

            if (lowercaseValue.Equals(Constants.Commands.Arguments.PositionSelectionBetween))
            {
                return new Tuple<PositionSelectionType, int>(PositionSelectionType.Between, 2);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.PositionSelectionNotBetween))
            {
                return new Tuple<PositionSelectionType, int>(PositionSelectionType.NotBetween, 2);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.PositionSelectionLast))
            {
                return new Tuple<PositionSelectionType, int>(PositionSelectionType.Last, 1);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.PositionSelectionNotLast))
            {
                return new Tuple<PositionSelectionType, int>(PositionSelectionType.NotLast, 1);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.PositionSelectionEach))
            {
                return new Tuple<PositionSelectionType, int>(PositionSelectionType.Each, 1);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.PositionSelectionNotEach))
            {
                return new Tuple<PositionSelectionType, int>(PositionSelectionType.NotEach, 1);
            }
            else
            {
                throw new CabeiroException($"Invalid number selection type argument: {argument}!");
            }
        }

        /// <summary>
        /// Parses argument value as a PositionInsertionType indicator.
        /// </summary>
        /// <param name="argument">The argument value to parse.</param>
        /// <returns>A tuple containing the PositionInsertionType and its number of associated arguments if the parsing was successful; an exception will be thrown otherwise.</returns>
        public static Tuple<PositionInsertionType, int> ParsePositionInsertionType(string argument)
        {
            string lowercaseValue = argument.ToLower();

            if (lowercaseValue.Equals(Constants.Commands.Arguments.PositionInsertionPosition))
            {
                return new Tuple<PositionInsertionType, int>(PositionInsertionType.Position, 1);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.PositionInsertionEach))
            {
                return new Tuple<PositionInsertionType, int>(PositionInsertionType.Each, 1);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.PositionInsertionLast))
            {
                return new Tuple<PositionInsertionType, int>(PositionInsertionType.Last, 0);
            }
            else
            {
                throw new CabeiroException($"Invalid number insertion type argument: {argument}!");
            }
        }

        /// <summary>
        /// Parses argument value as a StringSelectionType indicator.
        /// </summary>
        /// <param name="argument">The argument value to parse.</param>
        /// <returns>A tuple containing the StringSelectionType and its number of associated arguments if the parsing was successful; an exception will be thrown otherwise.</returns>
        public static Tuple<StringSelectionType, int> ParseStringSelectionType(string argument)
        {
            string lowercaseValue = argument.ToLower();

            if (lowercaseValue.Equals(Constants.Commands.Arguments.StringSelectionHasLengthBetween))
            {
                return new Tuple<StringSelectionType, int>(StringSelectionType.HasLengthBetween, 2);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.StringSelectionHasLengthNotBetween))
            {
                return new Tuple<StringSelectionType, int>(StringSelectionType.HasLengthNotBetween, 2);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.StringSelectionIncludes))
            {
                return new Tuple<StringSelectionType, int>(StringSelectionType.Includes, 1);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.StringSelectionNotIncludes))
            {
                return new Tuple<StringSelectionType, int>(StringSelectionType.NotIncludes, 1);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.StringSelectionStartsWith))
            {
                return new Tuple<StringSelectionType, int>(StringSelectionType.StartsWith, 1);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.StringSelectionNotStartsWith))
            {
                return new Tuple<StringSelectionType, int>(StringSelectionType.NotStartsWith, 1);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.StringSelectionEndsWith))
            {
                return new Tuple<StringSelectionType, int>(StringSelectionType.EndsWith, 1);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.StringSelectionNotEndsWith))
            {
                return new Tuple<StringSelectionType, int>(StringSelectionType.NotEndsWith, 1);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.StringSelectionIsDemarked))
            {
                return new Tuple<StringSelectionType, int>(StringSelectionType.IsDemarked, 2);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.StringSelectionIsNotDemarked))
            {
                return new Tuple<StringSelectionType, int>(StringSelectionType.IsNotDemarked, 2);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.StringSelectionEquals))
            {
                return new Tuple<StringSelectionType, int>(StringSelectionType.Equals, 1);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.StringSelectionNotEquals))
            {
                return new Tuple<StringSelectionType, int>(StringSelectionType.NotEquals, 1);
            }
            else
            {
                throw new CabeiroException($"Invalid string selection type argument: {argument}!");
            }
        }

        /// <summary>
        /// Parses argument value as a StringEditType indicator.
        /// </summary>
        /// <param name="argument">The argument value to parse.</param>
        /// <returns>A tuple containing the StringEditType and its number of associated arguments if the parsing was successful; an exception will be thrown otherwise.</returns>
        public static Tuple<StringEditType, int> ParseStringEditType(string argument)
        {
            string lowercaseValue = argument.ToLower();

            if (lowercaseValue.Equals(Constants.Commands.Arguments.StringEditTypeRewrite))
            {
                return new Tuple<StringEditType, int>(StringEditType.Rewrite, 0);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.StringEditTypeUppercase))
            {
                return new Tuple<StringEditType, int>(StringEditType.Uppercase, 0);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.StringEditTypeLowercase))
            {
                return new Tuple<StringEditType, int>(StringEditType.Lowercase, 0);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.StringEditTypeTrimStart))
            {
                return new Tuple<StringEditType, int>(StringEditType.TrimStart, 0);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.StringEditTypeTrimEnd))
            {
                return new Tuple<StringEditType, int>(StringEditType.TrimEnd, 0);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.StringEditTypeTrim))
            {
                return new Tuple<StringEditType, int>(StringEditType.Trim, 0);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.StringEditTypeInvert))
            {
                return new Tuple<StringEditType, int>(StringEditType.Invert, 0);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.StringEditTypePrefixLineNumbers))
            {
                return new Tuple<StringEditType, int>(StringEditType.PrefixLineNumbers, 1);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.StringEditTypeAddPrefix))
            {
                return new Tuple<StringEditType, int>(StringEditType.AddPrefix, 1);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.StringEditTypeAddSuffix))
            {
                return new Tuple<StringEditType, int>(StringEditType.AddSuffix, 1);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.StringEditTypeDeletePrefix))
            {
                return new Tuple<StringEditType, int>(StringEditType.DeletePrefix, 1);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.StringEditTypeDeleteSuffix))
            {
                return new Tuple<StringEditType, int>(StringEditType.DeleteSuffix, 1);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.StringEditTypeDeleteFirstCharacters))
            {
                return new Tuple<StringEditType, int>(StringEditType.DeleteFirstCharacters, 1);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.StringEditTypeDeleteLastCharacters))
            {
                return new Tuple<StringEditType, int>(StringEditType.DeleteLastCharacters, 1);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.StringEditTypeKeepFirstCharacters))
            {
                return new Tuple<StringEditType, int>(StringEditType.KeepFirstCharacters, 1);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.StringEditTypeKeepLastCharacters))
            {
                return new Tuple<StringEditType, int>(StringEditType.KeepLastCharacters, 1);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.StringEditTypeDeleteContentAtIndex))
            {
                return new Tuple<StringEditType, int>(StringEditType.DeleteContentAtIndex, 2);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.StringEditTypeKeepContentAtIndex))
            {
                return new Tuple<StringEditType, int>(StringEditType.KeepContentAtIndex, 2);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.StringEditTypeInsertContentAtIndex))
            {
                return new Tuple<StringEditType, int>(StringEditType.InsertContentAtIndex, 2);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.StringEditTypeReplaceContent))
            {
                return new Tuple<StringEditType, int>(StringEditType.ReplaceContent, 2);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.StringEditTypeDeleteContentBeforeMarker))
            {
                return new Tuple<StringEditType, int>(StringEditType.DeleteContentBeforeMarker, 1);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.StringEditTypeDeleteContentAfterMarker))
            {
                return new Tuple<StringEditType, int>(StringEditType.DeleteContentAfterMarker, 1);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.StringEditTypeKeepContentBeforeMarker))
            {
                return new Tuple<StringEditType, int>(StringEditType.KeepContentBeforeMarker, 1);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.StringEditTypeKeepContentAfterMarker))
            {
                return new Tuple<StringEditType, int>(StringEditType.KeepContentAfterMarker, 1);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.StringEditTypeInsertContentBeforeMarker))
            {
                return new Tuple<StringEditType, int>(StringEditType.InsertContentBeforeMarker, 2);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.StringEditTypeInsertContentAfterMarker))
            {
                return new Tuple<StringEditType, int>(StringEditType.InsertContentAfterMarker, 2);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.StringEditTypeDeleteContentBetweenMarkers))
            {
                return new Tuple<StringEditType, int>(StringEditType.DeleteContentBetweenMarkers, 2);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.StringEditTypeKeepContentBetweenMarkers))
            {
                return new Tuple<StringEditType, int>(StringEditType.KeepContentBetweenMarkers, 2);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.StringEditTypeKeepContentOutsideMarkers))
            {
                return new Tuple<StringEditType, int>(StringEditType.KeepContentOutsideMarkers, 2);
            }
            else
            {
                throw new CabeiroException($"Invalid string edit type argument: {argument}!");
            }
        }
    }
}
