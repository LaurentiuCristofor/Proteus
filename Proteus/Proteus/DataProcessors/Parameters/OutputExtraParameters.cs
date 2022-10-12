////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
///
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

namespace LaurentiuCristofor.Proteus.DataProcessors.Parameters
{
    /// <summary>
    /// Includes extra parameters for performing an output operation.
    /// </summary>
    public class OutputExtraParameters : BaseOutputParameters
    {
        /// <summary>
        /// String-type parameters.
        /// </summary>
        public string[] StringParameters { get; set; }

        /// <summary>
        /// Int-type parameters.
        /// </summary>
        public int[] IntParameters { get; set; }

        /// <summary>
        /// Ulong-type parameters.
        /// </summary>
        public ulong[] UlongParameters { get; set; }

        public OutputExtraParameters(
            string outputFilePath,
            string[] stringParameters = null,
            int[] intParameters = null,
            ulong[] ulongParameters = null)
            : base(outputFilePath)
        {
            this.StringParameters = stringParameters;
            this.IntParameters = intParameters;
            this.UlongParameters = ulongParameters;
        }
    }
}
