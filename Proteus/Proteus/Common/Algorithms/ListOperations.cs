////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
///
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;

namespace LaurentiuCristofor.Proteus.Common.Algorithms
{
    public abstract class ListOperations
    {
        /// <summary>
        /// Exchanges two elements in a list.
        /// Does not check the validity of the input.
        /// </summary>
        /// <typeparam name="T">The list element type.</typeparam>
        /// <param name="list">The list in which to exchange elements.</param>
        /// <param name="i">The index of the first element to exchange.</param>
        /// <param name="j">The index of the second element to exchange.</param>
        public static void Exchange<T>(List<T> list, int i, int j)
        {
            T savedValue = list[i];
            list[i] = list[j];
            list[j] = savedValue;
        }
    }
}
