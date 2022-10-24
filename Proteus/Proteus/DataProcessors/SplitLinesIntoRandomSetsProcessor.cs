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
    /// OutputExtraParameters is expected to contain:
    /// StringParameters[0] - output file extension
    /// IntParameters[0] - randomization seed value
    /// IntParameters[1] - count of sets
    /// </summary>
    public class SplitLinesIntoRandomSetsProcessor : BaseOutputProcessor, IDataProcessor<OutputExtraParameters, string>
    {
        protected const int OutputFileExtensionIndex = 0;
        protected const int SeedIndex = 0;
        protected const int SetsCountIndex = 1;

        protected string OutputFilePath { get; set; }

        /// <summary>
        /// The file extension that should be used for the output files.
        /// </summary>
        protected string OutputFileExtension { get; set; }

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

        public void Initialize(OutputExtraParameters processingParameters)
        {
            OutputFilePath = processingParameters.OutputFilePath;

            ArgumentChecker.CheckPresence(processingParameters.StringParameters, OutputFileExtensionIndex);
            ArgumentChecker.CheckPresence(processingParameters.IntParameters, SeedIndex);
            ArgumentChecker.CheckPresence(processingParameters.IntParameters, SetsCountIndex);

            OutputFileExtension = processingParameters.StringParameters[OutputFileExtensionIndex];
            int seed = processingParameters.IntParameters[SeedIndex];
            SetsCount = processingParameters.IntParameters[SetsCountIndex];

            ArgumentChecker.CheckNotNullAndNotEmpty(OutputFileExtension);
            ArgumentChecker.CheckGreaterThanOrEqualTo(SetsCount, 2);

            MapSetToFileWriter = new Dictionary<int, FileWriter>();

            RandomGenerator = (seed >= 0) ? new System.Random(seed) : new System.Random();
        }

        public bool Execute(ulong lineNumber, string line)
        {
            // Pick a random set to place the line in.
            //
            int setNumber = RandomGenerator.Next(1, SetsCount + 1);

            // We create the writer for a set only if we have a row to place in it.
            // This avoids creating unnecessary empty files.
            //
            if (!MapSetToFileWriter.ContainsKey(setNumber))
            {
                MapSetToFileWriter.Add(setNumber, new FileWriter(OutputFilePath + $".{setNumber}{OutputFileExtension}"));
            }

            // Write the line into the chosen set's writer.
            //
            MapSetToFileWriter[setNumber].WriteLine(line);

            return true;
        }

        public override void CompleteExecution()
        {
            foreach (int setNumber in MapSetToFileWriter.Keys)
            {
                MapSetToFileWriter[setNumber].CloseAndReport();
            }

            base.CompleteExecution();
        }
    }
}
