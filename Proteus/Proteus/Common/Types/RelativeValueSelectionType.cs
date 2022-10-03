////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// 
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

namespace LaurentiuCristofor.Proteus.Common.Types
{
    /// <summary>
    /// Types of criteria for selecting based on the relationship between different values.
    /// </summary>
    public enum RelativeValueSelectionType
    {
        NotSet = 0,

        First = 1,
        NotFirst = 2,
        Last = 3,
        NotLast = 4,
    }
}
