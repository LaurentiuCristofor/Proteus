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
    /// A data processor that edits the value of a column.
    /// </summary>
    public class ColumnEditProcessor : BaseOutputProcessor, IDataProcessor<OperationTypeParameters<StringEditType>, Tuple<LineParts, DataTypeContainer>>
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

        public bool Execute(ulong lineNumber, Tuple<LineParts, DataTypeContainer> inputData)
        {
            // We may not always be able to extract a column.
            // Ignore these cases; the extractor will already have printed a warning message.
            //
            if (inputData == null)
            {
                return true;
            }

            string column = inputData.Item2.ToString();
            string editedColumn = DataEditor.Edit(column, this.Parameters.OperationType, lineNumber, this.Parameters.FirstArgument, this.Parameters.SecondArgument);
            string editedRow = inputData.Item1.DataBeforeColumn + editedColumn + inputData.Item1.DataAfterColumn;

            this.OutputWriter.WriteLine(editedRow);

            return true;
        }
    }
}
