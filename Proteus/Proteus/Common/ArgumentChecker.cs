////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
///
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;

using LaurentiuCristofor.Proteus.Common.DataHolders;
using LaurentiuCristofor.Proteus.Common.Types;

namespace LaurentiuCristofor.Proteus.Common
{
    /// <summary>
    /// A collection of argument checks.
    /// </summary>
    internal abstract class ArgumentChecker
    {
        internal static void CheckPresence<T>(T[] parameters, int parameterIndex)
        {
            if (parameters == null || parameterIndex > parameters.Length - 1)
            {
                throw new ProteusException($"An expected argument is missing!");
            }
        }

        internal static void CheckNotNull<T>(T argument)
        {
            if (argument == null)
            {
                throw new ProteusException($"An expected argument is null!");
            }
        }

        internal static void CheckNotNullAndNotEmpty(string argument)
        {
            if (String.IsNullOrEmpty(argument))
            {
                throw new ProteusException($"An expected argument is null or is an empty string!");
            }
        }

        internal static void CheckIsOneCharacter(string argument)
        {
            if (String.IsNullOrEmpty(argument) || argument.Length != 1)
            {
                throw new ProteusException($"An expected argument is null or is not a one-character string!");
            }
        }

        internal static void CheckStrictlyPositive(int argument)
        {
            if (argument <= 0)
            {
                throw new ProteusException($"An integer argument is not strictly positive: {argument}!");
            }
        }

        internal static void CheckPositive(int argument)
        {
            if (argument < 0)
            {
                throw new ProteusException($"An integer argument is not positive: {argument}!");
            }
        }

        internal static void CheckNotZero(ulong argument)
        {
            if (argument == 0)
            {
                throw new ProteusException($"An unsigned long integer argument is zero!");
            }
        }

        internal static void CheckNotZero(double argument)
        {
            if (argument == 0)
            {
                throw new ProteusException($"A double argument is zero!");
            }
        }

        internal static void CheckDifferent<T>(T firstArgument, T secondArgument)
        {
            if (secondArgument.Equals(firstArgument))
            {
                throw new ProteusException($"Two different arguments were expected, but both arguments provided have the same value: {firstArgument}!");
            }
        }

        internal static void CheckInterval<T>(T intervalStart, T intervalEnd)
            where T:IComparable
        {
            if (intervalStart.CompareTo(intervalEnd) > 0)
            {
                throw new ProteusException($"An incorrect interval was provided: the start is larger than the end: ({intervalStart}, {intervalEnd})!");
            }
        }

        internal static void CheckDataType(DataType dataType)
        {
            if (dataType == DataType.NotSet)
            {
                throw new ProteusException($"A data type argument is not set!");
            }
        }
    }
}
