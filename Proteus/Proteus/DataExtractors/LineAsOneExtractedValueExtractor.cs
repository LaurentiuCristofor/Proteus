////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
///
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using LaurentiuCristofor.Proteus.Common;

namespace LaurentiuCristofor.Proteus.DataExtractors
{
    /// <summary>
    /// An extractor that packages each input line into a OneExtractedValue instance.
    /// </summary>
    public class LineAsOneExtractedValueExtractor : IDataExtractor<Unused, OneExtractedValue>
    {
        public void Initialize(Unused unusedExtractionParameters)
        {
        }

        public OneExtractedValue ExtractData(ulong lineNumber, string line)
        {
            return new OneExtractedValue(line);
        }
    }
}
