////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

namespace LaurentiuCristofor.Proteus.Common
{
    /// <summary>
    /// Types of string editing. 
    /// </summary>
    public enum StringEditType
    {
        NotSet = 0,

        Rewrite = 1,
        Invert = 2,
        Uppercase = 3,
        Lowercase = 4,

        TrimStart = 10,
        TrimEnd = 11,
        Trim = 12,
        TrimCharsStart = 13,
        TrimCharsEnd = 14,
        TrimChars = 15,
        PadLeft = 16,
        PadRight = 17,

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

        DeleteContentBeforeMarker = 100,
        DeleteContentAfterMarker = 101,
        KeepContentBeforeMarker = 102,
        KeepContentAfterMarker = 103,
        InsertContentBeforeMarker = 104,
        InsertContentAfterMarker = 105,

        DeleteContentBeforeLastMarker = 110,
        DeleteContentAfterLastMarker = 111,
        KeepContentBeforeLastMarker = 112,
        KeepContentAfterLastMarker = 113,
        InsertContentBeforeLastMarker = 114,
        InsertContentAfterLastMarker = 115,

        DeleteContentBetweenMarkers = 140,
        KeepContentBetweenMarkers = 141,
        KeepContentOutsideMarkers = 142,

        DeleteContentBetweenLastMarkers = 150,
        KeepContentBetweenLastMarkers = 151,
        KeepContentOutsideLastMarkers = 152,

        DeleteContentBetweenInnermostMarkers = 160,
        KeepContentBetweenInnermostMarkers = 161,
        KeepContentOutsideInnermostMarkers = 162,

        DeleteContentBetweenOutermostMarkers = 170,
        KeepContentBetweenOutermostMarkers = 171,
        KeepContentOutsideOutermostMarkers = 172,

        Set = 200,
        SetIfEquals = 201,
    }
}
