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
        Uppercase = 2,
        Lowercase = 3,
        Invert = 4,

        PrefixLineNumbers = 30,
        AddPrefix = 31,
        AddSuffix = 32,
        DeletePrefix = 33,
        DeleteSuffix = 34,
        DeleteFirstCharacters = 35,
        DeleteLastCharacters = 36,
        KeepFirstCharacters = 37,
        KeepLastCharacters = 38,

        DeleteContentAtIndex = 60,
        KeepContentAtIndex = 61,
        InsertContentAtIndex = 62,

        ReplaceContent = 90,
        DeleteContentBeforeMarker = 91,
        DeleteContentAfterMarker = 92,
        KeepContentBeforeMarker = 93,
        KeepContentAfterMarker = 94,
        InsertContentBeforeMarker = 95,
        InsertContentAfterMarker = 96,
        DeleteContentBetweenMarkers = 97,
        KeepContentBetweenMarkers = 98,
        KeepContentOutsideMarkers = 99,
    }
}
