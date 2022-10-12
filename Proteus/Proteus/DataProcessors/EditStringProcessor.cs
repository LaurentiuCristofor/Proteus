////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
///
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;

using LaurentiuCristofor.Proteus.Common.Types;
using LaurentiuCristofor.Proteus.Common.Utilities;
using LaurentiuCristofor.Proteus.DataExtractors;
using LaurentiuCristofor.Proteus.DataProcessors.Parameters;
using LaurentiuCristofor.Proteus.FileOperations;

namespace LaurentiuCristofor.Proteus.DataProcessors
{
    /// <summary>
    /// A data processor that edits a string.
    ///
    /// OutputExtraOperationParameters is expected to contain:
    /// StringParameters[0] - first string edit argument (optional)
    /// StringParameters[1] - second string edit argument (optional)
    /// </summary>
    public class EditStringProcessor : BaseOutputProcessor, IDataProcessor<OutputExtraOperationParameters<StringEditType>, OneExtractedValue>
    {
        protected const int FirstArgumentIndex = 0;
        protected const int SecondArgumentIndex = 1;

        /// <summary>
        /// The editor used to perform the operation.
        /// </summary>
        protected StringEditor StringEditor { get; set; }

        public void Initialize(OutputExtraOperationParameters<StringEditType> processingParameters)
        {
            this.StringEditor = new StringEditor();

            string firstArgument
                = (processingParameters.StringParameters != null && processingParameters.StringParameters.Length > FirstArgumentIndex)
                ? processingParameters.StringParameters[FirstArgumentIndex]
                : null;
            string secondArgument
                = (processingParameters.StringParameters != null && processingParameters.StringParameters.Length > SecondArgumentIndex)
                ? processingParameters.StringParameters[SecondArgumentIndex]
                : null;

            this.StringEditor.Initialize(processingParameters.OperationType, firstArgument, secondArgument);

            this.OutputWriter = new FileWriter(processingParameters.OutputFilePath);
        }

        public bool Execute(ulong lineNumber, OneExtractedValue lineData)
        {
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

            this.OutputWriter.WriteLine(editedLine);

            return true;
        }
    }
}
