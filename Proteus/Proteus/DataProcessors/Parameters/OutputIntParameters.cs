////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
///
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

namespace LaurentiuCristofor.Proteus.DataProcessors.Parameters
{
    /// <summary>
    /// Includes an int parameter along output parameters.
    /// </summary>
    public class OutputIntParameters : BaseOutputParameters
    {
        /// <summary>
        /// The int value.
        /// </summary>
        public int IntValue { get; protected set; }

        public OutputIntParameters(
            string outputFilePath,
            int intValue)
            : base(outputFilePath)
        {
            this.IntValue = intValue;
        }
    }
}
