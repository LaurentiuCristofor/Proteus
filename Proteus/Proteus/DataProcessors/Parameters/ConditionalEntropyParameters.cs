////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
///
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using LaurentiuCristofor.Proteus.Common.Types;

namespace LaurentiuCristofor.Proteus.DataProcessors.Parameters
{
    /// <summary>
    /// Includes parameters for calculating conditional entropy.
    /// </summary>
    public class ConditionalEntropyParameters
    {
        /// <summary>
        /// The first type of data that we process.
        /// </summary>
        public DataType FirstDataType { get; protected set; }

        /// <summary>
        /// The second type of data that we process.
        /// </summary>
        public DataType SecondDataType { get; protected set; }

        public ConditionalEntropyParameters(
            DataType firstDataType,
            DataType secondDataType)
        {
            FirstDataType = firstDataType;
            SecondDataType = secondDataType;
        }
    }
}
