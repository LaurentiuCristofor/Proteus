﻿////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// 
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System.IO;

using LaurentiuCristofor.Proteus.Common;
using LaurentiuCristofor.Proteus.Common.Utilities;
using LaurentiuCristofor.Proteus.DataExtractors;
using LaurentiuCristofor.Proteus.DataProcessors;

namespace LaurentiuCristofor.Proteus.FileProcessors
{
    /// <summary>
    /// The core template for processing a single file using a data extractor and a data processor.
    /// </summary>
    /// <typeparam name="TDataExtractor">The type of data extractor that will be applied to each row to produce the processing input.</typeparam>
    /// <typeparam name="TExtractionParameters">The type of parameters of the data extraction operation.</typeparam>
    /// <typeparam name="TData">The type of data that is produced by the extractor to be processed by the processor.</typeparam>
    /// <typeparam name="TDataProcessor">The type of data processor that will process the extracted data.</typeparam>
    /// <typeparam name="TProcessingParameters">The type of parameters of the processing operation.</typeparam>
    public class FileProcessor<TDataExtractor, TExtractionParameters, TData, TDataProcessor, TProcessingParameters>
        where TDataExtractor : class, IDataExtractor<TExtractionParameters, TData>, new()
        where TDataProcessor : class, IDataProcessor<TProcessingParameters, TData>, new()
        where TExtractionParameters : class
        where TProcessingParameters : class
        where TData : class
    {
        /// <summary>
        /// The path to the file to process.
        /// </summary>
        protected string InputFilePath { get; set; }

        /// <summary>
        /// The reader that we'll use with the input file.
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

        public FileProcessor(string inputFilePath, TExtractionParameters extractionParameters, TProcessingParameters processingParameters)
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
        protected bool ProcessNextRow()
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

            // Skip lines from which we could not extract data.
            //
            if (nextData == null)
            {
                return true;
            }

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
        protected bool EndProcessing()
        {
            this.DataProcessor.CompleteExecution();

            this.InputReader.Close();

            OutputInterface.LogLine($"\n{this.LineCounter:N0} lines were read from file '{Path.GetFileName(this.InputFilePath)}'.");

            return false;
        }
    }
}
