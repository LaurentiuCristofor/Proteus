////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// 
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System.IO;

using LaurentiuCristofor.Proteus.Common;
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
            this.OutputFilePath = outputFilePath;

            this.TrackProgress = trackProgress;

            this.OutputWriter = new StreamWriter(this.OutputFilePath);

            this.CountLinesWritten = 0;
        }

        public void WriteLine(string line)
        {
            // If we need to track progress, reset the progress tracker output before our first write.
            //
            if (this.TrackProgress && this.CountLinesWritten == 0)
            {
                ProgressTracker.Reset();
            }

            this.OutputWriter.WriteLine(line);

            this.CountLinesWritten++;

            if (this.TrackProgress)
            {
                ProgressTracker.Track(this.CountLinesWritten);
            }
        }

        public void CloseAndReport()
        {
            this.OutputWriter.Close();

            OutputInterface.LogLine($"\n{this.CountLinesWritten:N0} lines were written to file '{Path.GetFileName(this.OutputFilePath)}'.");
        }
    }
}
