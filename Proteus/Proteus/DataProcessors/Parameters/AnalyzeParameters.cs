////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
///
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using LaurentiuCristofor.Proteus.Common.Types;

namespace LaurentiuCristofor.Proteus.DataProcessors.Parameters
{
    /// <summary>
    /// Includes parameters for performing an 'analyze' operation.
    /// </summary>
    public class AnalyzeParameters
    {
        /// <summary>
        /// The type of data that we analyze.
        /// </summary>
        public DataType DataType { get; protected set; }

        /// <summary>
        /// The number of top/bottom values that the analyze report should output.
        /// </summary>
        public int OutputLimit { get; protected set; }

        public AnalyzeParameters(
            DataType dataType,
            int outputLimit)
        {
            DataType = dataType;
            OutputLimit = outputLimit;
        }
    }
}
