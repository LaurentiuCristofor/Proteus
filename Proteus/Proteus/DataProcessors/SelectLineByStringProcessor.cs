////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// 
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

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
    /// </summary>
    public class SelectLineByStringProcessor : BaseOutputProcessor, IDataProcessor<OutputOperationParameters<StringSelectionType>, ParsedLine>
    {
        protected OutputOperationParameters<StringSelectionType> Parameters { get; set; }

        /// <summary>
        /// The selector used to perform the operation.
        /// </summary>
        protected StringSelector StringSelector { get; set; }

        public void Initialize(OutputOperationParameters<StringSelectionType> processingParameters)
        {
            this.Parameters = processingParameters;

            this.StringSelector = new StringSelector();
            this.StringSelector.Initialize(this.Parameters.OperationType, this.Parameters.FirstArgument, this.Parameters.SecondArgument);

            this.OutputWriter = new FileWriter(this.Parameters.OutputFilePath);
        }

        public bool Execute(ulong lineNumber, ParsedLine lineData)
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
