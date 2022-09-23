////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;

namespace LaurentiuCristofor.Proteus.Common
{
    /// <summary>
    /// A collection of argument checks.
    /// </summary>
    internal abstract class ArgumentChecker
    {
        internal static void CheckNotNull(string argument)
        {
            if (argument == null)
            {
                throw new ProteusException($"An expected argument is missing!");
            }
        }

        internal static void CheckNotNull(DataTypeContainer argument)
        {
            if (argument == null)
            {
                throw new ProteusException($"An expected argument is missing!");
            }
        }

        internal static void CheckNotNullAndNotEmpty(string argument)
        {
            if (String.IsNullOrEmpty(argument))
            {
                throw new ProteusException($"An expected argument is missing or is an empty string!");
            }
        }

        internal static void CheckIsOneCharacter(string argument)
        {
            if (String.IsNullOrEmpty(argument) || argument.Length != 1)
            {
                throw new ProteusException($"An expected argument is missing or is not a one-character string!");
            }
        }

        internal static void CheckPositive(int argument)
        {
            if (argument <= 0)
            {
                throw new ProteusException($"An integer argument is negative or zero: {argument}!");
            }
        }

        internal static void CheckNotNegative(int argument)
        {
            if (argument < 0)
            {
                throw new ProteusException($"An integer argument is negative: {argument}!");
            }
        }

        internal static void CheckNotZero(ulong argument)
        {
            if (argument == 0)
            {
                throw new ProteusException($"An unsigned long integer argument is zero!");
            }
        }

        internal static void CheckInterval(int intervalStart, int intervalEnd)
        {
            if (intervalStart > intervalEnd)
            {
                throw new ProteusException($"An incorrect integer interval was provided: the start is larger than the end: ({intervalStart}, {intervalEnd})!");
            }
        }

        internal static void CheckInterval(ulong intervalStart, ulong intervalEnd)
        {
            if (intervalStart > intervalEnd)
            {
                throw new ProteusException($"An incorrect integer interval was provided: the start is larger than the end: ({intervalStart}, {intervalEnd})!");
            }
        }

        internal static void CheckDataType(DataType dataType)
        {
            if (dataType == DataType.NotSet)
            {
                throw new ProteusException($"A type argument is not set!");
            }
        }
    }
}
