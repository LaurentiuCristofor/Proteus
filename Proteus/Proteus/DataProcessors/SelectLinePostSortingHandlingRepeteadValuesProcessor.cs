﻿////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;

using LaurentiuCristofor.Proteus.Common;
using LaurentiuCristofor.Proteus.Common.DataHolders;
using LaurentiuCristofor.Proteus.Common.Types;
using LaurentiuCristofor.Proteus.DataExtractors;
using LaurentiuCristofor.Proteus.DataProcessors.Parameters;
using LaurentiuCristofor.Proteus.FileOperations;

namespace LaurentiuCristofor.Proteus.DataProcessors
{
    /// <summary>
    /// A data processor that checks a value to see if it's a repetition of a previously processed value,
    /// to decide whether to output the line or not.
    /// </summary>
    public class SelectLinePostSortingHandlingRepeteadValuesProcessor : BaseOutputProcessor, IDataProcessor<OperationOutputParameters<RepetitionHandlingType>, ParsedLine>
    {
        protected OperationOutputParameters<RepetitionHandlingType> Parameters { get; set; }

        /// <summary>
        /// Last seen data.
        /// </summary>
        protected IDataHolder LastSeenData { get; set; }

        public void Initialize(OperationOutputParameters<RepetitionHandlingType> processingParameters)
        {
            this.Parameters = processingParameters;

            this.OutputWriter = new FileWriter(this.Parameters.OutputFilePath);
        }

        public bool Execute(ulong lineNumber, ParsedLine lineData)
        {
            DataProcessorValidation.ValidateExtractedDataIsString(lineData);

            IDataHolder data = lineData.ExtractedData;
            bool isRepeatedData = false;

            // Verify that the input file is sorted on the extracted data.
            //
            if (data.CompareTo(this.LastSeenData) < 0)
            {
                throw new ProteusException($"Input file is not sorted as expected! Value '{data}' succeeds value '{this.LastSeenData}'.");
            }

            // Compare data with our last seen data;
            //
            if (data.Equals(this.LastSeenData))
            {
                isRepeatedData = true;
            }
            else
            {
                this.LastSeenData = data;
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