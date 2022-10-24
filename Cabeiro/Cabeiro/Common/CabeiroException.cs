////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
///
/// This file is part of the Cabeiro Software and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;

namespace LaurentiuCristofor.Cabeiro.Common
{
    /// <summary>
    /// The exception class used by Cabeiro to indicate errors.
    /// </summary>
    public class CabeiroException : Exception
    {
        public CabeiroException(string message) : base(message)
        {
        }
    }
}
