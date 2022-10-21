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
    /// A data processor that checks the column count against a selection criterion,
    /// to decide whether to output the line or not.
    ///
    /// OutputExtraOperationParameters is expected to contain:
    /// DataHolderParameters[0] - first comparison argument
    /// DataHolderParameters[1] - second comparison argument (if required)
    /// </summary>
    public class SelectLineByColumnCountProcessor : BaseOutputProcessor, IDataProcessor<OutputExtraOperationParameters<ComparisonType>, ExtractedColumnStrings>
    {
        protected const int FirstColumnCountIndex = 0;
        protected const int SecondColumnCountIndex = 1;

        protected ComparisonType ComparisonType { get; set; }

        /// <summary>
        /// First column count comparison argument.
        /// </summary>
        protected IDataHolder FirstColumnCount { get; set; }

        /// <summary>
        /// Second column count comparison argument, if expected.
        /// </summary>
        protected IDataHolder SecondColumnCount { get; set; }

        public void Initialize(OutputExtraOperationParameters<ComparisonType> processingParameters)
        {
            ComparisonType = processingParameters.OperationType;

            ArgumentChecker.CheckPresence(processingParameters.DataHolderParameters, FirstColumnCountIndex);
            FirstColumnCount = processingParameters.DataHolderParameters[FirstColumnCountIndex];
            ArgumentChecker.CheckNotNull(FirstColumnCount);

            if (processingParameters.DataHolderParameters.Length > SecondColumnCountIndex)
            {
                SecondColumnCount = processingParameters.DataHolderParameters[SecondColumnCountIndex];
                ArgumentChecker.CheckNotNull(SecondColumnCount);
            }

            OutputWriter = new FileWriter(processingParameters.OutputFilePath);
        }

        public bool Execute(ulong lineNumber, ExtractedColumnStrings lineData)
        {
            // Package column count in an IDataHolder.
            //
            IDataHolder columnCountContainer = new IntegerDataHolder(lineData.Columns.Length);

            // Perform the comparison to decide whether to output the line.
            //
            if (DataHolderOperations.Compare(columnCountContainer, ComparisonType, FirstColumnCount, SecondColumnCount))
            {
                OutputWriter.WriteLine(lineData.OriginalLine);
            }

            return true;
        }
    }
}
