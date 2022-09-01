////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using LaurentiuCristofor.Proteus.Common;
using LaurentiuCristofor.Proteus.FileOperations;

namespace LaurentiuCristofor.Proteus.DataProcessors
{
    /// <summary>
    /// A data processor that checks the line string against a selection criterion,
    /// to decide whether to output the line or not.
    /// </summary>
    public class LineStringSelectProcessor : BaseOutputProcessor, IDataProcessor<OperationTypeParameters<StringSelectionType>, string>
    {
        /// <summary>
        /// Parameters of this operation.
        /// </summary>
        protected OperationTypeParameters<StringSelectionType> Parameters { get; set; }

        /// <summary>
        /// The selector used to perform the operation.
        /// </summary>
        protected StringSelector StringSelector { get; set; }

        public void Initialize(OperationTypeParameters<StringSelectionType> processingParameters)
        {
            this.Parameters = processingParameters;

            this.StringSelector = new StringSelector();
            this.StringSelector.Initialize(this.Parameters.OperationType, this.Parameters.FirstArgument, this.Parameters.SecondArgument);

            this.OutputWriter = new TextFileWriter(this.Parameters.OutputFilePath);
        }

        public bool Execute(ulong lineNumber, string line)
        {
            DataProcessorValidation.ValidateLine(line);

            if (this.StringSelector.Select(line))
            {
                this.OutputWriter.WriteLine(line);
            }

            return true;
        }
    }
}
