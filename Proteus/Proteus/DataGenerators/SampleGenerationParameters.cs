////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
///
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

namespace LaurentiuCristofor.Proteus.DataGenerators
{
    /// <summary>
    /// Includes parameters for performing a sample data generation.
    /// </summary>
    public class SampleGenerationParameters : BaseGenerationParameters
    {
        /// <summary>
        /// The total count out of which the sample data should be produced.
        /// 
        /// The sample data will contain GeneratedCount distinct values from the interval [1, TotalCount].
        /// </summary>
        public ulong TotalCount { get; set; }

        public SampleGenerationParameters(
            int seed,
            ulong generationCount,
            ulong totalCount)
            : base(seed, generationCount)
        {
            this.TotalCount = totalCount;
        }
    }
}
