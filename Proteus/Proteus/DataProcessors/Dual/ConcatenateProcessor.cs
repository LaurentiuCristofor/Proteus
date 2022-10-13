////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
///
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using LaurentiuCristofor.Proteus.Common;
using LaurentiuCristofor.Proteus.DataProcessors.Parameters;
using LaurentiuCristofor.Proteus.FileOperations;

namespace LaurentiuCristofor.Proteus.DataProcessors.Dual
{
    /// <summary>
    /// A data processor that concatenates lines from two files.
    /// 
    /// OutputExtraParameters is expected to contain:
    /// StringParameters[0] - line separator
    /// </summary>
    public class ConcatenateProcessor: BaseOutputProcessor, IDualDataProcessor<OutputExtraParameters, string>
    {
        protected const int LineSeparatorIndex = 0;

        /// <summary>
        /// The line separator parameter.
        /// </summary>
        protected string LineSeparator { get; set; }

        public void Initialize(OutputExtraParameters processingParameters)
        {
            ArgumentChecker.CheckPresence(processingParameters.StringParameters, LineSeparatorIndex);
            this.LineSeparator = processingParameters.StringParameters[LineSeparatorIndex];
            ArgumentChecker.CheckNotNull(this.LineSeparator);

            this.OutputWriter = new FileWriter(processingParameters.OutputFilePath);
        }

        public ProcessingActionType Execute(
            bool hasProcessedFirstFile, ulong firstLineNumber, string firstLine,
            bool hasProcessedSecondFile, ulong secondLineNumber, string secondLine)
        {
            if (hasProcessedFirstFile || hasProcessedSecondFile)
            {
                return ProcessingActionType.Terminate;
            }

            string concatenatedLines = firstLine + this.LineSeparator + secondLine;

            this.OutputWriter.WriteLine(concatenatedLines);

            return ProcessingActionType.AdvanceBoth;
        }
    }
}
