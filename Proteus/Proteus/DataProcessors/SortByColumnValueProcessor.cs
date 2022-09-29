////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// 
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

using LaurentiuCristofor.Proteus.Common;
using LaurentiuCristofor.Proteus.Common.DataHolders;
using LaurentiuCristofor.Proteus.DataExtractors;
using LaurentiuCristofor.Proteus.DataProcessors.Parameters;
using LaurentiuCristofor.Proteus.FileOperations;

namespace LaurentiuCristofor.Proteus.DataProcessors
{
    /// <summary>
    /// A data processor that sorts the input lines by the value of a specific column.
    /// </summary>
    public class SortByColumnValueProcessor : BaseOutputProcessor, IDataProcessor<BaseOutputParameters, ParsedLine>
    {
        protected BaseOutputParameters Parameters { get; set; }

        /// <summary>
        /// Data structure used for loading the lines before sorting them.
        /// </summary>
        protected List<Tuple<IDataHolder, string>> ColumnLineTuples { get; set; }

        public void Initialize(BaseOutputParameters processingParameters)
        {
            this.Parameters = processingParameters;

            this.ColumnLineTuples = new List<Tuple<IDataHolder, string>>();

            this.OutputWriter = new FileWriter(this.Parameters.OutputFilePath, trackProgress: true);
        }

        public bool Execute(ulong lineNumber, ParsedLine lineData)
        {
            this.ColumnLineTuples.Add(new Tuple<IDataHolder, string>(lineData.ExtractedData, lineData.OriginalLine));

            return true;
        }

        public override void CompleteExecution()
        {
            if (this.ColumnLineTuples == null)
            {
                throw new ProteusException("Internal error: An expected data structure has not been initialized!");
            }

            OutputInterface.Log("\nSorting...");

            this.ColumnLineTuples.Sort();

            OutputInterface.Log("done!");

            foreach (Tuple<IDataHolder, string> tuple in this.ColumnLineTuples)
            {
                this.OutputWriter.WriteLine(tuple.Item2);
            }

            base.CompleteExecution();
        }
    }
}
