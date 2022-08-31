////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

namespace LaurentiuCristofor.Proteus.Common
{
    /// <summary>
    /// Types of number selections supported by Proteus.
    /// </summary>
    public enum NumberSelectionType
    {
        NotSet = 0,

        Between = 1,
        NotBetween = 2,
        Last = 3,
        NotLast = 4,
        Each = 5,
        NotEach = 6,
    }
}
