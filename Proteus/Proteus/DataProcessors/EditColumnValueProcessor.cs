////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using LaurentiuCristofor.Proteus.Common;
using LaurentiuCristofor.Proteus.Common.Types;
using LaurentiuCristofor.Proteus.DataExtractors;
using LaurentiuCristofor.Proteus.DataProcessors.Parameters;
using LaurentiuCristofor.Proteus.FileOperations;

namespace LaurentiuCristofor.Proteus.DataProcessors
{
    /// <summary>
    /// A data processor that edits a value passed through a DataTypeContainer.
    /// </summary>
    public class EditColumnValueProcessor : BaseOutputProcessor, IDataProcessor<ValueEditOutputParameters, ParsedLine>
    {
        protected ValueEditOutputParameters Parameters { get; set; }

        public void Initialize(ValueEditOutputParameters processingParameters)
        {
            this.Parameters = processingParameters;

            switch (this.Parameters.EditType)
            {
                case ValueEditType.Rewrite:
                    break;

                case ValueEditType.Add:
                case ValueEditType.Subtract:
                case ValueEditType.Multiply:
                case ValueEditType.Divide:
                    ArgumentChecker.CheckNotNull(this.Parameters.Argument);
                    break;

                default:
                    throw new ProteusException($"Internal error: Proteus is not handling value edit type '{this.Parameters.EditType}'!");
            }

            this.OutputWriter = new FileWriter(this.Parameters.OutputFilePath);
        }

        public bool Execute(ulong lineNumber, ParsedLine lineData)
        {
            DataProcessorValidation.ValidateColumnInformation(lineData);

            int columnIndex = lineData.ExtractedColumnNumber - 1;

            // Update the column string representation with the string representation of the edited data.
            //
            switch (this.Parameters.EditType)
            {
                case ValueEditType.Rewrite:
                    lineData.Columns[columnIndex] = lineData.ExtractedData.ToString();
                    break;

                case ValueEditType.Add:
                    lineData.Columns[columnIndex] = lineData.ExtractedData.Add(this.Parameters.Argument).ToString();
                    break;

                case ValueEditType.Subtract:
                    lineData.Columns[columnIndex] = lineData.ExtractedData.Subtract(this.Parameters.Argument).ToString();
                    break;

                case ValueEditType.Multiply:
                    lineData.Columns[columnIndex] = lineData.ExtractedData.Multiply(this.Parameters.Argument).ToString();
                    break;

                case ValueEditType.Divide:
                    lineData.Columns[columnIndex] = lineData.ExtractedData.Divide(this.Parameters.Argument).ToString();
                    break;

                default:
                    throw new ProteusException($"Internal error: Proteus is not handling value edit type '{this.Parameters.EditType}'!");
            }

            // Put back together all column strings, to form the edited line.
            //
            string editedLine = string.Join(lineData.ColumnSeparator, lineData.Columns);

            this.OutputWriter.WriteLine(editedLine);

            return true;
        }
    }
}
