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
    /// OutputExtraParameters is expected to contain:
    /// StringParameters[0] - output file extension
    /// UlongParameters[0] - range size
    /// </summary>
    public class SplitLineRangesProcessor : BaseOutputProcessor, IDataProcessor<OutputExtraParameters, string>
    {
        protected const int OutputFileExtensionIndex = 0;
        protected const int RangeSizeIndex = 0;

        protected string OutputFilePath { get; set; }

        /// <summary>
        /// The file extension that should be used for the output files.
        /// </summary>
        protected string OutputFileExtension { get; set; }

        /// <summary>
        /// The size of the ranges in which we should split the file.
        /// </summary>
        protected ulong RangeSize { get; set; }

        public void Initialize(OutputExtraParameters processingParameters)
        {
            OutputFilePath = processingParameters.OutputFilePath;

            ArgumentChecker.CheckPresence(processingParameters.StringParameters, OutputFileExtensionIndex);
            ArgumentChecker.CheckPresence(processingParameters.UlongParameters, RangeSizeIndex);

            OutputFileExtension = processingParameters.StringParameters[OutputFileExtensionIndex];
            RangeSize = processingParameters.UlongParameters[RangeSizeIndex];

            ArgumentChecker.CheckNotNullAndNotEmpty(OutputFileExtension);
            ArgumentChecker.CheckGreaterThanOrEqualTo(RangeSize, 1UL);
        }

        public bool Execute(ulong lineNumber, string line)
        {
            // Open a new file, if we're at the beginning of a new range.
            //
            if (OutputWriter == null)
            {
                OutputWriter = new FileWriter(OutputFilePath + $".{lineNumber}{OutputFileExtension}");
            }

            OutputWriter.WriteLine(line);

            // If we just finished outputting a range, close current file.
            //
            if (lineNumber % RangeSize == 0)
            {
                OutputWriter.CloseAndReport();
                OutputWriter = null;
            }

            return true;
        }
    }
}
