////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
///
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using LaurentiuCristofor.Proteus.FileOperations;
using LaurentiuCristofor.Proteus.DataProcessors.Parameters;

namespace LaurentiuCristofor.Proteus.DataProcessors
{
    /// <summary>
    /// A plain processor that just outputs line data without any changes.
    /// </summary>
    public class OutputProcessor : BaseOutputProcessor, IDataProcessor<BaseOutputParameters, string>
    {
        public void Initialize(BaseOutputParameters processingParameters)
        {
            OutputWriter = new FileWriter(processingParameters.OutputFilePath);
        }

        public bool Execute(ulong lineNumber, string line)
        {
            OutputWriter.WriteLine(line);

            return true;
        }
    }
}
