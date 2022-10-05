////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
///
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

namespace LaurentiuCristofor.Proteus.DataGenerators
{
    /// <summary>
    /// Includes parameters common to all data generation operations.
    /// </summary>
    public class BaseGenerationParameters
    {
        /// <summary>
        /// A seed value to use when initializing the random number generator.
        /// 
        /// If set to a negative value, no seed will be used.
        /// </summary>
        public int Seed { get; set; }

        /// <summary>
        /// The count of data that should be generated.
        /// </summary>
        public ulong GenerationCount { get; set; }

        public BaseGenerationParameters(
            int seed,
            ulong generationCount)
        {
            this.Seed = seed;
            this.GenerationCount = generationCount;
        }
    }
}
