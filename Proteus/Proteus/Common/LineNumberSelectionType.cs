////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

namespace LaurentiuCristofor.Proteus.Common
{
    /// <summary>
    /// Types of line number comparisons supported by Proteus.
    /// </summary>
    public enum LineNumberSelectionType
    {
        NotSet = 0,

        First = 1,
        NotFirst = 2,
        Last = 3,
        NotLast = 4, 
        Between = 5,
        NotBetween = 6,
        Each = 7,
        NotEach = 8,
    }
}
