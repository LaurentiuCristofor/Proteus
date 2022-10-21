////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
///
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using LaurentiuCristofor.Proteus.Common;
using LaurentiuCristofor.Proteus.Common.Types;
using LaurentiuCristofor.Proteus.DataProcessors.Parameters;
using LaurentiuCristofor.Proteus.FileOperations;

namespace LaurentiuCristofor.Proteus.DataProcessors
{
    /// <summary>
    /// A data processor that checks the current line number,
    /// to decide whether to insert a line at that position or not.
    /// 
    /// OutputExtraOperationParameters is expected to contain:
    /// StringParameters[0] - line to insert
    /// UlongParameters[0] - insertion line count (if required)
    /// </summary>
    public class InsertLineProcessor : BaseOutputProcessor, IDataProcessor<OutputExtraOperationParameters<PositionInsertionType>, string>
    {
        protected const int LineToInsertIndex = 0;
        protected const int InsertionLineCountIndex = 0;

        protected PositionInsertionType InsertionType { get; set; }

        protected string LineToInsert { get; set; }

        /// <summary>
        /// A line count indicator of where to insert, if expected.
        /// </summary>
        protected ulong InsertionLineCount { get; set; }

        /// <summary>
        /// Counter of lines written out.
        /// </summary>
        protected ulong LastOutputLineNumber { get; set; }

        public void Initialize(OutputExtraOperationParameters<PositionInsertionType> processingParameters)
        {
            this.InsertionType = processingParameters.OperationType;

            ArgumentChecker.CheckPresence(processingParameters.StringParameters, LineToInsertIndex);
            this.LineToInsert = processingParameters.StringParameters[LineToInsertIndex];
            ArgumentChecker.CheckNotNull(this.LineToInsert);

            switch (this.InsertionType)
            {
                case PositionInsertionType.Position:
                case PositionInsertionType.Each:
                    ArgumentChecker.CheckPresence(processingParameters.UlongParameters, InsertionLineCountIndex);
                    this.InsertionLineCount = processingParameters.UlongParameters[InsertionLineCountIndex];
                    ArgumentChecker.CheckGreaterThanOrEqualTo(this.InsertionLineCount, 1UL);
                    break;

                case PositionInsertionType.Last:
                    break;

                default:
                    throw new ProteusException($"Internal error: Proteus is not handling number insertion type '{this.InsertionType}'!");
            }

            this.OutputWriter = new FileWriter(processingParameters.OutputFilePath);
        }

        public bool Execute(ulong lineNumber, string line)
        {
            // Decide whether to output the argument line
            // before the current existing line.
            //
            switch (this.InsertionType)
            {
                case PositionInsertionType.Position:
                    // If we reached the desired position, insert our line argument.
                    //
                    if (lineNumber == this.InsertionLineCount)
                    {
                        this.OutputWriter.WriteLine(this.LineToInsert);
                        ++this.LastOutputLineNumber;
                    }
                    break;

                case PositionInsertionType.Each:
                    // Insert our line argument whenever its line number in the output file would be a multiple of our desired "each" argument.
                    //
                    if ((this.LastOutputLineNumber + 1) % this.InsertionLineCount == 0)
                    {
                        this.OutputWriter.WriteLine(this.LineToInsert);
                        ++this.LastOutputLineNumber;
                    }
                    break;

                case PositionInsertionType.Last:
                    // Nothing to do until we get to the end of the file.
                    //
                    break;

                default:
                    throw new ProteusException($"Internal error: Proteus is not handling number insertion type '{this.InsertionType}'!");
            }

            this.OutputWriter.WriteLine(line);
            ++this.LastOutputLineNumber;

            return true;
        }

        public override void CompleteExecution()
        {
            // Decide whether to output the line as the last line.
            //
            if ((this.InsertionType == PositionInsertionType.Position
                && this.InsertionLineCount == this.LastOutputLineNumber + 1)
                || (this.InsertionType == PositionInsertionType.Each
                && ((this.LastOutputLineNumber + 1) % this.InsertionLineCount == 0))
                || this.InsertionType == PositionInsertionType.Last)
            {
                this.OutputWriter.WriteLine(this.LineToInsert);
            }

            base.CompleteExecution();
        }
    }
}
