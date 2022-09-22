﻿////////////////////////////////////////////////////////////////////////////////////////////////////
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
    /// A data processor that looks up a string in a data structure,
    /// to find a line to join with the currently processed line.
    /// </summary>
    public class JoinProcessor : BaseOutputProcessor, IDataLookupProcessor<OperationTypeOutputParameters<JoinType>, Dictionary<string, string>, ParsedLine>
    {
        /// <summary>
        /// Parameters of this operation.
        /// </summary>
        protected OperationTypeOutputParameters<JoinType> Parameters { get; set; }

        /// <summary>
        /// The lookup data structure used to perform the operation.
        /// </summary>
        protected Dictionary<string, string> LookupDictionary { get; set; }

        public void Initialize(OperationTypeOutputParameters<JoinType> processingParameters)
        {
            this.Parameters = processingParameters;

            this.OutputWriter = new FileWriter(this.Parameters.OutputFilePath);
        }

        public void AddLookupDataStructure(Dictionary<string, string> lookupDictionary)
        {
            this.LookupDictionary = lookupDictionary;
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
            DataProcessorValidation.ValidateColumnInformation(lineData);

            string data = lineData.ExtractedData.ToString();

            // The case where we find a match in the lookup dictionary
            // is handled in the same way for all join types.
            //
            if (this.LookupDictionary.ContainsKey(data))
            {
                string otherLine = this.LookupDictionary[data];
                string outputLine = lineData.OriginalLine + lineData.ColumnSeparator + otherLine;
                this.OutputWriter.WriteLine(outputLine);
            }
            else
            {
                // In case of no match, we process according to the join type.
                //
                switch (this.Parameters.OperationType)
                {
                    case JoinType.Inner:
                        // We don't output anything for an inner join.
                        //
                        break;

                    case JoinType.LeftOuter:
                        // For a left outer join, we'll output the first line combined with the string provided by the user.
                        //
                        string outputLine = lineData.OriginalLine + lineData.ColumnSeparator + this.Parameters.FirstArgument;
                        this.OutputWriter.WriteLine(outputLine);
                        break;

                    default:
                        throw new ProteusException($"Internal error: Proteus is not handling join type '{this.Parameters.OperationType}'!");
                }
            }

            return true;
        }
    }
}