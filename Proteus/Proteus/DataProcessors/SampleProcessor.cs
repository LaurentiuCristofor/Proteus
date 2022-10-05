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
        /// Data structure used for holding the sample lines.
        /// </summary>
        protected List<string> SampleLines { get; set; }

        public void Initialize(OutputIntegerParameters processingParameters)
        {
            this.Parameters = processingParameters;

            this.Sampler = new UnknownTotalSampler(this.Parameters.IntegerValue);

            this.SampleLines = new List<string>();

            this.OutputWriter = new FileWriter(this.Parameters.OutputFilePath, trackProgress: true);
        }

        public bool Execute(ulong lineNumber, string line)
        {
            if (lineNumber <= (ulong)this.Parameters.IntegerValue)
            {
                this.SampleLines.Add(line);
            }
            else
            {
                // Check if the current line should be replace a sample line.
                //
                int sampleLineReplacementNumber = this.Sampler.EvaluateAnotherElement();
                if (sampleLineReplacementNumber > 0)
                {
                    this.SampleLines[sampleLineReplacementNumber - 1] = line;
                }
            }

            return true;
        }

        public override void CompleteExecution()
        {
            if (this.SampleLines.Count < this.Parameters.IntegerValue)
            {
                throw new ProteusException($"The input file is smaller than the requested sample size! The requested sample size was {this.Parameters.IntegerValue} but only {this.SampleLines.Count} lines were found.");
            }

            foreach (string line in this.SampleLines)
            {
                this.OutputWriter.WriteLine(line);
            }

            base.CompleteExecution();
        }
    }
}
