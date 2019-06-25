////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

namespace LaurentiuCristofor.Proteus.DataProcessors
{
    /// <summary>
    /// Includes parameters for performing an analyze operation.
    /// </summary>
    public class AnalyzeParameters
    {
        /// <summary>
        /// The number of top/bottom values that the analyze report should output.
        /// </summary>
        public int ValuesLimit { get; protected set; }

        public AnalyzeParameters(
            int valuesLimit)
        {
            this.ValuesLimit = valuesLimit;
        }
    }
}
