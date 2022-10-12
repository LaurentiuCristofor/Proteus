////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
///
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using LaurentiuCristofor.Proteus.Common;
using LaurentiuCristofor.Proteus.Common.Types;
using LaurentiuCristofor.Proteus.Common.Utilities;
using LaurentiuCristofor.Proteus.DataExtractors;
using LaurentiuCristofor.Proteus.DataProcessors.Parameters;
using LaurentiuCristofor.Proteus.FileOperations;

namespace LaurentiuCristofor.Proteus.DataProcessors
{
    /// <summary>
    /// A data processor that checks a string against a selection criterion,
    /// to decide whether to output the line or not.
    ///
    /// OutputExtraOperationParameters is expected to contain:
    /// StringParameters[0] - first string selection argument
    /// StringParameters[1] - second string selection argument (optional)
    /// </summary>
    public class SelectLineByStringProcessor : BaseOutputProcessor, IDataProcessor<OutputExtraOperationParameters<StringSelectionType>, OneExtractedValue>
    {
        protected const int FirstArgumentIndex = 0;
        protected const int SecondArgumentIndex = 1;

        /// <summary>
        /// The selector used to perform the operation.
        /// </summary>
        protected StringSelector StringSelector { get; set; }

        public void Initialize(OutputExtraOperationParameters<StringSelectionType> processingParameters)
        {
            this.StringSelector = new StringSelector();

            ArgumentChecker.CheckPresence(processingParameters.StringParameters, FirstArgumentIndex);
            string firstArgument = processingParameters.StringParameters[FirstArgumentIndex];

            string secondArgument = null;
            if (processingParameters.StringParameters.Length > SecondArgumentIndex)
            {
                secondArgument = processingParameters.StringParameters[SecondArgumentIndex];
            }

            this.StringSelector.Initialize(processingParameters.OperationType, firstArgument, secondArgument);

            this.OutputWriter = new FileWriter(processingParameters.OutputFilePath);
        }

        public bool Execute(ulong lineNumber, OneExtractedValue lineData)
        {
            string data = lineData.ExtractedData.ToString();

            if (this.StringSelector.Select(data))
            {
                this.OutputWriter.WriteLine(lineData.OriginalLine);
            }

            return true;
        }
    }
}
