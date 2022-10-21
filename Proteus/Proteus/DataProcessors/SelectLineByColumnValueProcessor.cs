////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
///
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using LaurentiuCristofor.Proteus.Common;
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
    ///
    /// OutputExtraOperationParameters is expected to contain:
    /// DataHolderParameters[0] - first comparison argument
    /// DataHolderParameters[1] - second comparison argument (if required)
    /// </summary>
    public class SelectLineByColumnValueProcessor : BaseOutputProcessor, IDataProcessor<OutputExtraOperationParameters<ComparisonType>, OneExtractedValue>
    {
        protected const int FirstValueIndex = 0;
        protected const int SecondValueIndex = 1;

        protected ComparisonType ComparisonType { get; set; }

        /// <summary>
        /// First comparison argument, if expected.
        /// </summary>
        protected IDataHolder FirstValue { get; set; }

        /// <summary>
        /// Second comparison argument, if expected.
        /// </summary>
        protected IDataHolder SecondValue { get; set; }

        public void Initialize(OutputExtraOperationParameters<ComparisonType> processingParameters)
        {
            this.ComparisonType = processingParameters.OperationType;

            ArgumentChecker.CheckPresence(processingParameters.DataHolderParameters, FirstValueIndex);
            this.FirstValue = processingParameters.DataHolderParameters[FirstValueIndex];
            ArgumentChecker.CheckNotNull(this.FirstValue);

            if (processingParameters.DataHolderParameters.Length > SecondValueIndex)
            {
                this.SecondValue = processingParameters.DataHolderParameters[SecondValueIndex];
                ArgumentChecker.CheckNotNull(this.SecondValue);
            }

            this.OutputWriter = new FileWriter(processingParameters.OutputFilePath);
        }

        public bool Execute(ulong lineNumber, OneExtractedValue lineData)
        {
            // Perform the comparison to decide whether to output the line.
            //
            if (DataHolderOperations.Compare(lineData.ExtractedData, this.ComparisonType, this.FirstValue, this.SecondValue))
            {
                this.OutputWriter.WriteLine(lineData.OriginalLine);
            }

            return true;
        }
    }
}
