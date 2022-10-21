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
    /// A data processor that edits a column value.
    ///
    /// OutputExtraOperationParameters is expected to contain:
    /// DataHolderParameters[0] - edit operand (if required)
    /// </summary>
    public class EditColumnValueProcessor : BaseOutputProcessor, IDataProcessor<OutputExtraOperationParameters<ValueEditType>, OneExtractedValue>
    {
        protected int EditOperandIndex = 0;

        protected ValueEditType EditType { get; set; }

        /// <summary>
        /// The edit operand, if expected.
        /// </summary>
        protected IDataHolder EditOperand { get; set; }

        public void Initialize(OutputExtraOperationParameters<ValueEditType> processingParameters)
        {
            EditType = processingParameters.OperationType;

            switch (EditType)
            {
                case ValueEditType.Rewrite:
                    break;

                case ValueEditType.Add:
                case ValueEditType.Subtract:
                case ValueEditType.Multiply:
                case ValueEditType.Divide:
                    ArgumentChecker.CheckPresence(processingParameters.DataHolderParameters, EditOperandIndex);
                    EditOperand = processingParameters.DataHolderParameters[EditOperandIndex];
                    ArgumentChecker.CheckNotNull(EditOperand);
                    break;

                default:
                    throw new ProteusException($"Internal error: Proteus is not handling value edit type '{EditType}'!");
            }

            OutputWriter = new FileWriter(processingParameters.OutputFilePath);
        }

        public bool Execute(ulong lineNumber, OneExtractedValue lineData)
        {
            int columnIndex = lineData.ExtractedColumnNumber - 1;

            // Update the column string representation with the string representation of the edited data.
            //
            switch (EditType)
            {
                case ValueEditType.Rewrite:
                    lineData.Columns[columnIndex] = lineData.ExtractedData.ToString();
                    break;

                case ValueEditType.Add:
                    lineData.Columns[columnIndex] = lineData.ExtractedData.Add(EditOperand).ToString();
                    break;

                case ValueEditType.Subtract:
                    lineData.Columns[columnIndex] = lineData.ExtractedData.Subtract(EditOperand).ToString();
                    break;

                case ValueEditType.Multiply:
                    lineData.Columns[columnIndex] = lineData.ExtractedData.Multiply(EditOperand).ToString();
                    break;

                case ValueEditType.Divide:
                    lineData.Columns[columnIndex] = lineData.ExtractedData.Divide(EditOperand).ToString();
                    break;

                default:
                    throw new ProteusException($"Internal error: Proteus is not handling value edit type '{EditType}'!");
            }

            // Put back together all column strings, to form the edited line.
            //
            string editedLine = string.Join(lineData.ColumnSeparator, lineData.Columns);

            OutputWriter.WriteLine(editedLine);

            return true;
        }
    }
}
