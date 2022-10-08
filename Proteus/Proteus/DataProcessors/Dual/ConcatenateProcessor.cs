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
    /// OutputParameters is expected to contain:
    /// StringParameters[0] - line separator
    /// </summary>
    public class ConcatenateProcessor: BaseOutputProcessor, IDualDataProcessor<OutputParameters, string>
    {
        protected OutputParameters Parameters { get; set; }

        /// <summary>
        /// The line separator parameter.
        /// </summary>
        protected string LineSeparator { get; set; }

        public void Initialize(OutputParameters processingParameters)
        {
            this.Parameters = processingParameters;

            ArgumentChecker.CheckPresence<string>(this.Parameters.StringParameters, 0);
            this.LineSeparator = this.Parameters.StringParameters[0];

            this.OutputWriter = new FileWriter(this.Parameters.OutputFilePath);
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
