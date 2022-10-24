////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
///
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;

namespace LaurentiuCristofor.Proteus.Common
{
    /// <summary>
    /// The exception class used by Proteus to indicate errors.
    /// </summary>
    public class ProteusException : Exception
    {
        public ProteusException(string message) : base(message)
        {
        }
    }
}
