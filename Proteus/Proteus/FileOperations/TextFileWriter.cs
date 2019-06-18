////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System.IO;

using LaurentiuCristofor.Proteus.Common;

namespace LaurentiuCristofor.Proteus.FileOperations
{
    /// <summary>
    /// A TextWriter that keeps a count of the lines written.
    /// </summary>
    public class TextFileWriter
    {
        /// <summary>
        /// The path of the output file.
        /// </summary>
        private string OutputFilePath { get; set; }

        /// <summary>
        /// The writer for producing the output file.
        /// </summary>
        private TextWriter OutputWriter { get; set; }

        /// <summary>
        /// A line counter for the output file.
        /// </summary>
        private ulong CountLinesWritten { get; set; }

        public TextFileWriter(string outputFilePath)
        {
            this.OutputFilePath = outputFilePath;

            this.OutputWriter = new StreamWriter(this.OutputFilePath);

            this.CountLinesWritten = 0;
        }

        public void WriteLine(string line)
        {
            this.OutputWriter.WriteLine(line);

            this.CountLinesWritten++;
        }

        public void CloseAndReport()
        {
            this.OutputWriter.Close();

            IOStream.LogLine($"\n{this.CountLinesWritten} lines were written to file { Path.GetFileName(this.OutputFilePath)}.");
        }
    }
}
