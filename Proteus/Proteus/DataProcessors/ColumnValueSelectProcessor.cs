////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;

using LaurentiuCristofor.Proteus.Common;
using LaurentiuCristofor.Proteus.DataExtractors;
using LaurentiuCristofor.Proteus.FileOperations;

namespace LaurentiuCristofor.Proteus.DataProcessors
{
    /// <summary>
    /// A data processor that checks the value of a column against a selection criterion,
    /// to decide whether to output the row or not.
    /// </summary>
    public class ColumnValueSelectProcessor : BaseOutputProcessor, IDataProcessor<OperationTypeParameters<ComparisonType>, Tuple<LineParts, DataTypeContainer>>
    {
        /// <summary>
        /// Parameters of this operation.
        /// </summary>
        private OperationTypeParameters<ComparisonType> Parameters { get; set; }

        public void Initialize(OperationTypeParameters<ComparisonType> processingParameters)
        {
            this.Parameters = processingParameters;

            this.OutputWriter = new TextFileWriter(this.Parameters.OutputFilePath);
        }

        public bool Execute(ulong lineNumber, Tuple<LineParts, DataTypeContainer> inputData)
        {
            // We may not always be able to extract a column.
            // Ignore these cases; the extractor will already have printed a warning message.
            //
            if (inputData == null)
            {
                return true;
            }

            // Perform the comparison to decide whether to output the row.
            //
            if (inputData.Item2.Compare(this.Parameters.OperationType, this.Parameters.FirstArgument, this.Parameters.SecondArgument))
            {
                this.OutputWriter.WriteLine(inputData.Item1.OriginalLine);
            }

            return true;
        }
    }
}
