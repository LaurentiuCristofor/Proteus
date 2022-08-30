////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;

using LaurentiuCristofor.Proteus.Common;

namespace LaurentiuCristofor.Proteus.DataExtractors
{
    /// <summary>
    /// A class for providing validation methods for data extractors.
    /// </summary>
    public static class DataExtractorValidation
    {
        public static void ValidateLine(string line)
        {
            if (String.IsNullOrEmpty(line))
            {
                throw new ProteusException("Internal error: A data extractor was called with a null or empty line!");
            }
        }
    }
}
