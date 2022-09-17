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
    public class EditStringProcessor : BaseOutputProcessor, IDataProcessor<OperationTypeParameters<StringEditType>, ParsedLine>
    {
        /// <summary>
        /// Parameters of this operation.
        /// </summary>
        protected OperationTypeParameters<StringEditType> Parameters { get; set; }

        /// <summary>
        /// The editor used to perform the operation.
        /// </summary>
        protected StringEditor StringEditor { get; set; }

        public void Initialize(OperationTypeParameters<StringEditType> processingParameters)
        {
            this.Parameters = processingParameters;

            this.StringEditor = new StringEditor();
            this.StringEditor.Initialize(this.Parameters.OperationType, this.Parameters.FirstArgument, this.Parameters.SecondArgument);

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
            string editedData = this.StringEditor.Edit(data, lineNumber);

            string editedLine = editedData;

            // Check if we need to reconstruct a line from column parts;
            // this is the case when we've been editing a column value.
            //
            if (lineData.ExtractedColumnNumber != 0)
            {
                // If we have a prefix, prepend it before the edited data.
                //
                string linePrefix = string.Join(lineData.ColumnSeparator, lineData.Columns, 0, lineData.ExtractedColumnNumber - 1);
                if (!String.IsNullOrEmpty(linePrefix))
                {
                    editedLine = linePrefix + lineData.ColumnSeparator + editedLine;
                }

                // If we have a suffix, append it after the edited data.
                //
                string lineSuffix = string.Join(lineData.ColumnSeparator, lineData.Columns, lineData.ExtractedColumnNumber, lineData.Columns.Length - lineData.ExtractedColumnNumber);
                if (!String.IsNullOrEmpty(lineSuffix))
                {
                    editedLine += lineData.ColumnSeparator + lineSuffix;
                }
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
