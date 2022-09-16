////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.IO;

using LaurentiuCristofor.Proteus.Common;
using LaurentiuCristofor.Proteus.DataExtractors;
using LaurentiuCristofor.Proteus.DataProcessors;
using LaurentiuCristofor.Proteus.DataProcessors.Builders;

namespace LaurentiuCristofor.Proteus.FileProcessors
{
    /// <summary>
    /// The core template for processing two files serially.
    /// </summary>
    /// <typeparam name="TFirstDataExtractor">The type of data extractor that will be applied to each row of the first file.</typeparam>
    /// <typeparam name="TFirstExtractionParameters">The type of parameters of the first data extraction operation.</typeparam>
    /// <typeparam name="TFirstExtractedData">The type of data that is produced by the first extractor.</typeparam>
    /// <typeparam name="TLookupDataStructureBuilder">The type of lookup data structure builder that is used on the first extracted data.</typeparam>
    /// <typeparam name="TLookupDataStructure">The type of lookup data structure that is built.</typeparam>
    /// <typeparam name="TSecondDataExtractor">The type of data extractor that will be applied to each row of the second file.</typeparam>
    /// <typeparam name="TSecondExtractionParameters">The type of parameters of the second data extraction operation.</typeparam>
    /// <typeparam name="TSecondExtractedData">The type of data that is produced by the second extractor.</typeparam>
    /// <typeparam name="TDataProcessor">The type of data processor that will process the second extracted data.</typeparam>
    /// <typeparam name="TProcessingParameters">The type of parameters of the processing operation.</typeparam>
    public class SerialTwoFileProcessor<
        TFirstDataExtractor, TFirstExtractionParameters, TFirstExtractedData,
        TLookupDataStructureBuilder, TLookupDataStructure,
        TSecondDataExtractor, TSecondExtractionParameters, TSecondExtractedData,
        TDataProcessor, TProcessingParameters>
        where TFirstDataExtractor : IDataExtractor<TFirstExtractionParameters, TFirstExtractedData>, new()
        where TLookupDataStructureBuilder : ILookupDataStructureBuilder<TFirstExtractedData, TLookupDataStructure>, new()
        where TSecondDataExtractor : IDataExtractor<TSecondExtractionParameters, TSecondExtractedData>, new()
        where TDataProcessor : IDataLookupProcessor<TProcessingParameters, TLookupDataStructure, TSecondExtractedData>, new()
    {
        /// <summary>
        /// The path to the first file to process.
        /// </summary>
        protected string FirstInputFilePath { get; set; }

        /// <summary>
        /// The path to the second file to process.
        /// </summary>
        protected string SecondInputFilePath { get; set; }

        /// <summary>
        /// The object that we'll use to read the input files.
        /// </summary>
        protected StreamReader InputReader { get; set; }

        /// <summary>
        /// A line counter.
        /// </summary>
        protected ulong LineCounter { get; set; }

        /// <summary>
        /// The data extractor that will be applied to each line of the first file.
        /// </summary>
        protected TFirstDataExtractor FirstDataExtractor { get; set; }

        /// <summary>
        /// The lookup data structure builder that will operate on the content extracted from the first file.
        /// </summary>
        protected TLookupDataStructureBuilder LookupDataStructureBuilder { get; set; }

        /// <summary>
        /// The data structure that will be constructed from the content extracted from the first file.
        /// </summary>
        protected TLookupDataStructure LookupDataStructure { get; set; }

        /// <summary>
        /// The data extractor that will be applied to each line of the second file.
        /// </summary>
        protected TSecondDataExtractor SecondDataExtractor { get; set; }

        /// <summary>
        /// The data processor that will be applied to the output of the second extraction.
        /// </summary>
        protected TDataProcessor DataProcessor { get; set; }

        public SerialTwoFileProcessor(
            string firstInputFilePath, string secondInputFilePath,
            TFirstExtractionParameters firstExtractionParameters, TSecondExtractionParameters secondExtractionParameters, TProcessingParameters processingParameters)
        {
            this.FirstDataExtractor = new TFirstDataExtractor();
            this.FirstDataExtractor.Initialize(firstExtractionParameters);

            this.LookupDataStructureBuilder = new TLookupDataStructureBuilder(); 

            this.SecondDataExtractor = new TSecondDataExtractor();
            this.SecondDataExtractor.Initialize(secondExtractionParameters);

            this.DataProcessor = new TDataProcessor();
            this.DataProcessor.Initialize(processingParameters);

            this.FirstInputFilePath = firstInputFilePath;
            this.SecondInputFilePath = secondInputFilePath;

            // Quickly check existence of second file, to avoid processing the first file for nothing.
            //
            this.InputReader = new StreamReader(this.SecondInputFilePath);
            this.InputReader.Close();
            this.InputReader = new StreamReader(this.FirstInputFilePath);

            this.LineCounter = 0;
        }

        /// <summary>
        /// Processes the input file.
        /// </summary>
        public void ProcessFiles()
        {
            while (ProcessNextRowOfFirstFile())
            {
                // Nothing else needs to be done, but to process each row.
            }

            // Switch reader to second input file.
            //
            this.InputReader = new StreamReader(this.SecondInputFilePath);

            // Reset line counter.
            //
            this.LineCounter = 0;
            ProgressTracker.Reset();

            // Add lookup data structure that we just built to the data processor.
            //
            this.DataProcessor.AddLookupDataStructure(this.LookupDataStructure);

            while (ProcessNextRowOfSecondFile())
            {
                // Nothing else needs to be done, but to process each row.
            }
        }

        /// <summary>
        /// Processes the next row in the first input file.
        /// </summary>
        /// <returns>True if processing should continue; false otherwise.</returns>
        private bool ProcessNextRowOfFirstFile()
        {
            // Read next line.
            //
            string nextRow = this.InputReader.ReadLine();

            // Check for end of file.
            //
            if (nextRow == null)
            {
                return EndFirstFileProcessing();
            }

            // Count line and track progress.
            //
            this.LineCounter++;
            ProgressTracker.Track(this.LineCounter);

            // Empty lines will be skipped.
            //
            if (String.IsNullOrEmpty(nextRow))
            {
                return true;
            }

            // Perform the extraction step.
            //
            TFirstExtractedData nextData = this.FirstDataExtractor.ExtractData(this.LineCounter, nextRow);

            // Then perform the lookup data structure building step.
            //
            this.LookupDataStructure = this.LookupDataStructureBuilder.Execute(nextData);

            return true;
        }

        /// <summary>
        /// Processes the next row in the second input file.
        /// </summary>
        /// <returns>True if processing should continue; false otherwise.</returns>
        private bool ProcessNextRowOfSecondFile()
        {
            // Read next line.
            //
            string nextRow = this.InputReader.ReadLine();

            // Check for end of file.
            //
            if (nextRow == null)
            {
                return EndSecondFileProcessing();
            }

            // Count line and track progress.
            //
            this.LineCounter++;
            ProgressTracker.Track(this.LineCounter);

            // Empty lines will be skipped.
            //
            if (String.IsNullOrEmpty(nextRow))
            {
                return true;
            }

            // Perform the extraction step.
            //
            TSecondExtractedData nextData = this.SecondDataExtractor.ExtractData(this.LineCounter, nextRow);

            // Then perform the processing step.
            // Check the result for an early processing termination.
            //
            if (!this.DataProcessor.Execute(this.LineCounter, nextData))
            {
                return EndSecondFileProcessing();
            }

            return true;
        }

        /// <summary>
        /// Finalizes the processing of the first file.
        /// </summary>
        /// <returns>Always returns false to indicate that execution should terminate.</returns>
        private bool EndFirstFileProcessing()
        {
            this.InputReader.Close();

            OutputInterface.LogLine($"\n{this.LineCounter} lines were read from file { Path.GetFileName(this.FirstInputFilePath)}.");

            return false;
        }

        /// <summary>
        /// Finalizes the processing of the second file.
        /// </summary>
        /// <returns>Always returns false to indicate that execution should terminate.</returns>
        private bool EndSecondFileProcessing()
        {
            this.DataProcessor.CompleteExecution();

            this.InputReader.Close();

            OutputInterface.LogLine($"\n{this.LineCounter} lines were read from file { Path.GetFileName(this.SecondInputFilePath)}.");

            return false;
        }
    }
}
