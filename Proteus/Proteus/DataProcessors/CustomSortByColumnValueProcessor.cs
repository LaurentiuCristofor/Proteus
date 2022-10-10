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
using LaurentiuCristofor.Proteus.Common.Types;
using LaurentiuCristofor.Proteus.Common.Utilities;
using LaurentiuCristofor.Proteus.DataExtractors;
using LaurentiuCristofor.Proteus.DataProcessors.Parameters;
using LaurentiuCristofor.Proteus.FileOperations;

namespace LaurentiuCristofor.Proteus.DataProcessors
{
    /// <summary>
    /// A data processor that sorts the input lines by the value of a specific column
    /// using a custom sorting algorithm.
    /// </summary>
    public class CustomSortByColumnValueProcessor : BaseOutputProcessor, IDataProcessor<OutputOperationParameters<SortingAlgorithmType>, OneExtractedValue>
    {
        protected OutputOperationParameters<SortingAlgorithmType> Parameters { get; set; }

        /// <summary>
        /// Data structure used for loading the lines before sorting them.
        /// </summary>
        protected List<Tuple<IDataHolder, string>> ColumnLineTuples { get; set; }

        public void Initialize(OutputOperationParameters<SortingAlgorithmType> processingParameters)
        {
            this.Parameters = processingParameters;

            this.ColumnLineTuples = new List<Tuple<IDataHolder, string>>();

            this.OutputWriter = new FileWriter(this.Parameters.OutputFilePath, trackProgress: true);
        }

        public bool Execute(ulong lineNumber, OneExtractedValue lineData)
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

            Timer timer = new Timer($"\n{Constants.Messages.SortingStart}", Constants.Messages.SortingEnd, countFinalLineEndings: 0);
            CustomSorting.Sort<Tuple<IDataHolder, string>>(this.ColumnLineTuples, this.Parameters.OperationType);
            timer.StopAndReport();

            foreach (Tuple<IDataHolder, string> tuple in this.ColumnLineTuples)
            {
                this.OutputWriter.WriteLine(tuple.Item2);
            }

            base.CompleteExecution();
        }
    }
}
