////////////////////////////////////////////////////////////////////////////////////////////////////
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
        protected BaseOutputParameters Parameters { get; set; }

        public void Initialize(BaseOutputParameters processingParameters)
        {
            this.Parameters = processingParameters;

            this.OutputWriter = new FileWriter(this.Parameters.OutputFilePath);
        }

        public bool Execute(ulong lineNumber, string line)
        {
            this.OutputWriter.WriteLine(line);

            return true;
        }
    }
}
