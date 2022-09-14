////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;
using LaurentiuCristofor.Proteus.Common;
using LaurentiuCristofor.Proteus.FileOperations;

namespace LaurentiuCristofor.Proteus.DataProcessors
{
    /// <summary>
    /// A data processor that extracts ranges of lines.
    /// </summary>
    public class LineRangeExtractProcessor : BaseOutputProcessor, IDataProcessor<UnsignedIntegerAndStringParameters, string>
    {
        /// <summary>
        /// Parameters of this operation.
        /// </summary>
        protected UnsignedIntegerAndStringParameters Parameters { get; set; }

        public void Initialize(UnsignedIntegerAndStringParameters processingParameters)
        {
            this.Parameters = processingParameters;

            ArgumentChecker.CheckNotZero(this.Parameters.UnsignedIntegerValue);
            ArgumentChecker.CheckNotNullAndNotEmpty(this.Parameters.StringValue);
        }

        public bool Execute(ulong lineNumber, string line)
        {
            DataProcessorValidation.ValidateLine(line);

            // Open a new file, if we're at the beginning of a new range.
            //
            if (this.OutputWriter == null)
            {
                this.OutputWriter = new TextFileWriter(this.Parameters.OutputFilePath + $".{lineNumber}{this.Parameters.StringValue}");
            }

            this.OutputWriter.WriteLine(line);

            // If we just finished outputting a range, close current file.
            //
            if (lineNumber % this.Parameters.UnsignedIntegerValue == 0)
            {
                this.OutputWriter.CloseAndReport();
                this.OutputWriter = null;
            }

            return true;
        }
    }
}
