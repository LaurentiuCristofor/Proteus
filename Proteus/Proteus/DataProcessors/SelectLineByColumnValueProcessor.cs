////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
///
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using LaurentiuCristofor.Proteus.Common.DataHolders;
using LaurentiuCristofor.Proteus.Common.Types;
using LaurentiuCristofor.Proteus.DataExtractors;
using LaurentiuCristofor.Proteus.DataProcessors.Parameters;
using LaurentiuCristofor.Proteus.FileOperations;

namespace LaurentiuCristofor.Proteus.DataProcessors
{
    /// <summary>
    /// A data processor that checks the value of a column against a selection criterion,
    /// to decide whether to output the line or not.
    /// </summary>
    public class SelectLineByColumnValueProcessor : BaseOutputProcessor, IDataProcessor<OutputOperationParameters<ComparisonType>, OneExtractedValue>
    {
        protected ComparisonType ComparisonType { get; set; }

        /// <summary>
        /// First comparison argument, if expected.
        /// </summary>
        protected IDataHolder FirstArgument { get; set; }

        /// <summary>
        /// Second comparison argument, if expected.
        /// </summary>
        protected IDataHolder SecondArgument { get; set; }

        public void Initialize(OutputOperationParameters<ComparisonType> processingParameters)
        {
            this.ComparisonType = processingParameters.OperationType;

            if (processingParameters.FirstArgument != null)
            {
                this.FirstArgument = DataHolderOperations.BuildAndCheckDataHolder(DataType.Integer, processingParameters.FirstArgument);

                if (processingParameters.SecondArgument != null)
                {
                    this.SecondArgument = DataHolderOperations.BuildAndCheckDataHolder(DataType.Integer, processingParameters.SecondArgument);
                }
            }

            this.OutputWriter = new FileWriter(processingParameters.OutputFilePath);
        }

        public bool Execute(ulong lineNumber, OneExtractedValue lineData)
        {
            // Perform the comparison to decide whether to output the line.
            //
            if (DataHolderOperations.Compare(lineData.ExtractedData, this.ComparisonType, this.FirstArgument, this.SecondArgument))
            {
                this.OutputWriter.WriteLine(lineData.OriginalLine);
            }

            return true;
        }
    }
}
