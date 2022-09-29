////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// 
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using LaurentiuCristofor.Proteus.Common;

using LaurentiuCristofor.Proteus.Common.ValueHolders;

namespace LaurentiuCristofor.Proteus.DataExtractors
{
    /// <summary>
    /// A packaging of two extracted data in addition to the original line and column strings.
    /// </summary>
    public class TwoExtractedValues : OneExtractedValue
    {
        /// <summary>
        /// A second extracted data.
        /// 
        /// This is typically a second column value.
        /// </summary>
        public IValueHolder SecondExtractedData { get; protected set; }

        public TwoExtractedValues(
            string originalLine,
            string[] columns,
            string columnSeparator,
            IValueHolder extractedData,
            int extractedColumnNumber,
            IValueHolder secondExtractedData)
            : base(originalLine, columns, columnSeparator, extractedData, extractedColumnNumber)
        {
            this.SecondExtractedData = secondExtractedData;
        }
    }
}
