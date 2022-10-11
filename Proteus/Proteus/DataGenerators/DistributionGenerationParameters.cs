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
    /// Includes parameters for generating data according to a specific distribution.
    /// </summary>
    public class DistributionGenerationParameters : RandomGenerationParameters
    {
        /// <summary>
        /// The type of data distribution that should be used for the data generation.
        /// </summary>
        public DistributionType DistributionType { get; set; }

        /// <summary>
        /// The mean of the distribution, if applicable.
        /// </summary>
        public string DistributionMean { get; set; }

        public DistributionGenerationParameters(
            int seed,
            ulong generationCount,
            DistributionType distributionType,
            string distributionMean)
            : base(seed, generationCount)
        {
            this.DistributionType = distributionType;
            this.DistributionMean = distributionMean;
        }
    }
}
