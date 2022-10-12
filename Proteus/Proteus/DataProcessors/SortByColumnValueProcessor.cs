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
using LaurentiuCristofor.Proteus.Common.Utilities;
using LaurentiuCristofor.Proteus.DataExtractors;
using LaurentiuCristofor.Proteus.DataProcessors.Parameters;
using LaurentiuCristofor.Proteus.FileOperations;

namespace LaurentiuCristofor.Proteus.DataProcessors
{
    /// <summary>
    /// A data processor that sorts the input lines by the value of a specific column.
    /// </summary>
    public class SortByColumnValueProcessor : BaseOutputProcessor, IDataProcessor<BaseOutputParameters, OneExtractedValue>
    {
        /// <summary>
        /// Data structure used for loading the lines before sorting them.
        /// </summary>
        protected List<DataPair<IDataHolder, string>> ColumnLinePairs { get; set; }

        public void Initialize(BaseOutputParameters processingParameters)
        {
            this.ColumnLinePairs = new List<DataPair<IDataHolder, string>>();

            this.OutputWriter = new FileWriter(processingParameters.OutputFilePath, trackProgress: true);
        }

        public bool Execute(ulong lineNumber, OneExtractedValue lineData)
        {
            this.ColumnLinePairs.Add(new DataPair<IDataHolder, string>(lineData.ExtractedData, lineData.OriginalLine));

            return true;
        }

        public override void CompleteExecution()
        {
            if (this.ColumnLinePairs == null)
            {
                throw new ProteusException("Internal error: An expected data structure has not been initialized!");
            }

            Timer timer = new Timer($"\n{Constants.Messages.SortingStart}", Constants.Messages.SortingEnd, countFinalLineEndings: 0);
            this.ColumnLinePairs.Sort();
            timer.StopAndReport();

            foreach (DataPair<IDataHolder, string> dataPair in this.ColumnLinePairs)
            {
                this.OutputWriter.WriteLine(dataPair.SecondData);
            }

            base.CompleteExecution();
        }
    }
}
