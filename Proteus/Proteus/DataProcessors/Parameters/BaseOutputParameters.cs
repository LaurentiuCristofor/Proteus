////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
///
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

namespace LaurentiuCristofor.Proteus.DataProcessors.Parameters
{
    /// <summary>
    /// Includes common parameters for performing any operation that outputs results to a file.
    /// </summary>
    public class BaseOutputParameters
    {
        /// <summary>
        /// The file path for storing the output of the operation.
        /// </summary>
        public string OutputFilePath { get; protected set; }

        public BaseOutputParameters(
            string outputFilePath)
        {
            this.OutputFilePath = outputFilePath;
        }
    }
}
