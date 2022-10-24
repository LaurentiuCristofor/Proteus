////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
///
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;

using LaurentiuCristofor.Proteus.Common;
using LaurentiuCristofor.Proteus.Common.Types;
using LaurentiuCristofor.Proteus.Common.Utilities;
using LaurentiuCristofor.Proteus.DataProcessors.Parameters;
using LaurentiuCristofor.Proteus.FileOperations;

namespace LaurentiuCristofor.Proteus.DataProcessors
{
    /// <summary>
    /// A data processor that sorts the input lines.
    /// Uses the specified custom sorting algorithm or the default sorting algorithm if no custom sorting algorithm is specified.
    /// </summary>
    public class SortProcessor : BaseOutputProcessor, IDataProcessor<OutputOperationParameters<SortingAlgorithmType>, string>
    {
        protected SortingAlgorithmType SortingType { get; set; }

        /// <summary>
        /// Data structure used for loading the lines before sorting them.
        /// </summary>
        protected List<string> Lines { get; set; }

        public void Initialize(OutputOperationParameters<SortingAlgorithmType> processingParameters)
        {
            SortingType = processingParameters.OperationType;

            Lines = new List<string>();

            OutputWriter = new FileWriter(processingParameters.OutputFilePath, trackProgress: true);
        }

        public bool Execute(ulong lineNumber, string line)
        {
            Lines.Add(line);

            return true;
        }

        public override void CompleteExecution()
        {
            if (Lines == null)
            {
                throw new ProteusException("Internal error: An expected data structure has not been initialized!");
            }

            Timer timer = new Timer($"\n{Constants.Messages.SortingStart}", Constants.Messages.SortingEnd, countFinalLineEndings: 0);
            CustomSorting.Sort(Lines, SortingType);
            timer.StopAndReport();

            foreach (string line in Lines)
            {
                OutputWriter.WriteLine(line);
            }

            base.CompleteExecution();
        }
    }
}
