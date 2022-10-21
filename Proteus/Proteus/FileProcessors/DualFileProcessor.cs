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
using LaurentiuCristofor.Proteus.DataProcessors.Dual;

namespace LaurentiuCristofor.Proteus.FileProcessors
{
    /// <summary>
    /// The core template for processing two files in parallel.
    /// </summary>
    /// <typeparam name="TDataExtractor">The type of data extractor that will be applied to each input file.</typeparam>
    /// <typeparam name="TExtractionParameters">The type of parameters of the extraction operations.</typeparam>
    /// <typeparam name="TExtractedData">The type of data that is produced by the extraction operations.</typeparam>
    /// <typeparam name="TDataProcessor">The type of data processor that will process the extracted data.</typeparam>
    /// <typeparam name="TProcessingParameters">The type of parameters of the processing operation.</typeparam>
    public class DualFileProcessor<
        TDataExtractor, TExtractionParameters, TExtractedData,
        TDataProcessor, TProcessingParameters>
        where TDataExtractor : IDataExtractor<TExtractionParameters, TExtractedData>, new()
        where TDataProcessor : IDualDataProcessor<TProcessingParameters, TExtractedData>, new()
        where TExtractionParameters : class
        where TProcessingParameters : class
        where TExtractedData : class
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
        /// The reader that we'll use with the first input file.
        /// </summary>
        protected StreamReader firstInputReader;

        /// <summary>
        /// The reader that we'll use with the second input file.
        /// </summary>
        protected StreamReader secondInputReader;

        /// <summary>
        /// A line counter for the first file.
        /// </summary>
        protected ulong firstLineCounter;

        /// <summary>
        /// A line counter for the second file.
        /// </summary>
        protected ulong secondLineCounter;

        /// <summary>
        /// The data extractor that will be applied to each line of the first file.
        /// </summary>
        protected TDataExtractor firstFileExtractor;

        /// <summary>
        /// The data extractor that will be applied to each line of the second file.
        /// </summary>
        protected TDataExtractor secondFileExtractor;

        /// <summary>
        /// The data processor that will be applied to the output of the file extractions.
        /// </summary>
        protected TDataProcessor DataProcessor { get; set; }

        /// <summary>
        /// Indicates the next input file processing action.
        /// </summary>
        protected ProcessingActionType NextAction { get; set; }

        /// <summary>
        /// The next data to process from the first file.
        /// </summary>
        protected TExtractedData nextFirstFileData;

        /// <summary>
        /// The next data to process from the second file.
        /// </summary>
        protected TExtractedData nextSecondFileData;

        /// <summary>
        /// Indicates whether the first file has been processed.
        /// </summary>
        protected bool hasProcessedFirstFile;

        /// <summary>
        /// Indicates whether the second file has been processed.
        /// </summary>
        protected bool hasProcessedSecondFile;

        public DualFileProcessor(
            string firstInputFilePath, TExtractionParameters firstExtractionParameters,
            string secondInputFilePath, TExtractionParameters secondExtractionParameters,
            TProcessingParameters processingParameters)
        {
            FirstInputFilePath = firstInputFilePath;
            SecondInputFilePath = secondInputFilePath;

            firstFileExtractor = new TDataExtractor();
            firstFileExtractor.Initialize(firstExtractionParameters);

            secondFileExtractor = new TDataExtractor();
            secondFileExtractor.Initialize(secondExtractionParameters);

            DataProcessor = new TDataProcessor();
            DataProcessor.Initialize(processingParameters);

            firstInputReader = new StreamReader(FirstInputFilePath);
            secondInputReader = new StreamReader(SecondInputFilePath);

            firstLineCounter = 0;
            secondLineCounter = 0;

            NextAction = ProcessingActionType.AdvanceBoth;
        }

        /// <summary>
        /// Processes the input files.
        /// </summary>
        public void ProcessFiles()
        {
            Timer timer = new Timer($"\n{Constants.Messages.TotalProcessingTime}");

            while (PerformNextAction())
            {
                // Nothing else needs to be done.
            }

            timer.StopAndReport();
        }

