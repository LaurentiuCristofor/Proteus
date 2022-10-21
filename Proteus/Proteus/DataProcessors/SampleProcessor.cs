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
    /// 
    /// OutputExtraParameters is expected to contain:
    /// IntParameters[0] - randomization seed value
    /// IntParameters[1] - sample size
    /// </summary>
    public class SampleProcessor : BaseOutputProcessor, IDataProcessor<OutputExtraParameters, string>
    {
        protected const int SeedIndex = 0;
        protected const int SampleSizeIndex = 1;

        /// <summary>
        /// The sample size parameter.
        /// </summary>
        protected int SampleSize { get; set; }

        /// <summary>
        /// The sampler used to sample the lines.
        /// </summary>
        protected UnknownTotalSampler Sampler { get; set; }

        /// <summary>
        /// Data structure used for holding the sample lines along their line numbers.
        /// </summary>
        protected List<DataPair<ulong, string>> SampleLinesWithNumbers { get; set; }

        public void Initialize(OutputExtraParameters processingParameters)
        {
            ArgumentChecker.CheckPresence(processingParameters.IntParameters, SeedIndex);
            ArgumentChecker.CheckPresence(processingParameters.IntParameters, SampleSizeIndex);

            int seed = processingParameters.IntParameters[SeedIndex];
            SampleSize = processingParameters.IntParameters[SampleSizeIndex];

            Random randomGenerator = (seed >= 0) ? new Random(seed) : new Random();
            Sampler = new UnknownTotalSampler(SampleSize, randomGenerator);

            SampleLinesWithNumbers = new List<DataPair<ulong, string>>();

            OutputWriter = new FileWriter(processingParameters.OutputFilePath, trackProgress: true);
        }

        public bool Execute(ulong lineNumber, string line)
        {
            if (lineNumber <= (ulong)SampleSize)
            {
                SampleLinesWithNumbers.Add(new DataPair<ulong, string>(lineNumber, line));
            }
            else
            {
                // Check if the current line should replace a sample line.
                // Convert the number to an index for the replacement.
                //
                int sampleLineReplacementNumber = Sampler.EvaluateAnotherElement();
                if (sampleLineReplacementNumber > 0)
                {
                    SampleLinesWithNumbers[sampleLineReplacementNumber - 1] = new DataPair<ulong, string>(lineNumber, line);
                }
            }

            return true;
        }

        public override void CompleteExecution()
        {
            if (SampleLinesWithNumbers.Count < SampleSize)
            {
                throw new ProteusException($"The input file is smaller than the requested sample size! The requested sample size was {SampleSize} but only {SampleLinesWithNumbers.Count} lines were found.");
            }

            Timer timer = new Timer($"\n{Constants.Messages.SortingStart}", Constants.Messages.SortingEnd, countFinalLineEndings: 0);
            SampleLinesWithNumbers.Sort();
            timer.StopAndReport();

            foreach (DataPair<ulong, string> dataPair in SampleLinesWithNumbers)
            {
                OutputWriter.WriteLine(dataPair.SecondData);
            }

            base.CompleteExecution();
        }
    }
}
