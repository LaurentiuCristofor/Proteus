////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using LaurentiuCristofor.Proteus.Common;

namespace LaurentiuCristofor.Proteus.DataProcessors
{
    /// <summary>
    /// Includes a ulong and string pair of parameters.
    /// </summary>
    public class UnsignedIntegerAndStringParameters : BaseOutputParameters
    {
        /// <summary>
        /// The unsigned integer value.
        /// </summary>
        public ulong UnsignedIntegerValue { get; protected set; }

        /// <summary>
        /// The string value.
        /// </summary>
        public string StringValue { get; protected set; }

        public UnsignedIntegerAndStringParameters(
            string outputFilePath,
            ulong unsignedIntegerValue,
            string stringValue)
            : base(outputFilePath)
        {
            
            this.UnsignedIntegerValue = unsignedIntegerValue;
            this.StringValue = stringValue;
        }
    }
}
