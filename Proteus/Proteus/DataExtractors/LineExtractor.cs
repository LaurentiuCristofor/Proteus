////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using LaurentiuCristofor.Proteus.Common;

namespace LaurentiuCristofor.Proteus.DataExtractors
{
    /// <summary>
    /// A pass-through line extractor.
    /// </summary>
    public class LineExtractor : IDataExtractor<UnusedType, string>
    {
        public void Initialize(UnusedType unusedExtractionParameters)
        {
        }

        public string ExtractData(ulong lineNumber, string inputLine)
        {
            return inputLine;
        }
    }
}
