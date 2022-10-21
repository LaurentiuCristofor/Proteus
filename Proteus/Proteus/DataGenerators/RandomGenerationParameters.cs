////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
///
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

namespace LaurentiuCristofor.Proteus.DataGenerators
{
    /// <summary>
    /// Includes parameters common to all random data generation operations.
    /// </summary>
    public class RandomGenerationParameters : BaseGenerationParameters
    {
        /// <summary>
        /// A seed value to use when initializing the random number generator.
        ///
        /// If set to a negative value, no seed will be used.
        /// </summary>
        public int Seed { get; set; }

        public RandomGenerationParameters(
            int seed,
            ulong generationCount)
            : base(generationCount)
        {
            Seed = seed;
        }
    }
}
