////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

namespace LaurentiuCristofor.Proteus.DataProcessors.Parameters
{
    /// <summary>
    /// Includes a ulong parameter along a string and output parameters.
    /// </summary>
    public class OutputStringAndULongParameters : OutputStringParameters
    {
        /// <summary>
        /// The ulong value.
        /// </summary>
        public ulong ULongValue { get; protected set; }

        public OutputStringAndULongParameters(
            string outputFilePath,
            string stringValue,
            ulong uLongValue)
            : base(outputFilePath, stringValue)
        {
            this.ULongValue = uLongValue;
        }
    }
}
