﻿////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
///
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;

using LaurentiuCristofor.Proteus.Common;
using LaurentiuCristofor.Proteus.Common.Types;
using LaurentiuCristofor.Proteus.DataProcessors.Parameters;
using LaurentiuCristofor.Proteus.FileOperations;

namespace LaurentiuCristofor.Proteus.DataProcessors
{
    /// <summary>
    /// A data processor that checks the line number against a selection criterion,
    /// to decide whether to output the line or not.
    /// </summary>
    public class SelectLineByNumberProcessor : BaseOutputProcessor, IDataProcessor<OutputOperationParameters<PositionSelectionType>, string>
    {
        protected PositionSelectionType SelectionType { get; set; }

        /// <summary>
        /// First line number argument, if expected.
        /// </summary>
        protected ulong FirstLineNumber { get; set; }

        /// <summary>
        /// Second line number argument, if expected.
        /// </summary>
        protected ulong SecondLineNumber { get; set; }

        /// <summary>
        /// Data structure used for implementing LineNumberComparisonType.Last and LineNumberComparisonType.NotLast.
        /// </summary>
        protected Queue<string> SizeLimitedQueue { get; set; }

        public void Initialize(OutputOperationParameters<PositionSelectionType> processingParameters)
        {
            this.SelectionType = processingParameters.OperationType;

            if (this.SelectionType == PositionSelectionType.Last
                || this.SelectionType == PositionSelectionType.NotLast)
            {
                this.SizeLimitedQueue = new Queue<string>();
            }

            switch (this.SelectionType)
            {
                case PositionSelectionType.Last:
                case PositionSelectionType.NotLast:
                case PositionSelectionType.Each:
                case PositionSelectionType.NotEach:
                    ArgumentChecker.CheckNotNull(processingParameters.FirstArgument);

                    this.FirstLineNumber = ulong.Parse(processingParameters.FirstArgument);

                    ArgumentChecker.CheckGreaterThanOrEqualTo(this.FirstLineNumber, 1UL);
                    break;

                case PositionSelectionType.Between:
                case PositionSelectionType.NotBetween:
                    ArgumentChecker.CheckNotNull(processingParameters.FirstArgument);
                    ArgumentChecker.CheckNotNull(processingParameters.SecondArgument);

                    this.FirstLineNumber = ulong.Parse(processingParameters.FirstArgument);
                    this.SecondLineNumber = ulong.Parse(processingParameters.SecondArgument);

                    ArgumentChecker.CheckGreaterThanOrEqualTo(this.FirstLineNumber, 1UL);
                    ArgumentChecker.CheckGreaterThanOrEqualTo(this.SecondLineNumber, 1UL);
                    ArgumentChecker.CheckInterval(this.FirstLineNumber, this.SecondLineNumber);
                    break;

                default:
                    throw new ProteusException($"Internal error: Proteus is not handling number selection type '{this.SelectionType}'!");
            }

            if (this.SelectionType == PositionSelectionType.Last)
            {
                // For this operation, the writing is performed after we've completed reading,
                // so we want additional progress tracking for it.
                //
                this.OutputWriter = new FileWriter(processingParameters.OutputFilePath, trackProgress: true);
            }
            else
            {
                this.OutputWriter = new FileWriter(processingParameters.OutputFilePath);
            }
        }

        public bool Execute(ulong lineNumber, string line)
        {
            // Decide whether to output the current line.
            //
            // By default, we will output the line and return true.
            // To skip outputting a line, we return true.
            // To stop further file processing, we return false.
            //
            switch (this.SelectionType)
            {
                case PositionSelectionType.Between:
                    // Skip first lines until we reach the line from which we start to output.
                    //
                    if (lineNumber < this.FirstLineNumber)
                    {
                        return true;
                    }
                    // Abort further processing once we've completed outputting the lines we wanted.
                    //
                    else if (lineNumber > this.SecondLineNumber)
                    {
                        return false;
                    }
                    break;

                case PositionSelectionType.NotBetween:
                    // Skip the lines in the interval that we don't want to output.
                    //
                    if (lineNumber >= this.FirstLineNumber
                        && lineNumber <= this.SecondLineNumber)
                    {
                        return true;
                    }
                    break;

                case PositionSelectionType.Last:
                    // Keep enqueuing lines into a queue.
                    // Once the queue contains as many lines as we want to output,
                    // we'll remove a line before adding a new one, to keep the queue size constant.
                    // By the time we finish processing the file, the queue will contain all lines
                    // that we need to output and we'll output them in CompleteExecution().
                    //
                    if ((ulong)this.SizeLimitedQueue.Count == this.FirstLineNumber)
                    {
                        this.SizeLimitedQueue.Dequeue();
                    }

                    this.SizeLimitedQueue.Enqueue(line);

                    return true;

                case PositionSelectionType.NotLast:
                    {
                        string lineToOutput = null;

                        // Keep enqueuing lines into a queue.
                        // Once the queue contains as many lines as we don't want to output,
                        // we'll remove a line when adding a new one, to keep the queue size constant;
                        // then we will output the line that we just removed.
                        //
                        if ((ulong)this.SizeLimitedQueue.Count == this.FirstLineNumber)
                        {
                            lineToOutput = this.SizeLimitedQueue.Dequeue();
                        }

                        this.SizeLimitedQueue.Enqueue(line);

                        // Quick exit in case we don't have a line to output.
                        //
                        if (lineToOutput == null)
                        {
                            return true;
                        }

                        line = lineToOutput;

                        break;
                    }

                case PositionSelectionType.Each:
                    // Skip the lines whose numbers are not multiples of our argument.
                    //
                    if (lineNumber % this.FirstLineNumber > 0)
                    {
                        return true;
                    }
                    break;

                case PositionSelectionType.NotEach:
                    // Skip the lines whose numbers are multiples of our argument.
                    //
                    if (lineNumber % this.FirstLineNumber == 0)
                    {
                        return true;
                    }
                    break;

                default:
                    throw new ProteusException($"Internal error: Proteus is not handling position selection type '{this.SelectionType}'!");
            }

            this.OutputWriter.WriteLine(line);

            return true;
        }

        public override void CompleteExecution()
        {
            if (this.SelectionType == PositionSelectionType.Last)
            {
                if (this.SizeLimitedQueue == null)
                {
                    throw new ProteusException("Internal error: An expected data structure has not been initialized!");
                }

                foreach (string line in this.SizeLimitedQueue)
                {
                    this.OutputWriter.WriteLine(line);
                }
            }

            base.CompleteExecution();
        }
    }
}
