////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using LaurentiuCristofor.Proteus.Common;

namespace LaurentiuCristofor.Proteus.DataProcessors
{
    /// <summary>
    /// Includes a string parameter.
    /// </summary>
    public class StringParameters : BaseOutputParameters
    {
        /// <summary>
        /// The string value.
        /// </summary>
        public string StringValue { get; protected set; }

        public StringParameters(
            string outputFilePath,
            string stringValue)
            : base(outputFilePath)
        {
            
            this.StringValue = stringValue;
        }
    }
}
