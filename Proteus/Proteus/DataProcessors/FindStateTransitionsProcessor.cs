////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
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
    /// A data processor that expects as input a file sorted by a primary column and will output consecutive rows that differ in the value of a specified 'state' column.
    /// </summary>
    public class FindStateTransitionsProcessor : BaseOutputProcessor, IDataProcessor<BaseOutputParameters, ParsedLine>
    {
        protected BaseOutputParameters Parameters { get; set; }

        /// <summary>
        /// A copy of the data seen for the previous line.
        /// </summary>
        protected ParsedLine PreviousLineData { get; set; }

        public void Initialize(BaseOutputParameters processingParameters)
        {
            this.Parameters = processingParameters;

            this.OutputWriter = new FileWriter(this.Parameters.OutputFilePath);
        }

        public bool Execute(ulong lineNumber, ParsedLine lineData)
        {
            DataProcessorValidation.ValidateSecondExtractedData(lineData);

            // Verify that the input file is sorted on the primary column.
            //
            if (this.PreviousLineData != null && lineData.ExtractedData.CompareTo(this.PreviousLineData.ExtractedData) < 0)
            {
                throw new ProteusException($"Input file is not sorted as expected! Value '{lineData.ExtractedData}' succeeds value '{this.PreviousLineData.ExtractedData}'.");
            }

            // If we find two consecutive rows with the same primary column data, but with different secondary column data,
            // output the pair of rows.
            //
            if (this.PreviousLineData != null
                && lineData.ExtractedData.Equals(this.PreviousLineData.ExtractedData)
                && !lineData.SecondExtractedData.Equals(this.PreviousLineData.SecondExtractedData))
            {
                this.OutputWriter.WriteLine(this.PreviousLineData.OriginalLine);
                this.OutputWriter.WriteLine(lineData.OriginalLine);
            }

            this.PreviousLineData = lineData;

            return true;
        }
    }
}
