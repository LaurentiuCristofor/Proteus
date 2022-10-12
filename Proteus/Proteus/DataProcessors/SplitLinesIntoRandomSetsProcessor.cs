////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
///
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;

using LaurentiuCristofor.Proteus.Common;
using LaurentiuCristofor.Proteus.DataProcessors.Parameters;
using LaurentiuCristofor.Proteus.FileOperations;

namespace LaurentiuCristofor.Proteus.DataProcessors
{
    /// <summary>
    /// A data processor that randomly splits a file's content into a number of files.
    /// 
    /// OutputParameters is expected to contain:
    /// StringParameters[0] - output file extension
    /// IntParameters[0] - randomization seed value
    /// IntParameters[1] - count of sets
    /// </summary>
    public class SplitLinesIntoRandomSetsProcessor : BaseOutputProcessor, IDataProcessor<OutputParameters, string>
    {
        protected const int OutputFileExtensionIndex = 0;
        protected const int SeedIndex = 0;
        protected const int SetsCountIndex = 1;

        /// <summary>
        /// The count of sets into which we should split the input file's lines.
        /// </summary>
        protected int SetsCount { get; set; }

        /// <summary>
        /// A dictionary to help us manage the file writers that we will use for each line set.
        /// </summary>
        protected Dictionary<int, FileWriter> MapSetToFileWriter { get; set; }

        /// <summary>
        /// The uniform random number generator that we will use for distributing lines to sets.
        /// </summary>
        protected System.Random RandomGenerator { get; set; }

        public void Initialize(OutputParameters processingParameters)
        {
            ArgumentChecker.CheckPresence<string>(processingParameters.StringParameters, OutputFileExtensionIndex);
            ArgumentChecker.CheckNotNullAndNotEmpty(processingParameters.StringParameters[OutputFileExtensionIndex]);
            string outputFileExtension = processingParameters.StringParameters[OutputFileExtensionIndex];

            ArgumentChecker.CheckPresence<int>(processingParameters.IntParameters, SeedIndex);
            int seed = processingParameters.IntParameters[SeedIndex];

            ArgumentChecker.CheckPresence<int>(processingParameters.IntParameters, SetsCountIndex);
            ArgumentChecker.CheckStrictlyPositive(processingParameters.IntParameters[SetsCountIndex]);
            this.SetsCount = processingParameters.IntParameters[SetsCountIndex];

            this.MapSetToFileWriter = new Dictionary<int, FileWriter>();
            for (int i = 1; i <= this.SetsCount; ++i)
            {
                this.MapSetToFileWriter.Add(i, new FileWriter(processingParameters.OutputFilePath + $".{i}{outputFileExtension}"));
            }

            this.RandomGenerator = (seed >= 0) ? new System.Random(seed) : new System.Random();
        }

        public bool Execute(ulong lineNumber, string line)
        {
            // Pick a random set to place the line in.
            //
            int setNumber = this.RandomGenerator.Next(1, this.SetsCount + 1);

            this.MapSetToFileWriter[setNumber].WriteLine(line);

            return true;
        }

        public override void CompleteExecution()
        {
            foreach (int setNumber in this.MapSetToFileWriter.Keys)
            {
                this.MapSetToFileWriter[setNumber].CloseAndReport();
            }

            base.CompleteExecution();
        }
    }
}
