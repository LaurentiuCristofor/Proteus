////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;

using LaurentiuCristofor.Proteus.Common;
using LaurentiuCristofor.Proteus.FileOperations;

namespace LaurentiuCristofor.Proteus.DataProcessors
{
    /// <summary>
    /// A data processor that performs an edit operation on each line.
    /// </summary>
    public class LineEditProcessor : BaseOutputProcessor, IDataProcessor<OperationTypeParameters<StringEditType>, string>
    {
        /// <summary>
        /// Parameters of this operation.
        /// </summary>
        private OperationTypeParameters<StringEditType> Parameters { get; set; }

        public void Initialize(OperationTypeParameters<StringEditType> processingParameters)
        {
            this.Parameters = processingParameters;

            this.OutputWriter = new TextFileWriter(this.Parameters.OutputFilePath);
        }

        public bool Execute(ulong lineNumber, string inputLine)
        {
            // Perform the transformation of the line and output the result.
            //
            string editedLine = DataEditor.Edit(
                inputLine,
                this.Parameters.OperationType,
                lineNumber,
                this.Parameters.FirstArgument,
                this.Parameters.SecondArgument);

            this.OutputWriter.WriteLine(editedLine);

            return true;
        }
    }
}
