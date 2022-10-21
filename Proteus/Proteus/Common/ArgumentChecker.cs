////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
///
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;

using LaurentiuCristofor.Proteus.Common.Types;

namespace LaurentiuCristofor.Proteus.Common
{
    /// <summary>
    /// A collection of argument checks performed by Proteus.
    /// </summary>
    internal abstract class ArgumentChecker
    {
        /// <summary>
        /// Checks the presence of an argument in an argument array.
        /// </summary>
        /// <typeparam name="T">The type of argument.</typeparam>
        /// <param name="parameters">The argument array.</param>
        /// <param name="parameterIndex">The index of the expected argument.</param>
        internal static void CheckPresence<T>(T[] parameters, int parameterIndex)
        {
            if (parameters == null || parameterIndex > parameters.Length - 1)
            {
                throw new ProteusException($"An expected argument is missing!");
            }
        }

        /// <summary>
        /// Checks that an argument is not null.
        /// </summary>
        /// <typeparam name="T">The type of argument.</typeparam>
        /// <param name="argument">The argument.</param>
        internal static void CheckNotNull<T>(T argument)
        {
            if (argument == null)
            {
                throw new ProteusException($"An expected argument is null!");
            }
        }

        /// <summary>
        /// Checks that a string argument is not null or empty.
        /// </summary>
        /// <param name="argument">The string argument.</param>
        internal static void CheckNotNullAndNotEmpty(string argument)
        {
            if (String.IsNullOrEmpty(argument))
            {
                throw new ProteusException($"An expected argument is null or is an empty string!");
            }
        }

        /// <summary>
        /// Checks that a string argument contains a single character.
        /// </summary>
        /// <param name="argument">The string argument.</param>
        internal static void CheckOneCharacter(string argument)
        {
            if (String.IsNullOrEmpty(argument) || argument.Length != 1)
            {
                throw new ProteusException($"An expected argument is null or is not a one-character string!");
            }
        }

        /// <summary>
        /// Checks that an argument value is greater than or equal to a threshold value.
        /// </summary>
        /// <typeparam name="T">The argument type.</typeparam>
        /// <param name="argument">The argument.</param>
        /// <param name="threshold">The threshold.</param>
        internal static void CheckGreaterThanOrEqualTo<T>(T argument, T threshold)
            where T : IComparable
        {
            if (argument.CompareTo(threshold) < 0)
            {
                throw new ProteusException($"An argument with value {argument} is smaller than the minimum value expected for it: {threshold}!");
            }
        }

        /// <summary>
        /// Checks than an argument is different than a specific value.
        /// </summary>
        /// <typeparam name="T">The argument type.</typeparam>
        /// <param name="argument">The argument.</param>
        /// <param name="value">The value that the argument is not expected to take.</param>
        internal static void CheckNotEqual<T>(T argument, T value)
        {
            if (argument.Equals(value))
            {
                throw new ProteusException($"An argument has unexpected value: {value}!");
            }
        }

        /// <summary>
        /// Checks that two arguments are not equal to each other.
        /// </summary>
        /// <typeparam name="T">The type of both arguments.</typeparam>
        /// <param name="firstArgument">The first argument.</param>
        /// <param name="secondArgument">The second argument.</param>
        internal static void CheckDifferent<T>(T firstArgument, T secondArgument)
        {
            if (secondArgument.Equals(firstArgument))
            {
                throw new ProteusException($"Two different arguments were expected, but both arguments provided have the same value: {firstArgument}!");
            }
        }

        /// <summary>
        /// Checks that two arguments represent valid bounds of an interval.
        /// </summary>
        /// <typeparam name="T">The type of both arguments.</typeparam>
        /// <param name="intervalStart">The argument indicating the interval start.</param>
        /// <param name="intervalEnd">The argument indicating the interval end.</param>
        internal static void CheckInterval<T>(T intervalStart, T intervalEnd)
            where T:IComparable
        {
            if (intervalStart.CompareTo(intervalEnd) > 0)
            {
                throw new ProteusException($"An incorrect interval was provided: the start is larger than the end: ({intervalStart}, {intervalEnd})!");
            }
        }

        /// <summary>
        /// Checks that a data type is set to a valid value.
        /// </summary>
        /// <param name="dataType">The data type to check.</param>
        internal static void CheckDataType(DataType dataType)
        {
            if (dataType == DataType.NotSet)
            {
                throw new ProteusException($"A data type argument is not set!");
            }
        }
    }
}
