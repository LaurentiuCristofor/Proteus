////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using LaurentiuCristofor.Proteus.FileOperations;

namespace LaurentiuCristofor.Proteus.DataProcessors
{
    /// <summary>
    /// A passthrough plain text writer.
    /// </summary>
    public class PassthroughOutputProcessor : BaseOutputProcessor, IDataProcessor<BaseOutputParameters, string>
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

        public bool Execute(ulong lineNumber, string inputData)
        {
            // We may not always be able to extract data.
            // Ignore these cases; the extractor will already have printed a warning message.
            //
            if (inputData == null)
            {
                return true;
            }

            this.OutputWriter.WriteLine(inputData);

            return true;
        }
    }
}
