////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;

using LaurentiuCristofor.Proteus.Common;
using LaurentiuCristofor.Proteus.DataExtractors;
using LaurentiuCristofor.Proteus.FileOperations;

namespace LaurentiuCristofor.Proteus.DataProcessors
{
    /// <summary>
    /// A data processor that checks a value to see if it's a repetition of a previously processed value,
    /// to decide whether to output the line or not.
    /// </summary>
    public class SelectLineHandlingRepeteadValuesProcessor : BaseOutputProcessor, IDataProcessor<OperationOutputParameters<RepetitionHandlingType>, ParsedLine>
    {
        protected OperationOutputParameters<RepetitionHandlingType> Parameters { get; set; }

        /// <summary>
        /// Set of values seen so far.
        /// </summary>
        protected HashSet<DataTypeContainer> SetValues { get; set; }

        public void Initialize(OperationOutputParameters<RepetitionHandlingType> processingParameters)
        {
            this.Parameters = processingParameters;

            this.SetValues = new HashSet<DataTypeContainer>();

            this.OutputWriter = new FileWriter(this.Parameters.OutputFilePath);
        }

        public bool Execute(ulong lineNumber, ParsedLine lineData)
        {
            DataProcessorValidation.ValidateExtractedDataIsString(lineData);

            DataTypeContainer data = lineData.ExtractedData;
            bool isRepeatedData = false;

            // Lookup data in our set;
            //
            if (this.SetValues.Contains(data))
            {
                isRepeatedData = true;
            }
            else
            {
                this.SetValues.Add(data);
            }

            // Determine whether to output the line based on the handling type.
            //
            bool shouldOutputLine;
            switch (this.Parameters.OperationType)
            {
                case RepetitionHandlingType.Pick:
                    shouldOutputLine = isRepeatedData;
                    break;

                case RepetitionHandlingType.Skip:
                    shouldOutputLine = !isRepeatedData;
                    break;

                default:
                    throw new ProteusException($"Internal error: Proteus is not handling repetition handling type '{this.Parameters.OperationType}'!");
            }

            if (shouldOutputLine)
            {
                this.OutputWriter.WriteLine(lineData.OriginalLine);
            }

            return true;
        }
    }
}
