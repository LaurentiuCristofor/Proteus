////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
///
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

using LaurentiuCristofor.Proteus.Common;
using LaurentiuCristofor.Proteus.Common.Random;
using LaurentiuCristofor.Proteus.Common.Utilities;
using LaurentiuCristofor.Proteus.DataProcessors.Parameters;
using LaurentiuCristofor.Proteus.FileOperations;

namespace LaurentiuCristofor.Proteus.DataProcessors
{
    /// <summary>
    /// A data processor that samples the input lines.
    /// </summary>
    public class SampleProcessor : BaseOutputProcessor, IDataProcessor<OutputIntegerParameters, string>
    {
        protected OutputIntegerParameters Parameters { get; set; }

        /// <summary>
        /// The sampler used to sample the lines.
        /// </summary>
        protected UnknownTotalSampler Sampler { get; set; }

        /// <summary>
        /// Data structure used for holding the sample lines along their line numbers.
        /// </summary>
        protected List<Tuple<ulong, string>> SampleLinesWithNumbers { get; set; }

        public void Initialize(OutputIntegerParameters processingParameters)
        {
            this.Parameters = processingParameters;

            this.Sampler = new UnknownTotalSampler(this.Parameters.IntegerValue);

            this.SampleLinesWithNumbers = new List<Tuple<ulong, string>>();

            this.OutputWriter = new FileWriter(this.Parameters.OutputFilePath, trackProgress: true);
        }

        public bool Execute(ulong lineNumber, string line)
        {
            if (lineNumber <= (ulong)this.Parameters.IntegerValue)
            {
                this.SampleLinesWithNumbers.Add(new Tuple<ulong, string>(lineNumber, line));
            }
            else
            {
                // Check if the current line should replace a sample line.
                // Convert the number to an index for the replacement.
                //
                int sampleLineReplacementNumber = this.Sampler.EvaluateAnotherElement();
                if (sampleLineReplacementNumber > 0)
                {
                    this.SampleLinesWithNumbers[sampleLineReplacementNumber - 1] = new Tuple<ulong, string>(lineNumber, line);
                }
            }

            return true;
        }

        public override void CompleteExecution()
        {
            if (this.SampleLinesWithNumbers.Count < this.Parameters.IntegerValue)
            {
                throw new ProteusException($"The input file is smaller than the requested sample size! The requested sample size was {this.Parameters.IntegerValue} but only {this.SampleLinesWithNumbers.Count} lines were found.");
            }

            Timer timer = new Timer($"\n{Constants.Messages.SortingStart}", Constants.Messages.SortingEnd, countFinalLineEndings: 0);
            this.SampleLinesWithNumbers.Sort();
            timer.StopAndReport();

            foreach (Tuple<ulong, string> tuple in this.SampleLinesWithNumbers)
            {
                this.OutputWriter.WriteLine(tuple.Item2);
            }

            base.CompleteExecution();
        }
    }
}
