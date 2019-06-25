////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using LaurentiuCristofor.Proteus.Common;

namespace LaurentiuCristofor.Proteus.DataProcessors
{
    /// <summary>
    /// A passthrough processor that doesn't change data and doesn't output data.
    /// </summary>
    public class PassthroughProcessor : IDataProcessor<UnusedType, string>
    {
        public void Initialize(UnusedType unusedProcessingParameters)
        {
        }

        public bool Execute(ulong lineNumber, string inputLine)
        {
            return true;
        }

        public void CompleteExecution()
        {
        }
    }
}
