////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;

using LaurentiuCristofor.Proteus.Common;
using LaurentiuCristofor.Proteus.DataExtractors;
using LaurentiuCristofor.Proteus.FileOperations;

namespace LaurentiuCristofor.Proteus.DataProcessors.Lookup
{
    /// <summary>
    /// A data processor that looks up a string in a data structure according to a selection criteria,
    /// to decide whether to output the line or not.
    /// </summary>
    public class LookupStringProcessor : BaseOutputProcessor, IDataLookupProcessor<OperationOutputParameters<LookupType>, HashSet<string>, ParsedLine>
    {
        protected OperationOutputParameters<LookupType> Parameters { get; set; }

        /// <summary>
        /// The lookup data structure used to perform the operation.
        /// </summary>
        protected HashSet<string> LookupSet { get; set; }

        public void Initialize(OperationOutputParameters<LookupType> processingParameters)
        {
            this.Parameters = processingParameters;

            this.OutputWriter = new FileWriter(this.Parameters.OutputFilePath);
        }

        public void AddLookupDataStructure(HashSet<string> lookupSet)
        {
            this.LookupSet = lookupSet;
        }

        public bool Execute(ulong lineNumber, ParsedLine lineData)
        {
            // We may not always be able to extract a column.
            // Ignore these cases; the extractor will already have printed a warning message.
            //
            if (lineData == null)
            {
                return true;
            }

            DataProcessorValidation.ValidateExtractedDataIsString(lineData);

            string data = lineData.ExtractedData.ToString();

            bool isDataIncluded = this.LookupSet.Contains(data);

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
