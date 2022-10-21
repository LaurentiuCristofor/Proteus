////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
///
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

using LaurentiuCristofor.Proteus.Common;
using LaurentiuCristofor.Proteus.Common.Algorithms;
using LaurentiuCristofor.Proteus.Common.Random;
using LaurentiuCristofor.Proteus.Common.Utilities;
using LaurentiuCristofor.Proteus.DataProcessors.Parameters;
using LaurentiuCristofor.Proteus.FileOperations;

namespace LaurentiuCristofor.Proteus.DataProcessors
{
    /// <summary>
    /// A data processor that shuffles the input lines.
    /// 
    /// OutputExtraParameters is expected to contain:
    /// IntParameters[0] - randomization seed value
    /// </summary>
    public class ShuffleProcessor : BaseOutputProcessor, IDataProcessor<OutputExtraParameters, string>
    {
        protected const int SeedIndex = 0;

        /// <summary>
        /// The randomization seed used for shuffling.
        /// A negative value indicates that no seed will be used.
        /// </summary>
        protected int Seed { get; set; }

        /// <summary>
        /// Data structure used for loading the lines before sorting them.
        /// </summary>
        protected List<string> Lines { get; set; }

        public void Initialize(OutputExtraParameters processingParameters)
        {
            Lines = new List<string>();

            ArgumentChecker.CheckPresence(processingParameters.IntParameters, SeedIndex);
            Seed = processingParameters.IntParameters[SeedIndex];

            OutputWriter = new FileWriter(processingParameters.OutputFilePath, trackProgress: true);
        }

        public bool Execute(ulong lineNumber, string line)
        {
            Lines.Add(line);

            return true;
        }

        public override void CompleteExecution()
        {
            if (Lines == null)
            {
                throw new ProteusException("Internal error: An expected data structure has not been initialized!");
            }

            Random randomGenerator = (Seed >= 0) ? new Random(Seed) : new Random();
            Shuffler shuffler = new Shuffler(Lines.Count, randomGenerator);

            Timer timer = new Timer($"\n{Constants.Messages.ShufflingStart}", Constants.Messages.ShufflingEnd, countFinalLineEndings: 0);

            while (true)
            {
                Tuple<int, int> shuffle = shuffler.Next();

                if (shuffle == null)
                {
                    break;
                }

                if (shuffle.Item1 == shuffle.Item2)
                {
                    continue;
                }

                // Convert shuffle numbers to indices.
                //
                int indexFirstLine = shuffle.Item1 - 1;
                int indexSecondLine = shuffle.Item2 - 1;

                // Exchange the lines indicated by the pair of indices.
                //
                SortingOperations.Exchange(Lines, indexFirstLine, indexSecondLine);
            }

            timer.StopAndReport();

            foreach (string line in Lines)
            {
                OutputWriter.WriteLine(line);
            }

            base.CompleteExecution();
        }
    }
}
