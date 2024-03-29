////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
///
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

namespace LaurentiuCristofor.Proteus.Common.Random
{
    /// <summary>
    /// A sampler of SampleCount distinct values between 1 and TotalCount.
    ///
    /// The algorithm used here is Algorithm S described in Knuth, TAOCP, volume 2, section 3.4.2,
    /// page 122 of the second edition and page 142 of the third edition.
    ///
    /// An interesting side-effect of this algorithm is that the values generated will be produced in increasing order.
    /// </summary>
    public sealed class KnownTotalSampler
    {
        /// <summary>
        /// The largest value that can be sampled.
        /// We will sample values from 1 up to TotalCount.
        /// </summary>
        private ulong TotalCount { get; set; }

        /// <summary>
        /// The number of values to sample.
        /// </summary>
        private ulong SampleCount { get; set; }

        /// <summary>
        /// The number of values evaluated so far for the sampling.
        /// </summary>
        private ulong EvaluatedCount { get; set; }

        /// <summary>
        /// The number of values that were generated (sampled) so far.
        /// </summary>
        private ulong GeneratedCount { get; set; }

        /// <summary>
        /// The uniform random generator source.
        /// </summary>
        private System.Random RandomGenerator { get; set; }

        /// <summary>
        /// Creates a new random sampler that will produce sampleCount distinct values between the values 1 and totalCount,
        /// using a specific instance of System.Random.
        /// </summary>
        /// <param name="totalCount">The largest value to sample; the smallest being 1.</param>
        /// <param name="sampleCount">The size of the sample.</param>
        /// <param name="randomGenerator">The System.Random instance to use, or null to generate a new instance.</param>
        public KnownTotalSampler(ulong totalCount, ulong sampleCount, System.Random randomGenerator = null)
        {
            if (sampleCount < 1 || sampleCount > totalCount)
            {
                throw new ProteusException("Sample count and total count must satisfy the following relationship: 0 < sample count <= total count");
            }

            TotalCount = totalCount;
            SampleCount = sampleCount;
            RandomGenerator = randomGenerator ?? new System.Random();

            // S1. [Initialize]
            //
            EvaluatedCount = 0;
            GeneratedCount = 0;
        }

        /// <summary>
        /// Returns the next value in the sample, between 1 and totalCount.
        /// </summary>
        /// <returns>The next value from the requested sample, or 0 if the method has been called after the sampling was completed.</returns>
        public ulong Next()
        {
            if (GeneratedCount == SampleCount)
            {
                return 0;
            }

            while (true)
            {
                // S2. [Generate U]
                //
                double U = RandomGenerator.NextDouble();

                // S3. [Test]
                //
                if ((TotalCount - EvaluatedCount) * U >= SampleCount - GeneratedCount)
                {
                    // S5. [Skip]
                    //
                    ++EvaluatedCount;
                }
                else
                {
                    // S4. [Select]
                    //
                    ulong generatedValue = EvaluatedCount + 1;

                    ++GeneratedCount;
                    ++EvaluatedCount;

                    return generatedValue;
                }
            }
        }
    }
}
