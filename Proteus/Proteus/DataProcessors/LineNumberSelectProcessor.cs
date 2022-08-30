﻿////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;
using LaurentiuCristofor.Proteus.Common;
using LaurentiuCristofor.Proteus.FileOperations;

namespace LaurentiuCristofor.Proteus.DataProcessors
{
    /// <summary>
    /// A data processor that checks the line number against a selection criterion,
    /// to decide whether to output the line or not.
    /// </summary>
    public class LineNumberSelectProcessor : BaseOutputProcessor, IDataProcessor<OperationTypeParameters<LineNumberSelectionType>, string>
    {
        /// <summary>
        /// Parameters of this operation.
        /// </summary>
        protected OperationTypeParameters<LineNumberSelectionType> Parameters { get; set; }

        /// <summary>
        /// First line number comparison argument, as an integer value.
        /// </summary>
        protected ulong FirstArgumentAsULong { get; set; }

        /// <summary>
        /// Second line number comparison argument, as an integer value.
        /// </summary>
        protected ulong SecondArgumentAsULong { get; set; }

        /// <summary>
        /// Data structure used for implementing LineNumberComparisonType.Last and LineNumberComparisonType.NotLast.
        /// </summary>
        protected Queue<string> SizeLimitedQueue { get; set; }

        public void Initialize(OperationTypeParameters<LineNumberSelectionType> processingParameters)
        {
            this.Parameters = processingParameters;

            this.OutputWriter = new TextFileWriter(this.Parameters.OutputFilePath);

            if (this.Parameters.OperationType == LineNumberSelectionType.Last
                || this.Parameters.OperationType == LineNumberSelectionType.NotLast)
            {
                this.SizeLimitedQueue = new Queue<string>();
            }

            switch (this.Parameters.OperationType)
            {
                case LineNumberSelectionType.First:
                case LineNumberSelectionType.NotFirst:
                case LineNumberSelectionType.Last:
                case LineNumberSelectionType.NotLast:
                case LineNumberSelectionType.Each:
                case LineNumberSelectionType.NotEach:
                    ArgumentChecker.CheckPresence(this.Parameters.FirstArgument);

                    this.FirstArgumentAsULong = ulong.Parse(this.Parameters.FirstArgument);

                    ArgumentChecker.CheckNotZero(this.FirstArgumentAsULong);
                    break;

                case LineNumberSelectionType.Between:
                case LineNumberSelectionType.NotBetween:
                    ArgumentChecker.CheckPresence(this.Parameters.FirstArgument);
                    ArgumentChecker.CheckPresence(this.Parameters.SecondArgument);

                    this.FirstArgumentAsULong = ulong.Parse(this.Parameters.FirstArgument);
                    this.SecondArgumentAsULong = ulong.Parse(this.Parameters.SecondArgument);

                    ArgumentChecker.CheckNotZero(this.FirstArgumentAsULong);
                    ArgumentChecker.CheckNotZero(this.SecondArgumentAsULong);
                    break;

                default:
                    throw new ProteusException($"Internal error: Proteus is not handling line number comparison type '{this.Parameters.OperationType}'!");
            }
        }

        public bool Execute(ulong lineNumber, string line)
        {
            DataProcessorValidation.ValidateLine(line);

            // Perform the comparison to decide whether to output the line.
            //
            // By default, we will output the line and return true.
            // To skip outputting a line, we return true.
            // To stop further file processing, we return false.
            //
            switch (this.Parameters.OperationType)
            {
                case LineNumberSelectionType.First:
                    // Once we're done outputting the first lines, we can abort processing the rest of the file.
                    //
                    if (lineNumber > this.FirstArgumentAsULong)
                    {
                        return false;
                    }
                    break;

                case LineNumberSelectionType.NotFirst:
                    // Skip first lines until we reach the line from which we start to output.
                    //
                    if (lineNumber <= this.FirstArgumentAsULong)
                    {
                        return true;
                    }
                    break;

                case LineNumberSelectionType.Last:
                    // Keep enqueuing lines into a queue.
                    // Once the queue contains as many lines as we want to output,
                    // we'll remove a line before adding a new one, to keep the queue size constant.
                    // By the time we finish processing the file, the queue will contain all lines
                    // that we need to output and we'll output them in CompleteExecution().
                    if ((ulong)this.SizeLimitedQueue.Count == this.FirstArgumentAsULong)
                    {
                        this.SizeLimitedQueue.Dequeue();
                    }

                    this.SizeLimitedQueue.Enqueue(line);

                    return true;

                case LineNumberSelectionType.NotLast:
                    string lineToOutput = null;

                    // Keep enqueuing lines into a queue.
                    // Once the queue contains as many lines as we don't want to output,
                    // we'll remove a line when adding a new one, to keep the queue size constant;
                    // then we will output the line that we just removed.
                    if ((ulong)this.SizeLimitedQueue.Count == this.FirstArgumentAsULong)
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

                case LineNumberSelectionType.Between:
                    // Skip first lines until we reach the line from which we start to output.
                    //
                    if (lineNumber < this.FirstArgumentAsULong)
                    {
                        return true;
                    }
                    // Abort further processing once we've completed outputting the lines we wanted.
                    //
                    else if (lineNumber > this.SecondArgumentAsULong)
                    {
                        return false;
                    }
                    break;

                case LineNumberSelectionType.NotBetween:
                    // Skip the lines in the interval that we don't want to output.
                    //
                    if (lineNumber >= this.FirstArgumentAsULong
                        && lineNumber <= this.SecondArgumentAsULong)
                    {
                        return true;
                    }
                    break;

                case LineNumberSelectionType.Each:
                    // Skip the lines whose numbers are not multiples of our argument.
                    //
                    if (lineNumber % this.FirstArgumentAsULong > 0)
                    {
                        return true;
                    }
                    break;

                case LineNumberSelectionType.NotEach:
                    // Skip the lines whose numbers are multiples of our argument.
                    //
                    if (lineNumber % this.FirstArgumentAsULong == 0)
                    {
                        return true;
                    }
                    break;

                default:
                    throw new ProteusException($"Internal error: Proteus is not handling line number comparison type '{this.Parameters.OperationType}'!");
            }

            this.OutputWriter.WriteLine(line);

            return true;
        }

        public override void CompleteExecution()
        {
            if (this.Parameters.OperationType == LineNumberSelectionType.Last)
            {
                if (this.SizeLimitedQueue == null || this.SizeLimitedQueue.Count == 0)
                {
                    throw new ProteusException("Internal error: No lines could be found to output for LineNumberComparisonType.Last!");
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