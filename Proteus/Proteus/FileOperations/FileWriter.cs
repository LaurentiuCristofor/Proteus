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

namespace LaurentiuCristofor.Proteus.FileOperations
{
    /// <summary>
    /// A TextWriter that keeps a count of the lines written.
    /// </summary>
    public class FileWriter
    {
        /// <summary>
        /// The path of the output file.
        /// </summary>
        protected string OutputFilePath { get; set; }

        /// <summary>
        /// Indicates whether writing should track progress.
        ///
        /// This is used when writing is performed separately from reading.
        /// </summary>
        protected bool TrackProgress { get; set; }

        /// <summary>
        /// The writer for producing the output file.
        /// </summary>
        protected TextWriter OutputWriter { get; set; }

        /// <summary>
        /// A line counter for the output file.
        /// </summary>
        protected ulong CountLinesWritten { get; set; }

        public FileWriter(string outputFilePath, bool trackProgress = false)
        {
            OutputFilePath = outputFilePath;

            TrackProgress = trackProgress;

            OutputWriter = new StreamWriter(OutputFilePath);

            CountLinesWritten = 0;
        }

        /// <summary>
        /// Write a line and track progress.
        /// </summary>
        /// <param name="line">The line to write.</param>
        public void WriteLine(string line)
        {
            // If we need to track progress, reset the progress tracker output before our first write.
            //
            if (TrackProgress && CountLinesWritten == 0)
            {
                ProgressTracker.Reset();
            }

            OutputWriter.WriteLine(line);

            CountLinesWritten++;

            if (TrackProgress)
            {
                ProgressTracker.Track(CountLinesWritten);
            }
        }

        /// <summary>
        /// Closes the writer stream and reports how many lines were written.
        /// </summary>
        public void CloseAndReport()
        {
            OutputWriter.Close();

            LoggingManager.GetLogger().LogLine($"\n{CountLinesWritten:N0} {Constants.Messages.LinesWrittenToFile} '{Path.GetFileName(OutputFilePath)}'.");
        }
    }
}
