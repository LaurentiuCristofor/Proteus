////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
///
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

namespace LaurentiuCristofor.Proteus.DataExtractors
{
    /// <summary>
    /// Specifies the operating parameters for the extraction of all columns as string values.
    /// </summary>
    public class ColumnStringsExtractionParameters
    {
        /// <summary>
        /// The string that should be used as column separator.
        ///
        /// The String.Split() API expects a string array, so we store our column separator in an array of one element.
        /// </summary>
        public string[] Separators { get; protected set; }

        public ColumnStringsExtractionParameters(
            string separator)
        {
            this.Separators = new string[] { separator };
        }
    }
}
