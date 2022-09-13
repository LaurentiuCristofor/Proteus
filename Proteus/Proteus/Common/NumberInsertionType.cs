////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

namespace LaurentiuCristofor.Proteus.Common
{
    /// <summary>
    /// Types of insertions at locations specified by a number (for example: line or column number).
    /// </summary>
    public enum NumberInsertionType
    {
        NotSet = 0,

        Position = 1,
        Each = 2,
        Last = 3,
    }
}
