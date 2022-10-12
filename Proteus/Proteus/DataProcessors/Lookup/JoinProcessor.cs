﻿////////////////////////////////////////////////////////////////////////////////////////////////////
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
    /// A data processor that looks up a string in a data structure,
    /// to find a line to join with the currently processed line.
    /// </summary>
    public class JoinProcessor : BaseOutputProcessor, IDataLookupProcessor<OutputExtraOperationParameters<JoinType>, Dictionary<IDataHolder, List<string>>, OneExtractedValue>
    {
        protected JoinType JoinType { get; set; }

        /// <summary>
        /// The default value to use for LeftOuterJoin, when there is no matching row "on the right".
        /// </summary>
        protected string OuterJoinDefaultValue { get; set; }

        /// <summary>
        /// The lookup data structure used to perform the operation.
        /// </summary>
        protected Dictionary<IDataHolder, List<string>> LookupDictionary { get; set; }

        public void Initialize(OutputExtraOperationParameters<JoinType> processingParameters)
        {
            this.JoinType = processingParameters.OperationType;
            this.OuterJoinDefaultValue = processingParameters.FirstArgument;

            this.OutputWriter = new FileWriter(processingParameters.OutputFilePath);
        }

        public void AddLookupDataStructure(Dictionary<IDataHolder, List<string>> lookupDictionary)
        {
            this.LookupDictionary = lookupDictionary;
        }

        public bool Execute(ulong lineNumber, OneExtractedValue lineData)
        {
            IDataHolder lineKey = lineData.ExtractedData;

            // The case where we find a match in the lookup dictionary
            // is handled in the same way for all join types.
            //
            if (this.LookupDictionary.ContainsKey(lineKey))
            {
                List<string> joinLines = this.LookupDictionary[lineKey];

                foreach (string joinLine in joinLines)
                {
                    // Special case: if there is no join line (the line in the join file only contained the join key and no other columns),
                    // then we will still output the current line as is.
                    //
                    string outputLine = lineData.OriginalLine;

                    // Join the line with our line.
                    //
                    if (joinLine != null)
                    {
                        outputLine += lineData.ColumnSeparator + joinLine;
                    }

                    this.OutputWriter.WriteLine(outputLine);
                }
            }
            else
            {
                // In case of no match, we process according to the join type.
                //
                switch (this.JoinType)
                {
                    case JoinType.Inner:
                        // We don't output anything for an inner join.
                        //
                        break;

                    case JoinType.LeftOuter:
                        // For a left outer join, we'll output the first line combined with the string provided by the user.
                        //
                        string outputLine = lineData.OriginalLine + lineData.ColumnSeparator + this.OuterJoinDefaultValue;
                        this.OutputWriter.WriteLine(outputLine);
                        break;

                    default:
                        throw new ProteusException($"Internal error: Proteus is not handling join type '{this.JoinType}'!");
                }
            }

            return true;
        }
    }
}
