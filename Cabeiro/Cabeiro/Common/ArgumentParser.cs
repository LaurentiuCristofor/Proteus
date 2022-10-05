////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
///
/// This file is part of the Cabeiro Software and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;

using LaurentiuCristofor.Proteus.Common.Types;

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
        /// Checks if the argument number matches the expected argument numbers
        /// and throws an exception if it does not.
        /// </summary>
        /// <param name="argumentNumber">The argument number to check.</param>
        /// <param name="minimumArgumentNumber">The minimum number of expected arguments.</param>
        /// <param name="maximumArgumentNumber">The maximum number of expected arguments.</param>
        public static void CheckExpectedArgumentNumber(int argumentNumber, int minimumArgumentNumber, int maximumArgumentNumber)
        {
            if (maximumArgumentNumber < minimumArgumentNumber
                || minimumArgumentNumber < 0)
            {
                throw new CabeiroException($"Internal error: invalid argument number range: {minimumArgumentNumber}-{maximumArgumentNumber}!");
            }

            if (argumentNumber < minimumArgumentNumber)
            {
                throw new CabeiroException($"Command expects at least {minimumArgumentNumber} arguments, but has received only {argumentNumber} arguments!");
            }
            else if (argumentNumber > maximumArgumentNumber)
            {
                throw new CabeiroException($"Command expects no more than {maximumArgumentNumber} arguments, but has received {argumentNumber} arguments!");
            }
        }

        /// <summary>
        /// Checks if the argument number matches the expected argument number
        /// and throws an exception if it does not.
        /// </summary>
        /// <param name="argumentNumber">The argument number to check.</param>
        /// <param name="expectedArgumentNumber">The number of expected arguments.</param>
        public static void CheckExpectedArgumentNumber(int argumentNumber, int expectedArgumentNumber)
        {
            if (expectedArgumentNumber < 0)
            {
                throw new CabeiroException($"Internal error: invalid expected argument number: {expectedArgumentNumber}!");
            }

            if (argumentNumber != expectedArgumentNumber)
            {
                throw new CabeiroException($"Command expected {expectedArgumentNumber} arguments, but has received {argumentNumber} arguments!");
            }
        }

        /// <summary>
        /// Interprets an argument as a strictly positive integer value.
        /// </summary>
        /// <param name="argument">The argument value to process.</param>
        /// <returns>An integer value or an exception if the value is not a valid strictly positive integer.</returns>
        public static int GetStrictlyPositiveInteger(string argument)
        {
            int argumentValue = int.Parse(argument);

            if (argumentValue <= 0)
            {
                throw new CabeiroException($"Invalid strictly positive integer value: {argumentValue}!");
            }

            return argumentValue;
        }

        /// <summary>
        /// Interprets an argument as a positive integer value.
        /// </summary>
        /// <param name="argument">The argument value to process.</param>
        /// <returns>An integer value or an exception if the value is not a valid positive integer.</returns>
        public static int GetPositiveInteger(string argument)
        {
            int argumentValue = int.Parse(argument);

            if (argumentValue < 0)
            {
                throw new CabeiroException($"Invalid positive integer value: {argumentValue}!");
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
        /// Looks for the presence of an expected argument.
        /// </summary>
        /// <param name="arguments">The list of arguments.</param>
        /// <param name="optionalArgumentIndex">The index of the expected argument.</param>
        /// <returns>The value of the argument if one is present; otherwise an exception will be thrown.</returns>
        public static string GetExpectedArgument(string[] arguments, int expectedArgumentIndex)
        {
            if (expectedArgumentIndex >= arguments.Length)
            {
                throw new CabeiroException($"Command is missing arguments!");
            }

            return arguments[expectedArgumentIndex];
        }

        /// <summary>
        /// Looks for the presence of an optional argument.
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
        /// Extracts the last arguments of a command.
        /// </summary>
        /// <param name="expectedOperationArguments">The number or arguments required for the current operation.</param>
        /// <param name="nextArgumentIndex">The index from where to extract the next argument.</param>
        /// <param name="arguments">The arguments that we work on.</param>
        /// <param name="operationArguments">Will collect the arguments, or will be set to an empty array if no arguments were expected.</param>
        /// <param name="outputFilePath">Will get set to the output file path, or null if no argument was available.</param>
        public static void ExtractLastArguments(
            int expectedOperationArguments,
            int nextArgumentIndex,
            string[] arguments,
            out string[] operationArguments,
            out string outputFilePath)
        {
            operationArguments = new string[expectedOperationArguments];

            if (expectedOperationArguments >= 1)
            {
                operationArguments[0] = GetExpectedArgument(arguments, nextArgumentIndex);
                ++nextArgumentIndex;
            }

            if (expectedOperationArguments >= 2)
            {
                operationArguments[1] = GetExpectedArgument(arguments, nextArgumentIndex);
                ++nextArgumentIndex;
            }

            if (expectedOperationArguments >= 3)
            {
                operationArguments[2] = GetExpectedArgument(arguments, nextArgumentIndex);
                ++nextArgumentIndex;
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

            if (lowercaseValue.Equals(Constants.Commands.Arguments.ComparisonTypeLessThan))
            {
                return new Tuple<ComparisonType, int>(ComparisonType.LessThan, 1);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.ComparisonTypeLessThanOrEqual))
            {
                return new Tuple<ComparisonType, int>(ComparisonType.LessThanOrEqual, 1);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.ComparisonTypeEqual))
            {
                return new Tuple<ComparisonType, int>(ComparisonType.Equal, 1);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.ComparisonTypeGreaterThanOrEqual))
            {
                return new Tuple<ComparisonType, int>(ComparisonType.GreaterThanOrEqual, 1);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.ComparisonTypeGreaterThan))
            {
                return new Tuple<ComparisonType, int>(ComparisonType.GreaterThan, 1);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.ComparisonTypeNotEqual))
            {
                return new Tuple<ComparisonType, int>(ComparisonType.NotEqual, 1);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.ComparisonTypeBetween))
            {
                return new Tuple<ComparisonType, int>(ComparisonType.Between, 2);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.ComparisonTypeStrictlyBetween))
            {
                return new Tuple<ComparisonType, int>(ComparisonType.StrictlyBetween, 2);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.ComparisonTypeNotBetween))
            {
                return new Tuple<ComparisonType, int>(ComparisonType.NotBetween, 2);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.ComparisonTypeNotStrictlyBetween))
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

            if (lowercaseValue.Equals(Constants.Commands.Arguments.PositionSelectionTypeBetween))
            {
                return new Tuple<PositionSelectionType, int>(PositionSelectionType.Between, 2);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.PositionSelectionTypeNotBetween))
            {
                return new Tuple<PositionSelectionType, int>(PositionSelectionType.NotBetween, 2);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.PositionSelectionTypeLast))
            {
                return new Tuple<PositionSelectionType, int>(PositionSelectionType.Last, 1);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.PositionSelectionTypeNotLast))
            {
                return new Tuple<PositionSelectionType, int>(PositionSelectionType.NotLast, 1);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.PositionSelectionTypeEach))
            {
                return new Tuple<PositionSelectionType, int>(PositionSelectionType.Each, 1);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.PositionSelectionTypeNotEach))
            {
                return new Tuple<PositionSelectionType, int>(PositionSelectionType.NotEach, 1);
            }
            else
            {
                throw new CabeiroException($"Invalid position selection type argument: {argument}!");
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

            if (lowercaseValue.Equals(Constants.Commands.Arguments.PositionInsertionTypePosition))
            {
                return new Tuple<PositionInsertionType, int>(PositionInsertionType.Position, 1);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.PositionInsertionTypeEach))
            {
                return new Tuple<PositionInsertionType, int>(PositionInsertionType.Each, 1);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.PositionInsertionTypeLast))
            {
                return new Tuple<PositionInsertionType, int>(PositionInsertionType.Last, 0);
            }
            else
            {
                throw new CabeiroException($"Invalid position insertion type argument: {argument}!");
            }
        }

        /// <summary>
        /// Parses argument value as a RelativeValueSelectionType indicator for line selection.
        ///
        /// Some RelativeValueSelectionType values are redundant in the case of line selection
        /// so they are not exposed.
        /// </summary>
        /// <param name="argument">The argument value to parse.</param>
        /// <returns>A tuple containing the RelativeValueSelectionType and its number of associated arguments if the parsing was successful; an exception will be thrown otherwise.</returns>
        public static Tuple<RelativeValueSelectionType, int> ParseRelativeLineSelectionType(string argument)
        {
            string lowercaseValue = argument.ToLower();

            if (lowercaseValue.Equals(Constants.Commands.Arguments.RelativeValueSelectionFirst))
            {
                return new Tuple<RelativeValueSelectionType, int>(RelativeValueSelectionType.First, 0);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.RelativeValueSelectionNotFirst))
            {
                return new Tuple<RelativeValueSelectionType, int>(RelativeValueSelectionType.NotFirst, 0);
            }
            else
            {
                throw new CabeiroException($"Invalid relative value selection type argument: {argument}!");
            }
        }

        /// <summary>
        /// Parses argument value as a RelativeValueSelectionType indicator.
        /// </summary>
        /// <param name="argument">The argument value to parse.</param>
        /// <returns>A tuple containing the RelativeValueSelectionType and its number of associated arguments if the parsing was successful; an exception will be thrown otherwise.</returns>
        public static Tuple<RelativeValueSelectionType, int> ParseRelativeValueSelectionType(string argument)
        {
            string lowercaseValue = argument.ToLower();

            if (lowercaseValue.Equals(Constants.Commands.Arguments.RelativeValueSelectionFirst))
            {
                return new Tuple<RelativeValueSelectionType, int>(RelativeValueSelectionType.First, 0);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.RelativeValueSelectionNotFirst))
            {
                return new Tuple<RelativeValueSelectionType, int>(RelativeValueSelectionType.NotFirst, 0);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.RelativeValueSelectionLast))
            {
                return new Tuple<RelativeValueSelectionType, int>(RelativeValueSelectionType.Last, 0);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.RelativeValueSelectionNotLast))
            {
                return new Tuple<RelativeValueSelectionType, int>(RelativeValueSelectionType.NotLast, 0);
            }
            else
            {
                throw new CabeiroException($"Invalid relative value selection type argument: {argument}!");
            }
        }

        /// <summary>
        /// Parses argument value as a JoinType indicator.
        /// </summary>
        /// <param name="argument">The argument value to parse.</param>
        /// <returns>A tuple containing the JoinType and its number of associated arguments if the parsing was successful; an exception will be thrown otherwise.</returns>
        public static Tuple<JoinType, int> ParseJoinType(string argument)
        {
            string lowercaseValue = argument.ToLower();

            if (lowercaseValue.Equals(Constants.Commands.Arguments.JoinTypeInner))
            {
                return new Tuple<JoinType, int>(JoinType.Inner, 0);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.JoinTypeLeftOuter))
            {
                return new Tuple<JoinType, int>(JoinType.LeftOuter, 1);
            }
            else
            {
                throw new CabeiroException($"Invalid join type argument: {argument}!");
            }
        }

        /// <summary>
        /// Parses argument value as a LookupType indicator.
        /// </summary>
        /// <param name="argument">The argument value to parse.</param>
        /// <returns>A tuple containing the LookupType and its number of associated arguments if the parsing was successful; an exception will be thrown otherwise.</returns>
        public static Tuple<LookupType, int> ParseLookupType(string argument)
        {
            string lowercaseValue = argument.ToLower();

            if (lowercaseValue.Equals(Constants.Commands.Arguments.LookupTypeIncluded))
            {
                return new Tuple<LookupType, int>(LookupType.Included, 0);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.LookupTypeNotIncluded))
            {
                return new Tuple<LookupType, int>(LookupType.NotIncluded, 0);
            }
            else
            {
                throw new CabeiroException($"Invalid lookup type argument: {argument}!");
            }
        }

        /// <summary>
        /// Parses argument value as a ColumnTransformationType indicator.
        /// </summary>
        /// <param name="argument">The argument value to parse.</param>
        /// <returns>A tuple containing the ColumnTransformationType and its number of associated arguments if the parsing was successful; an exception will be thrown otherwise.</returns>
        public static Tuple<ColumnTransformationType, int> ParseColumnTransformationType(string argument)
        {
            string lowercaseValue = argument.ToLower();

            if (lowercaseValue.Equals(Constants.Commands.Arguments.ColumnTransformationTypePack))
            {
                return new Tuple<ColumnTransformationType, int>(ColumnTransformationType.Pack, 3);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.ColumnTransformationTypeUnpack))
            {
                return new Tuple<ColumnTransformationType, int>(ColumnTransformationType.Unpack, 2);
            }
            else
            {
                throw new CabeiroException($"Invalid column transformation type argument: {argument}!");
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

            if (lowercaseValue.Equals(Constants.Commands.Arguments.StringSelectionTypeHasLengthBetween))
            {
                return new Tuple<StringSelectionType, int>(StringSelectionType.HasLengthBetween, 2);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.StringSelectionTypeHasLengthNotBetween))
            {
                return new Tuple<StringSelectionType, int>(StringSelectionType.HasLengthNotBetween, 2);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.StringSelectionTypeIncludes ))
            {
                return new Tuple<StringSelectionType, int>(StringSelectionType.Includes, 1);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.StringSelectionTypeNotIncludes))
            {
                return new Tuple<StringSelectionType, int>(StringSelectionType.NotIncludes, 1);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.StringSelectionTypeStartsWith))
            {
                return new Tuple<StringSelectionType, int>(StringSelectionType.StartsWith, 1);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.StringSelectionTypeNotStartsWith))
            {
                return new Tuple<StringSelectionType, int>(StringSelectionType.NotStartsWith, 1);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.StringSelectionTypeEndsWith))
            {
                return new Tuple<StringSelectionType, int>(StringSelectionType.EndsWith, 1);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.StringSelectionTypeNotEndsWith))
            {
                return new Tuple<StringSelectionType, int>(StringSelectionType.NotEndsWith, 1);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.StringSelectionTypeStartsAndEndsWith))
            {
                return new Tuple<StringSelectionType, int>(StringSelectionType.StartsAndEndsWith, 2);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.StringSelectionTypeNotStartsAndEndsWith))
            {
                return new Tuple<StringSelectionType, int>(StringSelectionType.NotStartsAndEndsWith, 2);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.StringSelectionTypeEquals))
            {
                return new Tuple<StringSelectionType, int>(StringSelectionType.Equals, 1);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.StringSelectionTypeNotEquals))
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
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.StringEditTypeTrimCharsStart))
            {
                return new Tuple<StringEditType, int>(StringEditType.TrimCharsStart, 1);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.StringEditTypeTrimCharsEnd))
            {
                return new Tuple<StringEditType, int>(StringEditType.TrimCharsEnd, 1);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.StringEditTypeTrimChars))
            {
                return new Tuple<StringEditType, int>(StringEditType.TrimChars, 1);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.StringEditTypePadLeft))
            {
                return new Tuple<StringEditType, int>(StringEditType.PadLeft, 2);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.StringEditTypePadRight))
            {
                return new Tuple<StringEditType, int>(StringEditType.PadRight, 2);
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
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.StringEditTypeDeleteContentBeforeLastMarker))
            {
                return new Tuple<StringEditType, int>(StringEditType.DeleteContentBeforeLastMarker, 1);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.StringEditTypeDeleteContentAfterMarker))
            {
                return new Tuple<StringEditType, int>(StringEditType.DeleteContentAfterMarker, 1);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.StringEditTypeDeleteContentAfterLastMarker))
            {
                return new Tuple<StringEditType, int>(StringEditType.DeleteContentAfterLastMarker, 1);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.StringEditTypeKeepContentBeforeMarker))
            {
                return new Tuple<StringEditType, int>(StringEditType.KeepContentBeforeMarker, 1);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.StringEditTypeKeepContentBeforeLastMarker))
            {
                return new Tuple<StringEditType, int>(StringEditType.KeepContentBeforeLastMarker, 1);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.StringEditTypeKeepContentAfterMarker))
            {
                return new Tuple<StringEditType, int>(StringEditType.KeepContentAfterMarker, 1);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.StringEditTypeKeepContentAfterLastMarker))
            {
                return new Tuple<StringEditType, int>(StringEditType.KeepContentAfterLastMarker, 1);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.StringEditTypeInsertContentBeforeMarker))
            {
                return new Tuple<StringEditType, int>(StringEditType.InsertContentBeforeMarker, 2);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.StringEditTypeInsertContentBeforeLastMarker))
            {
                return new Tuple<StringEditType, int>(StringEditType.InsertContentBeforeLastMarker, 2);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.StringEditTypeInsertContentAfterMarker))
            {
                return new Tuple<StringEditType, int>(StringEditType.InsertContentAfterMarker, 2);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.StringEditTypeInsertContentAfterLastMarker))
            {
                return new Tuple<StringEditType, int>(StringEditType.InsertContentAfterLastMarker, 2);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.StringEditTypeDeleteContentBetweenMarkers))
            {
                return new Tuple<StringEditType, int>(StringEditType.DeleteContentBetweenMarkers, 2);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.StringEditTypeDeleteContentBetweenLastMarkers))
            {
                return new Tuple<StringEditType, int>(StringEditType.DeleteContentBetweenLastMarkers, 2);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.StringEditTypeDeleteContentBetweenInnermostMarkers))
            {
                return new Tuple<StringEditType, int>(StringEditType.DeleteContentBetweenInnermostMarkers, 2);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.StringEditTypeDeleteContentBetweenOutermostMarkers))
            {
                return new Tuple<StringEditType, int>(StringEditType.DeleteContentBetweenOutermostMarkers, 2);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.StringEditTypeKeepContentBetweenMarkers))
            {
                return new Tuple<StringEditType, int>(StringEditType.KeepContentBetweenMarkers, 2);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.StringEditTypeKeepContentBetweenLastMarkers))
            {
                return new Tuple<StringEditType, int>(StringEditType.KeepContentBetweenLastMarkers, 2);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.StringEditTypeKeepContentBetweenInnermostMarkers))
            {
                return new Tuple<StringEditType, int>(StringEditType.KeepContentBetweenInnermostMarkers, 2);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.StringEditTypeKeepContentBetweenOutermostMarkers))
            {
                return new Tuple<StringEditType, int>(StringEditType.KeepContentBetweenOutermostMarkers, 2);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.StringEditTypeKeepContentOutsideMarkers))
            {
                return new Tuple<StringEditType, int>(StringEditType.KeepContentOutsideMarkers, 2);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.StringEditTypeKeepContentOutsideLastMarkers))
            {
                return new Tuple<StringEditType, int>(StringEditType.KeepContentOutsideLastMarkers, 2);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.StringEditTypeKeepContentOutsideInnermostMarkers))
            {
                return new Tuple<StringEditType, int>(StringEditType.KeepContentOutsideInnermostMarkers, 2);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.StringEditTypeKeepContentOutsideOutermostMarkers))
            {
                return new Tuple<StringEditType, int>(StringEditType.KeepContentOutsideOutermostMarkers, 2);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.StringEditTypeSet))
            {
                return new Tuple<StringEditType, int>(StringEditType.Set, 1);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.StringEditTypeSetIfEquals))
            {
                return new Tuple<StringEditType, int>(StringEditType.SetIfEquals, 2);
            }
            else
            {
                throw new CabeiroException($"Invalid string edit type argument: {argument}!");
            }
        }

        /// <summary>
        /// Parses argument value as a ValueEditType indicator.
        /// </summary>
        /// <param name="argument">The argument value to parse.</param>
        /// <returns>A tuple containing the ValueEditType and its number of associated arguments if the parsing was successful; an exception will be thrown otherwise.</returns>
        public static Tuple<ValueEditType, int> ParseValueEditType(string argument)
        {
            string lowercaseValue = argument.ToLower();

            if (lowercaseValue.Equals(Constants.Commands.Arguments.ValueEditTypeRewrite))
            {
                return new Tuple<ValueEditType, int>(ValueEditType.Rewrite, 0);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.ValueEditTypeAdd))
            {
                return new Tuple<ValueEditType, int>(ValueEditType.Add, 1);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.ValueEditTypeSubtract))
            {
                return new Tuple<ValueEditType, int>(ValueEditType.Subtract, 1);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.ValueEditTypeMultiply))
            {
                return new Tuple<ValueEditType, int>(ValueEditType.Multiply, 1);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.ValueEditTypeDivide))
            {
                return new Tuple<ValueEditType, int>(ValueEditType.Divide, 1);
            }
            else
            {
                throw new CabeiroException($"Invalid value edit type argument: {argument}!");
            }
        }

        /// <summary>
        /// Parses argument value as a DataDistributionType indicator.
        /// </summary>
        /// <param name="argument">The argument value to parse.</param>
        /// <returns>A tuple containing the DataDistributionType and its number of associated arguments if the parsing was successful; an exception will be thrown otherwise.</returns>
        public static Tuple<DataDistributionType, int> ParseDataDistributionType(string argument)
        {
            string lowercaseValue = argument.ToLower();

            if (lowercaseValue.Equals(Constants.Commands.Arguments.DataDistributionTypeNormal))
            {
                return new Tuple<DataDistributionType, int>(DataDistributionType.Normal, 0);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.DataDistributionTypeUniform))
            {
                return new Tuple<DataDistributionType, int>(DataDistributionType.Uniform, 0);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.DataDistributionTypeExponential))
            {
                return new Tuple<DataDistributionType, int>(DataDistributionType.Exponential, 1);
            }
            else if (lowercaseValue.Equals(Constants.Commands.Arguments.DataDistributionTypePoisson))
            {
                return new Tuple<DataDistributionType, int>(DataDistributionType.Poisson, 1);
            }
            else
            {
                throw new CabeiroException($"Invalid data distribution type argument: {argument}!");
            }
        }
    }
}
