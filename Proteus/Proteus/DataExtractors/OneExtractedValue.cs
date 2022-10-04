////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
///
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using LaurentiuCristofor.Proteus.Common.DataHolders;
using LaurentiuCristofor.Proteus.Common.Types;

namespace LaurentiuCristofor.Proteus.DataExtractors
{
    /// <summary>
    /// A packaging of an extracted data in addition to the original line and column strings.
    /// </summary>
    public class OneExtractedValue : ExtractedColumnStrings
    {
        /// <summary>
        /// The extracted data.
        ///
        /// This can be a column or a line.
        /// </summary>
        public IDataHolder ExtractedData { get; protected set;}

        /// <summary>
        /// The number of the extracted column, if ExtractedData contains a column;
        /// if ExtractedData contains a line, this will be set to 0.
        /// </summary>
        public int ExtractedColumnNumber { get; protected set; }

        public OneExtractedValue(
            string originalLine,
            string[] columns,
            string columnSeparator,
            IDataHolder extractedData,
            int extractedColumnNumber)
            : base(originalLine, columns, columnSeparator)
        {
            this.ExtractedData = extractedData;
            this.ExtractedColumnNumber = extractedColumnNumber;
        }

        /// <summary>
        /// This constructor is used in the special case when the extracted data is the line itself.
        ///
        /// This enables the writing of data processors that can operate on both column strings and lines,
        /// by consuming them both as instances of OneExtractedValue.
        /// </summary>
        /// <param name="originalLine">The original line that will also be stored as extracted data.</param>
        public OneExtractedValue(
            string originalLine)
            : base(originalLine, null, null)
        {
            this.ExtractedData = new StringDataHolder(originalLine);
            this.ExtractedColumnNumber = 0;
        }
    }
}
