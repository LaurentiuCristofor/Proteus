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
    /// A data processor that checks the column count against a selection criterion,
    /// to decide whether to output the line or not.
    /// </summary>
    public class SelectLineByColumnCountProcessor : BaseOutputProcessor, IDataProcessor<OperationTypeOutputParameters<ComparisonType>, ParsedLine>
    {
        /// <summary>
        /// Parameters of this operation.
        /// </summary>
        protected OperationTypeOutputParameters<ComparisonType> Parameters { get; set; }

        public void Initialize(OperationTypeOutputParameters<ComparisonType> processingParameters)
        {
            this.Parameters = processingParameters;

            this.OutputWriter = new FileWriter(this.Parameters.OutputFilePath);
        }

        public bool Execute(ulong lineNumber, ParsedLine lineData)
        {
            DataProcessorValidation.ValidateLineData(lineData);
            DataProcessorValidation.ValidateColumnInformation(lineData);

            // Package column count in a DataTypeContainer.
            //
            DataTypeContainer columnCountContainer = new DataTypeContainer(lineData.Columns.Length);

            // Perform the comparison to decide whether to output the line.
            //
            if (columnCountContainer.Compare(this.Parameters.OperationType, this.Parameters.FirstArgument, this.Parameters.SecondArgument))
            {
                this.OutputWriter.WriteLine(lineData.OriginalLine);
            }

            return true;
        }
    }
}
