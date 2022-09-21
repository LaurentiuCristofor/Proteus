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
    public class StringAndUnsignedIntegerOutputParameters : StringOutputParameters
    {
        /// <summary>
        /// The unsigned integer value.
        /// </summary>
        public ulong UnsignedIntegerValue { get; protected set; }

        public StringAndUnsignedIntegerOutputParameters(
            string outputFilePath,
            string stringValue,
            ulong unsignedIntegerValue)
            : base(outputFilePath, stringValue)
        {
            this.UnsignedIntegerValue = unsignedIntegerValue;
        }
    }
}
