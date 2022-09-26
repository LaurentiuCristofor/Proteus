////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

namespace LaurentiuCristofor.Proteus.Common.Types
{
    /// <summary>
    /// Types of string selection criteria.
    /// </summary>
    public enum StringSelectionType
    {
        NotSet = 0,

        HasLengthBetween = 1,
        HasLengthNotBetween = 2,
        Includes = 3,
        NotIncludes = 4,
        StartsWith = 5,
        NotStartsWith = 6,
        EndsWith = 7,
        NotEndsWith = 8,
        IsDemarked = 9,
        IsNotDemarked = 10,
        Equals = 11,
        NotEquals = 12,
    }
}