        /// <summary>
        /// Advances to next non-emmpty row in a file.
        /// </summary>
        /// <param name="inputReader">A file reader.</param>
        /// <param name="dataExtractor">A data extractor to run on the line.</param>
        /// <param name="nextExtractedData">A reference to the extracted data.</param>
        /// <param name="hasProcessedFile">A flag to indicate if the reader has reached the end of the file.</param>
        /// <param name="lineCounter">A reference to a line counter for the input file.</param>
        /// <param name="otherLineCounter">The line counter for the other file being processed.</param>
        protected void AdvanceInFile(
            ref StreamReader inputReader,
            ref TDataExtractor dataExtractor,
            ref TExtractedData nextExtractedData,
            ref bool hasProcessedFile,
            ref ulong lineCounter,
            ulong otherLineCounter)
        {
            if (hasProcessedFile)
            {
                throw new ProteusException("Internal error: AdvanceInFile() was called on a file that has been processed already!");
            }

            // Keep processing lines until we are able to extract data from one.
            //
            do
            {
                string nextRow = inputReader.ReadLine();

                // Indicate if we've reached the end of the file.
                //
                if (nextRow == null)
                {
                    hasProcessedFile = true;
                    return;
                }

                // Count line and track progress.
                //
                lineCounter++;
                ProgressTracker.Track(lineCounter + otherLineCounter);

                // Pass the line to the data extractor.
                //
                nextExtractedData = dataExtractor.ExtractData(lineCounter, nextRow);
            }
            while (nextExtractedData == null);
        }

        /// <summary>
        /// Performs next action and then determines the next one.
        /// </summary>
        /// <returns>True if processing should continue; false otherwise.</returns>
        protected bool PerformNextAction()
        {
            switch (NextAction)
            {
                case ProcessingActionType.AdvanceFirst:
                    AdvanceInFile(ref firstInputReader, ref firstFileExtractor, ref nextFirstFileData, ref hasProcessedFirstFile, ref firstLineCounter, secondLineCounter);
                    break;

                case ProcessingActionType.AdvanceSecond:
                    AdvanceInFile(ref secondInputReader, ref secondFileExtractor, ref nextSecondFileData, ref hasProcessedSecondFile, ref secondLineCounter, firstLineCounter);
                    break;

                case ProcessingActionType.AdvanceBoth:
                    AdvanceInFile(ref firstInputReader, ref firstFileExtractor, ref nextFirstFileData, ref hasProcessedFirstFile, ref firstLineCounter, secondLineCounter);
                    AdvanceInFile(ref secondInputReader, ref secondFileExtractor, ref nextSecondFileData, ref hasProcessedSecondFile, ref secondLineCounter, firstLineCounter);
                    break;

                case ProcessingActionType.Terminate:
                    return EndProcessing();

                default:
                    throw new ProteusException($"Internal error: Proteus is not handling processing action type '{NextAction}'!");
            }

            // Then perform the processing step.
            //
            NextAction = DataProcessor.Execute(
                hasProcessedFirstFile, firstLineCounter, nextFirstFileData,
                hasProcessedSecondFile, secondLineCounter, nextSecondFileData);

            return true;
        }

        /// <summary>
        /// Finalizes the processing of the files.
        /// </summary>
        /// <returns>Always returns false to indicate that execution should terminate.</returns>
        protected bool EndProcessing()
        {
            DataProcessor.CompleteExecution();

            firstInputReader.Close();
            secondInputReader.Close();

            ILogger logger = LoggingManager.GetLogger();
            logger.LogLine($"\n{firstLineCounter:N0} {Constants.Messages.LinesReadFromFirstFile} '{Path.GetFileName(FirstInputFilePath)}'.");
            logger.LogLine($"\n{secondLineCounter:N0} {Constants.Messages.LinesReadFromSecondFile} '{Path.GetFileName(SecondInputFilePath)}'.");

            return false;
        }
    }
}
