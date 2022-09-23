////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using LaurentiuCristofor.Proteus.Common;
using LaurentiuCristofor.Proteus.DataExtractors;
using LaurentiuCristofor.Proteus.FileOperations;

namespace LaurentiuCristofor.Proteus.DataProcessors
{
    /// <summary>
    /// A data processor that checks a string against a selection criterion,
    /// to decide whether to output the line or not.
    /// </summary>
    public class SelectLineByStringProcessor : BaseOutputProcessor, IDataProcessor<OperationOutputParameters<StringSelectionType>, ParsedLine>
    {
        /// <summary>
        /// Parameters of this operation.
        /// </summary>
        protected OperationOutputParameters<StringSelectionType> Parameters { get; set; }

        /// <summary>
        /// The selector used to perform the operation.
        /// </summary>
        protected StringSelector StringSelector { get; set; }

        public void Initialize(OperationOutputParameters<StringSelectionType> processingParameters)
        {
            this.Parameters = processingParameters;

            this.StringSelector = new StringSelector();
            this.StringSelector.Initialize(this.Parameters.OperationType, this.Parameters.SafeFirstArgument, this.Parameters.SafeSecondArgument);

            this.OutputWriter = new FileWriter(this.Parameters.OutputFilePath);
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

            DataProcessorValidation.ValidateExtractedDataIsString(lineData);

            string data = lineData.ExtractedData.ToString();

            if (this.StringSelector.Select(data))
            {
                this.OutputWriter.WriteLine(lineData.OriginalLine);
            }

            return true;
        }
    }
}
