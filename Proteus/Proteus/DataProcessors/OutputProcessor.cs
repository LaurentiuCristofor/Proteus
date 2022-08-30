﻿////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using LaurentiuCristofor.Proteus.FileOperations;

namespace LaurentiuCristofor.Proteus.DataProcessors
{
    /// <summary>
    /// A plain text writer.
    /// </summary>
    public class OutputProcessor : BaseOutputProcessor, IDataProcessor<BaseOutputParameters, string>
    {
        /// <summary>
        /// Parameters of this processor.
        /// </summary>
        protected BaseOutputParameters Parameters { get; set; }

        public void Initialize(BaseOutputParameters processingParameters)
        {
            this.Parameters = processingParameters;

            this.OutputWriter = new TextFileWriter(this.Parameters.OutputFilePath);
        }

        public bool Execute(ulong lineNumber, string line)
        {
            DataProcessorValidation.ValidateLine(line);

            this.OutputWriter.WriteLine(line);

            return true;
        }
    }
}