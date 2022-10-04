////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
///
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

namespace LaurentiuCristofor.Proteus.Common.Types
{
    /// <summary>
    /// Types of criteria for selection from a position, where a position is identified by an integer value.
    /// </summary>
    public enum PositionSelectionType
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
