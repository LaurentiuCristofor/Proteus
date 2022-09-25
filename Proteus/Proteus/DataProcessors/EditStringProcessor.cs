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
    /// A data processor that edits a string passed through a DataTypeContainer.
    /// </summary>
    public class EditStringProcessor : BaseOutputProcessor, IDataProcessor<OperationOutputParameters<StringEditType>, ParsedLine>
    {
        protected OperationOutputParameters<StringEditType> Parameters { get; set; }

        /// <summary>
        /// The editor used to perform the operation.
        /// </summary>
        protected StringEditor StringEditor { get; set; }

        public void Initialize(OperationOutputParameters<StringEditType> processingParameters)
        {
            this.Parameters = processingParameters;

            this.StringEditor = new StringEditor();
            this.StringEditor.Initialize(this.Parameters.OperationType, this.Parameters.FirstArgument, this.Parameters.SecondArgument);

            this.OutputWriter = new FileWriter(this.Parameters.OutputFilePath);
        }

        public bool Execute(ulong lineNumber, ParsedLine lineData)
        {
            DataProcessorValidation.ValidateExtractedDataIsString(lineData);

            string data = lineData.ExtractedData.ToString();
            string editedData = this.StringEditor.Edit(data, lineNumber);

            string editedLine = editedData;

            // Check if we need to reconstruct a line from column parts;
            // this is the case when we've been editing a column value.
            //
            if (lineData.ExtractedColumnNumber != 0)
            {
                // Replace the column data with the edited one and join all the columns into a line.
                //
                int columnIndex = lineData.ExtractedColumnNumber - 1;
                lineData.Columns[columnIndex] = editedData;
                editedLine = string.Join(lineData.ColumnSeparator, lineData.Columns);
            }

            // Do not output empty lines.
            //
            if (String.IsNullOrEmpty(editedLine))
            {
                return true;
            }

            this.OutputWriter.WriteLine(editedLine);

            return true;
        }
    }
}
