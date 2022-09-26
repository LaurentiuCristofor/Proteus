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

namespace LaurentiuCristofor.Proteus.DataProcessors.Lookup
{
    /// <summary>
    /// A data processor that looks up an IDataType in a data structure according to a selection criteria,
    /// to decide whether to output the line or not.
    /// </summary>
    public class LookupProcessor : BaseOutputProcessor, IDataLookupProcessor<OperationOutputParameters<LookupType>, HashSet<IDataHolder>, ParsedLine>
    {
        protected OperationOutputParameters<LookupType> Parameters { get; set; }

        /// <summary>
        /// The lookup data structure used to perform the operation.
        /// </summary>
        protected HashSet<IDataHolder> LookupSet { get; set; }

        public void Initialize(OperationOutputParameters<LookupType> processingParameters)
        {
            this.Parameters = processingParameters;

            this.OutputWriter = new FileWriter(this.Parameters.OutputFilePath);
        }

        public void AddLookupDataStructure(HashSet<IDataHolder> lookupSet)
        {
            this.LookupSet = lookupSet;
        }

        public bool Execute(ulong lineNumber, ParsedLine lineData)
        {
            DataProcessorValidation.ValidateExtractedDataIsString(lineData);

            bool isDataIncluded = this.LookupSet.Contains(lineData.ExtractedData);

            bool shouldOutputLine;
            switch (this.Parameters.OperationType)
            {
                case LookupType.Included:
                    shouldOutputLine = isDataIncluded;
                    break;

                case LookupType.NotIncluded:
                    shouldOutputLine = !isDataIncluded;
                    break;

                default:
                    throw new ProteusException($"Internal error: Proteus is not handling lookup type '{this.Parameters.OperationType}'!");
            }

            if (shouldOutputLine)
            {
                this.OutputWriter.WriteLine(lineData.OriginalLine);
            }

            return true;
        }
    }
}