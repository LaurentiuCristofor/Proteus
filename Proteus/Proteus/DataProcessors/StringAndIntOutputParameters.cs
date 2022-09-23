////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using LaurentiuCristofor.Proteus.Common;

namespace LaurentiuCristofor.Proteus.DataProcessors
{
    /// <summary>
    /// Includes an int parameter along a string and output parameters.
    /// </summary>
    public class StringAndIntOutputParameters : StringOutputParameters
    {
        /// <summary>
        /// The int value.
        /// </summary>
        public int IntValue { get; protected set; }

        public StringAndIntOutputParameters(
            string outputFilePath,
            string stringValue,
            int intValue)
            : base(outputFilePath, stringValue)
        {
            this.IntValue = intValue;
        }
    }
}
