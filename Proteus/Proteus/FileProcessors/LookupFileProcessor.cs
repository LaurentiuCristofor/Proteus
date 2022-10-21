////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
///
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System.IO;

using LaurentiuCristofor.Proteus.Common;
using LaurentiuCristofor.Proteus.Common.Logging;
using LaurentiuCristofor.Proteus.Common.Utilities;
using LaurentiuCristofor.Proteus.DataExtractors;
using LaurentiuCristofor.Proteus.DataProcessors.Lookup;

namespace LaurentiuCristofor.Proteus.FileProcessors
{
    /// <summary>
    /// The core template for processing a first file to build a lookup data structure and then processing a second file using that lookup data structure.
    /// </summary>
    /// <typeparam name="TDataExtractor">The type of data extractor that will be applied to each row of the data file.</typeparam>
    /// <typeparam name="TExtractionParameters">The type of parameters of the first data file extraction operation.</typeparam>
    /// <typeparam name="TExtractedData">The type of data that is produced by the data file extractor.</typeparam>
    /// <typeparam name="TLookupDataExtractor">The type of data extractor that will be applied to each row of the lookup file.</typeparam>
    /// <typeparam name="TLookupExtractionParameters">The type of parameters of the lookup file extraction operation.</typeparam>
    /// <typeparam name="TLookupExtractedData">The type of data that is produced by the lookup file extractor.</typeparam>
    /// <typeparam name="TLookupDataStructureBuilder">The type of lookup data structure builder that is used on the lookup file extracted data.</typeparam>
    /// <typeparam name="TLookupDataStructure">The type of lookup data structure that is built.</typeparam>
    /// <typeparam name="TDataProcessor">The type of data processor that will process the data extracted from the data file.</typeparam>
    /// <typeparam name="TProcessingParameters">The type of parameters of the processing operation.</typeparam>
    public class LookupFileProcessor<
        TDataExtractor, TExtractionParameters, TExtractedData,
        TLookupDataExtractor, TLookupExtractionParameters, TLookupExtractedData,
        TLookupDataStructureBuilder, TLookupDataStructure,
        TDataProcessor, TProcessingParameters>
        where TDataExtractor : class, IDataExtractor<TExtractionParameters, TExtractedData>, new()
        where TLookupDataExtractor : class, IDataExtractor<TLookupExtractionParameters, TLookupExtractedData>, new()
        where TLookupDataStructureBuilder : class, ILookupDataStructureBuilder<TLookupExtractedData, TLookupDataStructure>, new()
        where TDataProcessor : class, IDataLookupProcessor<TProcessingParameters, TLookupDataStructure, TExtractedData>, new()
        where TExtractionParameters : class
        where TLookupExtractionParameters : class
        where TProcessingParameters : class
        where TExtractedData : class
        where TLookupExtractedData : class
        where TLookupDataStructure : class
    {
        /// <summary>
        /// The path to the data file to process.
        /// </summary>
        protected string DataFilePath { get; set; }

        /// <summary>
        /// The path to the lookup file to process.
        /// </summary>
        protected string LookupFilePath { get; set; }

        /// <summary>
        /// The reader that we'll use with the input files.
        /// </summary>
        protected StreamReader InputReader { get; set; }

        /// <summary>
        /// A line counter.
        /// </summary>
        protected ulong LineCounter { get; set; }

        /// <summary>
        /// The data extractor that will be applied to each line of the data file.
        /// </summary>
        protected TDataExtractor DataFileExtractor { get; set; }

        /// <summary>
        /// The data extractor that will be applied to each line of the lookup file.
        /// </summary>
        protected TLookupDataExtractor LookupFileExtractor { get; set; }

        /// <summary>
        /// The lookup data structure builder that will operate on the content extracted from the lookup file.
        /// </summary>
        protected TLookupDataStructureBuilder LookupDataStructureBuilder { get; set; }

        /// <summary>
        /// The data structure that will be constructed from the content extracted from the lookup file.
        /// </summary>
        protected TLookupDataStructure LookupDataStructure { get; set; }

        /// <summary>
        /// The data processor that will be applied to the output of the data file extraction.
        /// </summary>
        protected TDataProcessor DataProcessor { get; set; }

        public LookupFileProcessor(
            string dataFilePath, TExtractionParameters dataFileExtractionParameters,
            string lookupFilePath, TLookupExtractionParameters lookupFileExtractionParameters,
            TProcessingParameters processingParameters)
        {
            DataFilePath = dataFilePath;

            DataFileExtractor = new TDataExtractor();
            DataFileExtractor.Initialize(dataFileExtractionParameters);

            LookupFilePath = lookupFilePath;

            LookupFileExtractor = new TLookupDataExtractor();
            LookupFileExtractor.Initialize(lookupFileExtractionParameters);

            LookupDataStructureBuilder = new TLookupDataStructureBuilder();

            DataProcessor = new TDataProcessor();
            DataProcessor.Initialize(processingParameters);

            // Quickly check existence of data file, to avoid processing the lookup file for nothing.
            //
            InputReader = new StreamReader(DataFilePath);
            InputReader.Close();

            InputReader = new StreamReader(LookupFilePath);

            LineCounter = 0;
        }

        /// <summary>
        /// Processes the input files.
        /// </summary>
        public void ProcessFiles()
        {
            Timer timer = new Timer($"\n{Constants.Messages.TotalProcessingTime}");

            while (ProcessNextRowOfLookupFile())
            {
                // Nothing else needs to be done, but to process each row.
            }

            // Switch reader to data file.
            //
            InputReader = new StreamReader(DataFilePath);

            // Reset line counter.
            // It's not necessary to reset the progress tracker
            // because the status message from the end of the lookup file processing does that.
            //
            LineCounter = 0;

            // Add lookup data structure that we just built to the data processor.
            //
            DataProcessor.AddLookupDataStructure(LookupDataStructure);

            while (ProcessNextRowOfDataFile())
            {
                // Nothing else needs to be done, but to process each row.
            }

            timer.StopAndReport();
        }

        /// <summary>
        /// Processes the next row in the lookup file.
        /// </summary>
        /// <returns>True if processing should continue; false otherwise.</returns>
        protected bool ProcessNextRowOfLookupFile()
        {
            // Read next line.
            //
            string nextRow = InputReader.ReadLine();

            // Check for end of file.
            //
            if (nextRow == null)
            {
                return EndLookupFileProcessing();
            }

            // Count line and track progress.
            //
            LineCounter++;
            ProgressTracker.Track(LineCounter);

            // Perform the extraction step.
            //
            TLookupExtractedData nextData = LookupFileExtractor.ExtractData(LineCounter, nextRow);

            // Skip lines from which we could not extract data.
            //
            if (nextData == null)
            {
                return true;
            }

            // Then perform the lookup data structure building step.
            //
            LookupDataStructure = LookupDataStructureBuilder.Execute(nextData);

            return true;
        }

        /// <summary>
        /// Processes the next row in the data file.
        /// </summary>
        /// <returns>True if processing should continue; false otherwise.</returns>
        protected bool ProcessNextRowOfDataFile()
        {
            // Read next line.
            //
            string nextRow = InputReader.ReadLine();

            // Check for end of file.
            //
            if (nextRow == null)
            {
                return EndDataFileProcessing();
            }

            // Count line and track progress.
            //
            LineCounter++;
            ProgressTracker.Track(LineCounter);

            // Perform the extraction step.
            //
            TExtractedData nextData = DataFileExtractor.ExtractData(LineCounter, nextRow);

            // Skip lines from which we could not extract data.
            //
            if (nextData == null)
            {
                return true;
            }

            // Then perform the processing step.
            // Check the result for an early processing termination.
            //
            if (!DataProcessor.Execute(LineCounter, nextData))
            {
                return EndDataFileProcessing();
            }

            return true;
        }

        /// <summary>
        /// Finalizes the processing of the lookup file.
        /// </summary>
        /// <returns>Always returns false to indicate that execution should terminate.</returns>
        protected bool EndLookupFileProcessing()
        {
            InputReader.Close();

            LoggingManager.GetLogger().LogLine($"\n{LineCounter:N0} {Constants.Messages.LinesReadFromLookupFile} '{Path.GetFileName(LookupFilePath)}'.");

            return false;
        }

        /// <summary>
        /// Finalizes the processing of the data file.
        /// </summary>
        /// <returns>Always returns false to indicate that execution should terminate.</returns>
        protected bool EndDataFileProcessing()
        {
            DataProcessor.CompleteExecution();

            InputReader.Close();

            LoggingManager.GetLogger().LogLine($"\n{LineCounter:N0} {Constants.Messages.LinesReadFromDataFile} '{Path.GetFileName(DataFilePath)}'.");

            return false;
        }
    }
}
