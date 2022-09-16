////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

using LaurentiuCristofor.Proteus.Common;
using LaurentiuCristofor.Proteus.DataExtractors;
using LaurentiuCristofor.Proteus.FileOperations;

namespace LaurentiuCristofor.Proteus.DataProcessors
{
    /// <summary>
    /// A data processor that sorts the input lines by the value of a specific column.
    /// </summary>
    public class SortByColumnValueProcessor : BaseOutputProcessor, IDataProcessor<BaseOutputParameters, ParsedLine>
    {
        /// <summary>
        /// Parameters of this processor.
        /// </summary>
        protected BaseOutputParameters Parameters { get; set; }

        /// <summary>
        /// Data structure used for loading the lines before sorting them.
        /// </summary>
        protected List<Tuple<DataTypeContainer, string>> ColumnLineTuples { get; set; }

        public void Initialize(BaseOutputParameters processingParameters)
        {
            this.Parameters = processingParameters;

            this.ColumnLineTuples = new List<Tuple<DataTypeContainer, string>>();

            this.OutputWriter = new FileWriter(this.Parameters.OutputFilePath, trackProgress: true);
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

            this.ColumnLineTuples.Add(new Tuple<DataTypeContainer, string>(lineData.ExtractedData, lineData.OriginalLine));

            return true;
        }

        public override void CompleteExecution()
        {
            if (this.ColumnLineTuples == null)
            {
                throw new ProteusException("Internal error: An expected data structure has not been initialized!");
            }

            this.ColumnLineTuples.Sort();

            foreach (Tuple<DataTypeContainer, string> tuple in this.ColumnLineTuples)
            {
                this.OutputWriter.WriteLine(tuple.Item2);
            }

            base.CompleteExecution();
        }
    }
}
