////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
///
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using LaurentiuCristofor.Proteus.Common;
using LaurentiuCristofor.Proteus.Common.Utilities;
using LaurentiuCristofor.Proteus.DataGenerators;
using LaurentiuCristofor.Proteus.DataProcessors.Parameters;
using LaurentiuCristofor.Proteus.FileOperations;

namespace LaurentiuCristofor.Proteus.FileProcessors
{
    /// <summary>
    /// The core template for generating a file using a data generator.
    /// </summary>
    /// <typeparam name="TDataGenerator">The type of data generator that will be used.</typeparam>
    /// <typeparam name="TGenerationParameters">The type of parameters of the data generation operation.</typeparam>
    public class FileGenerationProcessor<TDataGenerator, TGenerationParameters>
        where TDataGenerator : class, IDataGenerator<TGenerationParameters>, new()
        where TGenerationParameters : class
    {
        /// <summary>
        /// The data generator that will be used to produce each output line.
        /// </summary>
        protected TDataGenerator DataGenerator { get; set; }

        /// <summary>
        /// A writer for the output file.
        /// </summary>
        protected FileWriter OutputWriter { get; set; }

        public FileGenerationProcessor(TGenerationParameters generationParameters, BaseOutputParameters processingParameters)
        {
            this.DataGenerator = new TDataGenerator();
            this.DataGenerator.Initialize(generationParameters);

            this.OutputWriter = new FileWriter(processingParameters.OutputFilePath, trackProgress: true);
        }

        /// <summary>
        /// Generates the output file.
        /// </summary>
        public void GenerateFile()
        {
            Timer timer = new Timer($"\n{Constants.Messages.TotalProcessingTime}");

            while (GenerateNextRow())
            {
                // Nothing else needs to be done, but to generate each row.
            }

            timer.StopAndReport();
        }

        /// <summary>
        /// Generates the next row and writes it to the output file.
        /// </summary>
        /// <returns>True if processing should continue; false otherwise.</returns>
        protected bool GenerateNextRow()
        {
            // Perform the generation step.
            //
            string nextLine = this.DataGenerator.Generate();

            // Generation stops when a null line is returned.
            //
            if (nextLine == null)
            {
                this.OutputWriter.CloseAndReport();

                return false;
            }

            this.OutputWriter.WriteLine(nextLine);

            return true;
        }
    }
}
