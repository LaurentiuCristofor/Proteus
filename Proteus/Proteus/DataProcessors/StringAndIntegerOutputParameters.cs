////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using LaurentiuCristofor.Proteus.Common;

namespace LaurentiuCristofor.Proteus.DataProcessors
{
    /// <summary>
    /// Includes a string and ulong pair of parameters.
    /// </summary>
    public class StringAndIntegerOutputParameters : StringOutputParameters
    {
        /// <summary>
        /// The integer value.
        /// </summary>
        public int IntegerValue { get; protected set; }

        public StringAndIntegerOutputParameters(
            string outputFilePath,
            string stringValue,
            int integerValue)
            : base(outputFilePath, stringValue)
        {
            this.IntegerValue = integerValue;
        }
    }
}
