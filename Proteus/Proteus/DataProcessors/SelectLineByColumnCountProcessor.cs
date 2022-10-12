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
    /// DataHolderParameters[1] - second comparison argument (optional)
    /// </summary>
    public class SelectLineByColumnCountProcessor : BaseOutputProcessor, IDataProcessor<OutputExtraOperationParameters<ComparisonType>, ExtractedColumnStrings>
    {
        protected const int FirstArgumentIndex = 0;
        protected const int SecondArgumentIndex = 1;

        protected ComparisonType ComparisonType { get; set; }

        /// <summary>
        /// First comparison argument, if expected.
        /// </summary>
        protected IDataHolder FirstArgument { get; set; }

        /// <summary>
        /// Second comparison argument, if expected.
        /// </summary>
        protected IDataHolder SecondArgument { get; set; }

        public void Initialize(OutputExtraOperationParameters<ComparisonType> processingParameters)
        {
            this.ComparisonType = processingParameters.OperationType;

            ArgumentChecker.CheckPresence(processingParameters.DataHolderParameters, FirstArgumentIndex);
            this.FirstArgument = processingParameters.DataHolderParameters[FirstArgumentIndex];

            if (processingParameters.DataHolderParameters.Length > SecondArgumentIndex)
            {
                this.SecondArgument = processingParameters.DataHolderParameters[SecondArgumentIndex];
            }

            this.OutputWriter = new FileWriter(processingParameters.OutputFilePath);
        }

        public bool Execute(ulong lineNumber, ExtractedColumnStrings lineData)
        {
            // Package column count in an IDataHolder.
            //
            IDataHolder columnCountContainer = new IntegerDataHolder(lineData.Columns.Length);

            // Perform the comparison to decide whether to output the line.
            //
            if (DataHolderOperations.Compare(columnCountContainer, this.ComparisonType, this.FirstArgument, this.SecondArgument))
            {
                this.OutputWriter.WriteLine(lineData.OriginalLine);
            }

            return true;
        }
    }
}
