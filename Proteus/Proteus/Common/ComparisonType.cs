////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

namespace LaurentiuCristofor.Proteus.Common
{
    /// <summary>
    /// Types of comparisons.
    /// </summary>
    public enum ComparisonType
    {
        NotSet = 0,

        LessThan = 1,
        LessThanOrEqual = 2,
        Equal = 3,
        GreaterThanOrEqual = 4,
        GreaterThan = 5,
        NotEqual = 6,
        Between = 7,
        StrictlyBetween = 8,
        NotBetween = 9,
        NotStrictlyBetween = 10,
    }
}
