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
    /// to decide whether to output the line or not.
    /// </summary>
    public class ColumnValueSelectProcessor : BaseOutputProcessor, IDataProcessor<OperationTypeParameters<ComparisonType>, ParsedLine>
    {
        /// <summary>
        /// Parameters of this operation.
        /// </summary>
        protected OperationTypeParameters<ComparisonType> Parameters { get; set; }

        public void Initialize(OperationTypeParameters<ComparisonType> processingParameters)
        {
            this.Parameters = processingParameters;

            this.OutputWriter = new TextFileWriter(this.Parameters.OutputFilePath);
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

            if (String.IsNullOrEmpty(lineData.OriginalLine))
            {
                throw new ProteusException("ColumnValueSelectProcessor was called on a null or empty line!");
            }

            // Perform the comparison to decide whether to output the line.
            //
            if (lineData.ExtractedData.Compare(this.Parameters.OperationType, this.Parameters.FirstArgument, this.Parameters.SecondArgument))
            {
                this.OutputWriter.WriteLine(lineData.OriginalLine);
            }

            return true;
        }
    }
}
