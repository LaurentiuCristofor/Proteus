////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

namespace LaurentiuCristofor.Proteus.Common
{
    /// <summary>
    /// A collection of argument checks.
    /// </summary>
    internal abstract class ArgumentChecker
    {
        internal static void CheckPresence(string argument)
        {
            if (argument == null)
            {
                throw new ProteusException($"An expected argument is missing!");
            }
        }

        internal static void CheckPositive(int argument)
        {
            if (argument <= 0)
            {
                throw new ProteusException($"An integer argument is negative or zero!");
            }
        }

        internal static void CheckNotNegative(int argument)
        {
            if (argument < 0)
            {
                throw new ProteusException($"An integer argument is negative!");
            }
        }

        internal static void CheckNotZero(ulong argument)
        {
            if (argument == 0)
            {
                throw new ProteusException($"An unsigned long integer argument is zero!");
            }
        }
    }
}
