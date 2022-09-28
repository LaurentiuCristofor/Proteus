﻿////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using LaurentiuCristofor.Proteus.DataProcessors.Parameters;
using LaurentiuCristofor.Proteus.FileOperations;

namespace LaurentiuCristofor.Proteus.DataProcessors.Dual
{
    /// <summary>
    /// A data processor that concatenates lines from two files.
    /// </summary>
    public class ConcatenateProcessor: BaseOutputProcessor, IDualDataProcessor<OutputStringParameters, string>
    {
        protected OutputStringParameters Parameters { get; set; }

        public void Initialize(OutputStringParameters processingParameters)
        {
            this.Parameters = processingParameters;

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

            string concatenatedLines = firstLine + this.Parameters.StringValue + secondLine;

            this.OutputWriter.WriteLine(concatenatedLines);

            return ProcessingActionType.AdvanceBoth;
        }
    }
}
