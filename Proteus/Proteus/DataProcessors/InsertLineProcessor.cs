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
    /// </summary>
    public class InsertLineProcessor : BaseOutputProcessor, IDataProcessor<OutputOperationParameters<PositionInsertionType>, string>
    {
        protected OutputOperationParameters<PositionInsertionType> Parameters { get; set; }

        /// <summary>
        /// Second argument, as an unsigned integer value.
        /// </summary>
        protected ulong SecondArgumentAsULong { get; set; }

        /// <summary>
        /// Counter of lines written out.
        /// </summary>
        protected ulong LastOutputLineNumber { get; set; }

        public void Initialize(OutputOperationParameters<PositionInsertionType> processingParameters)
        {
            this.Parameters = processingParameters;

            switch (this.Parameters.OperationType)
            {
                case PositionInsertionType.Position:
                case PositionInsertionType.Each:
                    ArgumentChecker.CheckNotNullAndNotEmpty(this.Parameters.FirstArgument);
                    ArgumentChecker.CheckNotNull(this.Parameters.SecondArgument);

                    this.SecondArgumentAsULong = ulong.Parse(this.Parameters.SecondArgument);

                    ArgumentChecker.CheckNotZero(this.SecondArgumentAsULong);
                    break;

                case PositionInsertionType.Last:
                    ArgumentChecker.CheckNotNullAndNotEmpty(this.Parameters.FirstArgument);
                    break;

                default:
                    throw new ProteusException($"Internal error: Proteus is not handling number insertion type '{this.Parameters.OperationType}'!");
            }

            this.OutputWriter = new FileWriter(this.Parameters.OutputFilePath);
        }

        public bool Execute(ulong lineNumber, string line)
        {
            // Decide whether to output the argument line
            // before the current existing line.
            //
            switch (this.Parameters.OperationType)
            {
                case PositionInsertionType.Position:
                    // If we reached the desired position, insert our line argument.
                    //
                    if (lineNumber == this.SecondArgumentAsULong)
                    {
                        this.OutputWriter.WriteLine(this.Parameters.FirstArgument);
                        ++this.LastOutputLineNumber;
                    }
                    break;

                case PositionInsertionType.Each:
                    // Insert our line argument whenever its line number in the output file would be a multiple of our desired "each" argument.
                    //
                    if ((this.LastOutputLineNumber + 1) % this.SecondArgumentAsULong == 0)
                    {
                        this.OutputWriter.WriteLine(this.Parameters.FirstArgument);
                        ++this.LastOutputLineNumber;
                    }
                    break;

                case PositionInsertionType.Last:
                    // Nothing to do until we get to the end of the file.
                    //
                    break;

                default:
                    throw new ProteusException($"Internal error: Proteus is not handling number insertion type '{this.Parameters.OperationType}'!");
            }

            this.OutputWriter.WriteLine(line);
            ++this.LastOutputLineNumber;

            return true;
        }

        public override void CompleteExecution()
        {
            // Decide whether to output the line as the last line.
            //
            if ((this.Parameters.OperationType == PositionInsertionType.Position
                && this.SecondArgumentAsULong == this.LastOutputLineNumber + 1)
                || (this.Parameters.OperationType == PositionInsertionType.Each
                && ((this.LastOutputLineNumber + 1) % this.SecondArgumentAsULong == 0))
                || this.Parameters.OperationType == PositionInsertionType.Last)
            {
                this.OutputWriter.WriteLine(this.Parameters.FirstArgument);
            }

            base.CompleteExecution();
        }
    }
}
