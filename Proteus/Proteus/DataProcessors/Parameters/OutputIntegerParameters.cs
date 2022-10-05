////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
///
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

namespace LaurentiuCristofor.Proteus.DataProcessors.Parameters
{
    /// <summary>
    /// Includes an integer parameter along output parameters.
    /// </summary>
    public class OutputIntegerParameters : BaseOutputParameters
    {
        /// <summary>
        /// The integer value.
        /// </summary>
        public int IntegerValue { get; protected set; }

        public OutputIntegerParameters(
            string outputFilePath,
            int integerValue)
            : base(outputFilePath)
        {
            this.IntegerValue = integerValue;
        }
    }
}
