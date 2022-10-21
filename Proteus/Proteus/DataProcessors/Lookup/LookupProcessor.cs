////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
///
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
    /// A data processor that looks up a value in a data structure according to a selection criteria,
    /// to decide whether to output the line or not.
    /// </summary>
    public class LookupProcessor : BaseOutputProcessor, IDataLookupProcessor<OutputOperationParameters<LookupType>, HashSet<IDataHolder>, OneExtractedValue>
    {
        protected LookupType LookupType { get; set; }

        /// <summary>
        /// The lookup data structure used to perform the operation.
        /// </summary>
        protected HashSet<IDataHolder> LookupSet { get; set; }

        public void Initialize(OutputOperationParameters<LookupType> processingParameters)
        {
            LookupType = processingParameters.OperationType;

            OutputWriter = new FileWriter(processingParameters.OutputFilePath);
        }

        public void AddLookupDataStructure(HashSet<IDataHolder> lookupSet)
        {
            LookupSet = lookupSet;
        }

        public bool Execute(ulong lineNumber, OneExtractedValue lineData)
        {
            bool isDataIncluded = LookupSet.Contains(lineData.ExtractedData);

            bool shouldOutputLine;
            switch (LookupType)
            {
                case LookupType.Included:
                    shouldOutputLine = isDataIncluded;
                    break;

                case LookupType.NotIncluded:
                    shouldOutputLine = !isDataIncluded;
                    break;

                default:
                    throw new ProteusException($"Internal error: Proteus is not handling lookup type '{LookupType}'!");
            }

            if (shouldOutputLine)
            {
                OutputWriter.WriteLine(lineData.OriginalLine);
            }

            return true;
        }
    }
}
