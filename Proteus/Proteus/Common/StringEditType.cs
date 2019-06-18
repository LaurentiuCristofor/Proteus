////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

namespace LaurentiuCristofor.Proteus.Common
{
    /// <summary>
    /// Types of string operations supported by Proteus. 
    /// </summary>
    public enum StringEditType
    {
        NotSet = 0,

        Rewrite = 1,
        PrefixLineNumbers = 2,
        Invert = 3,
        AddPrefix = 4,
        AddSuffix = 5,
        DeletePrefix = 6,
        DeleteSuffix = 7,
        DeleteContent = 8,
        DeleteContentBeforeMarker = 9,
        DeleteContentAfterMarker = 10,
        InsertContentBeforeMarker = 11,
        InsertContentAfterMarker = 12,
        InsertContentAtIndex = 13,
        ReplaceContent = 14,
        Uppercase = 15,
        Lowercase = 16,
    }
}
