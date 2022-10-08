////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
///
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using LaurentiuCristofor.Proteus.Common;
using LaurentiuCristofor.Proteus.DataProcessors.Parameters;
using LaurentiuCristofor.Proteus.FileOperations;

namespace LaurentiuCristofor.Proteus.DataProcessors
{
    /// <summary>
    /// A data processor that splits ranges of lines into their own files.
    /// 
    /// OutputParameters is expected to contain:
    /// StringParameters[0] - output file extension
    /// UlongParameters[0] - range size
    /// </summary>
    public class SplitLineRangesProcessor : BaseOutputProcessor, IDataProcessor<OutputParameters, string>
    {
        protected OutputParameters Parameters { get; set; }

        /// <summary>
        /// The file extension that should be used for the output files.
        /// </summary>
        protected string OutputFileExtension { get; set; }

        /// <summary>
        /// The size of the ranges in which we should split the file.
        /// </summary>
        protected ulong RangeSize { get; set; }

        public void Initialize(OutputParameters processingParameters)
        {
            this.Parameters = processingParameters;

            ArgumentChecker.CheckPresence<string>(this.Parameters.StringParameters, 0);
            ArgumentChecker.CheckNotNullAndNotEmpty(this.Parameters.StringParameters[0]);
            this.OutputFileExtension = this.Parameters.StringParameters[0];

            ArgumentChecker.CheckPresence<ulong>(this.Parameters.UlongParameters, 0);
            ArgumentChecker.CheckNotZero(this.Parameters.UlongParameters[0]);
            this.RangeSize = this.Parameters.UlongParameters[0];
        }

        public bool Execute(ulong lineNumber, string line)
        {
            // Open a new file, if we're at the beginning of a new range.
            //
            if (this.OutputWriter == null)
            {
                this.OutputWriter = new FileWriter(this.Parameters.OutputFilePath + $".{lineNumber}{this.OutputFileExtension}");
            }

            this.OutputWriter.WriteLine(line);

            // If we just finished outputting a range, close current file.
            //
            if (lineNumber % this.RangeSize == 0)
            {
                this.OutputWriter.CloseAndReport();
                this.OutputWriter = null;
            }

            return true;
        }
    }
}
