﻿////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.IO;

using LaurentiuCristofor.Proteus.Common;
using LaurentiuCristofor.Proteus.DataExtractors;
using LaurentiuCristofor.Proteus.DataProcessors;

namespace LaurentiuCristofor.Proteus.FileProcessors
{
    /// <summary>
    /// The core template for processing a single file using a data extractor and a data processor.
    /// </summary>
    /// <typeparam name="TDataExtractor">The data extractor will be applied to each row to produce the processing input.</typeparam>
    /// <typeparam name="TExtractionParameters">These define the parameters of the data extraction operation.</typeparam>
    /// <typeparam name="TData">The type of data that is produced by the extractor to be processed by the processor.</typeparam>
    /// <typeparam name="TDataProcessor">The data processor will process the input data and may also generate additional output.</typeparam>
    /// <typeparam name="TProcessingParameters">These define the parameters of the processing operation.</typeparam>
    public class TextFileProcessor<TDataExtractor, TExtractionParameters, TData, TDataProcessor, TProcessingParameters>
        where TDataExtractor : IDataExtractor<TExtractionParameters, TData>, new()
        where TDataProcessor : IDataProcessor<TProcessingParameters, TData>, new()
    {
        /// <summary>
        /// The path to the file to process.
        /// </summary>
        protected string InputFilePath { get; set; }

        /// <summary>
        /// The object that we'll use to read the input file.
        /// </summary>
        protected StreamReader InputReader { get; set; }

        /// <summary>
        /// A line counter.
        /// </summary>
        protected ulong LineCounter { get; set; }

        /// <summary>
        /// The data extractor that will be applied to each line.
        /// </summary>
        protected TDataExtractor DataExtractor { get; set; }

        /// <summary>
        /// The data processor that will be applied to the output of each extraction.
        /// </summary>
        protected TDataProcessor DataProcessor { get; set; }

        public TextFileProcessor(string inputFilePath, TExtractionParameters extractionParameters, TProcessingParameters processingParameters)
        {
            this.DataExtractor = new TDataExtractor();
            this.DataExtractor.Initialize(extractionParameters);

            this.DataProcessor = new TDataProcessor();
            this.DataProcessor.Initialize(processingParameters);

            this.InputFilePath = inputFilePath;
            this.InputReader = new StreamReader(this.InputFilePath);
            this.LineCounter = 0;
        }

        /// <summary>
        /// Processes the input file.
        /// </summary>
        public void ProcessFile()
        {
            while (ProcessNextRow())
            {
                // Nothing else needs to be done, but to process each row.
            }
        }

        /// <summary>
        /// Processes the next row in the input file.
        /// </summary>
        /// <returns>True if processing should continue; false otherwise.</returns>
        private bool ProcessNextRow()
        {
            // Read next line.
            //
            string nextRow = this.InputReader.ReadLine();

            // Check for end of file.
            //
            if (nextRow == null)
            {
                return EndProcessing();
            }

            // Count line and track progress.
            //
            this.LineCounter++;
            ProgressTracker.Track(this.LineCounter);

            // Perform the extraction step.
            //
            TData nextData = this.DataExtractor.ExtractData(this.LineCounter, nextRow);

            // Then perform the processing step.
            // Check the result for an early processing termination.
            //
            if (!this.DataProcessor.Execute(this.LineCounter, nextData))
            {
                return EndProcessing();
            }

            return true;
        }

        /// <summary>
        /// Finalizes the processing.
        /// </summary>
        /// <returns>Always returns false to indicate that execution should terminate.</returns>
        private bool EndProcessing()
        {
            OutputInterface.LogLine($"\n{this.LineCounter} lines were read from file { Path.GetFileName(this.InputFilePath)}.");

            this.DataProcessor.CompleteExecution();

            this.InputReader.Close();

            return false;
        }
    }
}
