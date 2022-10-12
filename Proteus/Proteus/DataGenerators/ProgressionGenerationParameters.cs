////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
///
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using LaurentiuCristofor.Proteus.Common.Types;

namespace LaurentiuCristofor.Proteus.DataGenerators
{
    /// <summary>
    /// Includes parameters for generating data according to a specific progression.
    /// </summary>
    public class ProgressionGenerationParameters : BaseGenerationParameters
    {
        /// <summary>
        /// The type of progression that should be generated.
        /// </summary>
        public ProgressionType ProgressionType { get; set; }

        /// <summary>
        /// Progression parameters.
        /// </summary>
        public double[] ProgressionParameters { get; set; }

        public ProgressionGenerationParameters(
            ulong generationCount,
            ProgressionType progressionType,
            double[] progressionParameters = null)
            : base(generationCount)
        {
            this.ProgressionType = progressionType;
            this.ProgressionParameters = progressionParameters;
        }
    }
}
