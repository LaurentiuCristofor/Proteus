////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
///
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using LaurentiuCristofor.Proteus.Common;
using LaurentiuCristofor.Proteus.DataExtractors;
using LaurentiuCristofor.Proteus.DataProcessors.Parameters;
using LaurentiuCristofor.Proteus.FileOperations;

namespace LaurentiuCristofor.Proteus.DataProcessors
{
    /// <summary>
    /// A data processor that expects as input a file sorted by a primary column
    /// and will output consecutive rows having the same primary column value
    /// but differing in the value of another 'state' column.
    /// </summary>
    public class FindStateTransitionsProcessor : BaseOutputProcessor, IDataProcessor<BaseOutputParameters, TwoExtractedValues>
    {
        /// <summary>
        /// A copy of the data seen for the previous line.
        /// </summary>
        protected TwoExtractedValues PreviousLineData { get; set; }

        public void Initialize(BaseOutputParameters processingParameters)
        {
            OutputWriter = new FileWriter(processingParameters.OutputFilePath);
        }

        public bool Execute(ulong lineNumber, TwoExtractedValues lineData)
        {
            // Verify that the input file is sorted on the primary column.
            //
            if (PreviousLineData != null && lineData.ExtractedData.CompareTo(PreviousLineData.ExtractedData) < 0)
            {
                throw new ProteusException($"Input file is not sorted as expected! Value '{lineData.ExtractedData}' succeeds value '{PreviousLineData.ExtractedData}'.");
            }

            // If we find two consecutive rows with the same primary column data, but with different secondary column data,
            // output the pair of rows.
            //
            if (PreviousLineData != null
                && lineData.ExtractedData.Equals(PreviousLineData.ExtractedData)
                && !lineData.SecondExtractedData.Equals(PreviousLineData.SecondExtractedData))
            {
                OutputWriter.WriteLine(PreviousLineData.OriginalLine);
                OutputWriter.WriteLine(lineData.OriginalLine);
            }

            PreviousLineData = lineData;

            return true;
        }
    }
}
