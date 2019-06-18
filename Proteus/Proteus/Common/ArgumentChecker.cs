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
                throw new ProteusException($"Internal error: an expected argument is missing!");
            }
        }
    }
}
