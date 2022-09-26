////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
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
    /// </summary>
    public class SplitLineRangesProcessor : BaseOutputProcessor, IDataProcessor<StringAndULongOutputParameters, string>
    {
        protected StringAndULongOutputParameters Parameters { get; set; }

        public void Initialize(StringAndULongOutputParameters processingParameters)
        {
            this.Parameters = processingParameters;

            ArgumentChecker.CheckNotNullAndNotEmpty(this.Parameters.StringValue);
            ArgumentChecker.CheckNotZero(this.Parameters.ULongValue);
        }

        public bool Execute(ulong lineNumber, string line)
        {
            // Open a new file, if we're at the beginning of a new range.
            //
            if (this.OutputWriter == null)
            {
                this.OutputWriter = new FileWriter(this.Parameters.OutputFilePath + $".{lineNumber}{this.Parameters.StringValue}");
            }

            this.OutputWriter.WriteLine(line);

            // If we just finished outputting a range, close current file.
            //
            if (lineNumber % this.Parameters.ULongValue == 0)
            {
                this.OutputWriter.CloseAndReport();
                this.OutputWriter = null;
            }

            return true;
        }
    }
}
