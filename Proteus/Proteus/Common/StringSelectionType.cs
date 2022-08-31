////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

namespace LaurentiuCristofor.Proteus.Common
{
    /// <summary>
    /// Types of string selections supported by Proteus.
    /// </summary>
    public enum StringSelectionType
    {
        NotSet = 0,

        LengthBetween = 1,
        LengthNotBetween = 2,
        Including = 3,
        NotIncluding = 4,
        StartingWith = 5,
        EndingWith = 6,
    }
}
