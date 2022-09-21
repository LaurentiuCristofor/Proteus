////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.IO;

using LaurentiuCristofor.Proteus.Common;
using LaurentiuCristofor.Proteus.DataExtractors;
using LaurentiuCristofor.Proteus.DataProcessors.SideBySide;

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
    public class SideBySideFileProcessor<
        TDataExtractor, TExtractionParameters, TExtractedData,
        TDataProcessor, TProcessingParameters>
        where TDataExtractor : IDataExtractor<TExtractionParameters, TExtractedData>, new()
        where TDataProcessor : ISideBySideDataProcessor<TProcessingParameters, TExtractedData>, new()
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
        /// The object that we'll use to read the first input file.
        /// </summary>
        protected StreamReader firstInputReader;

        /// <summary>
        /// The object that we'll use to read the second input file.
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

        public SideBySideFileProcessor(
            string firstInputFilePath, TExtractionParameters firstExtractionParameters,
            string secondInputFilePath, TExtractionParameters secondExtractionParameters,
            TProcessingParameters processingParameters)
        {
            this.FirstInputFilePath = firstInputFilePath;
            this.SecondInputFilePath = secondInputFilePath;

            this.firstFileExtractor = new TDataExtractor();
            this.firstFileExtractor.Initialize(firstExtractionParameters);

            this.secondFileExtractor = new TDataExtractor();
            this.secondFileExtractor.Initialize(secondExtractionParameters);

            this.DataProcessor = new TDataProcessor();
            this.DataProcessor.Initialize(processingParameters);

            this.firstInputReader = new StreamReader(this.FirstInputFilePath);
            this.secondInputReader = new StreamReader(this.SecondInputFilePath);

            this.firstLineCounter = 0;
            this.secondLineCounter = 0;

            this.NextAction = ProcessingActionType.AdvanceBoth;
        }

        /// <summary>
        /// Processes the input files.
        /// </summary>
        public void ProcessFiles()
        {
            while (ProcessCurrentRows())
            {
                // Nothing else needs to be done, but to process each row.
            }
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
        void AdvanceInFile(
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

            // Loop over empty lines.
            //
            while (true)
            {
                string nextRow = inputReader.ReadLine();

                // Indicate if we've reached the end of the file.
                //
                if (nextRow == null)
                {
                    hasProcessedFile = true;
                    break;
                }

                // Count line and track progress.
                //
                lineCounter++;
                ProgressTracker.Track(lineCounter + otherLineCounter);

                // Empty lines will get skipped; for all others, we pass them to the data extractor.
                //
                if (!String.IsNullOrEmpty(nextRow))
                {
                    nextExtractedData = dataExtractor.ExtractData(lineCounter, nextRow);
                    break;
                }
            }
        }

        /// <summary>
        /// Processes the current rows from the input files.
        /// </summary>
        /// <returns>True if processing should continue; false otherwise.</returns>
        private bool ProcessCurrentRows()
        {
            switch (this.NextAction)
            {
                case ProcessingActionType.AdvanceFirst:
                    AdvanceInFile(ref this.firstInputReader, ref this.firstFileExtractor, ref this.nextFirstFileData, ref this.hasProcessedFirstFile, ref this.firstLineCounter, this.secondLineCounter);
                    break;

                case ProcessingActionType.AdvanceSecond:
                    AdvanceInFile(ref this.secondInputReader, ref this.secondFileExtractor, ref this.nextSecondFileData, ref this.hasProcessedSecondFile, ref this.secondLineCounter, this.firstLineCounter);
                    break;

                case ProcessingActionType.AdvanceBoth:
                    AdvanceInFile(ref this.firstInputReader, ref this.firstFileExtractor, ref this.nextFirstFileData, ref this.hasProcessedFirstFile, ref this.firstLineCounter, this.secondLineCounter);
                    AdvanceInFile(ref this.secondInputReader, ref this.secondFileExtractor, ref this.nextSecondFileData, ref this.hasProcessedSecondFile, ref this.secondLineCounter, this.firstLineCounter);
                    break;

                case ProcessingActionType.Terminate:
                    return EndProcessing();

                default:
                    throw new ProteusException($"Internal error: Proteus is not handling processing action type '{this.NextAction}'!");
            }

            // Then perform the processing step.
            //
            this.NextAction = this.DataProcessor.Execute(
                this.hasProcessedFirstFile, this.firstLineCounter, this.nextFirstFileData,
                this.hasProcessedSecondFile, this.secondLineCounter, this.nextSecondFileData);

            return true;
        }

        /// <summary>
        /// Finalizes the processing of the files.
        /// </summary>
        /// <returns>Always returns false to indicate that execution should terminate.</returns>
        private bool EndProcessing()
        {
            this.DataProcessor.CompleteExecution();

            this.firstInputReader.Close();
            this.secondInputReader.Close();

            OutputInterface.LogLine($"\n{this.firstLineCounter} lines were read from file { Path.GetFileName(this.FirstInputFilePath)}.");
            OutputInterface.LogLine($"\n{this.secondLineCounter} lines were read from file { Path.GetFileName(this.SecondInputFilePath)}.");

            return false;
        }
    }
}
