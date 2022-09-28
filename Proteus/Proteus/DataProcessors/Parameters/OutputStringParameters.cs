////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

namespace LaurentiuCristofor.Proteus.DataProcessors.Parameters
{
    /// <summary>
    /// Includes a string parameter along output parameters.
    /// </summary>
    public class OutputStringParameters : BaseOutputParameters
    {
        /// <summary>
        /// The string value.
        /// </summary>
        public string StringValue { get; protected set; }

        public OutputStringParameters(
            string outputFilePath,
            string stringValue)
            : base(outputFilePath)
        {
            this.StringValue = stringValue;
        }
    }
}
