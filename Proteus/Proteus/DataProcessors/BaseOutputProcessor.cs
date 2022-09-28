////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// 
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using LaurentiuCristofor.Proteus.FileOperations;

namespace LaurentiuCristofor.Proteus.DataProcessors
{
    /// <summary>
    /// Base class for processors that produce an output file.
    /// </summary>
    public abstract class BaseOutputProcessor
    {
        /// <summary>
        /// A writer for producing the output file.
        /// </summary>
        protected FileWriter OutputWriter { get; set; }

        public virtual void CompleteExecution()
        {
            if (this.OutputWriter != null)
            {
                this.OutputWriter.CloseAndReport();
            }
        }
    }
}
