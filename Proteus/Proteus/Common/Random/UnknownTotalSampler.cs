////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
///
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;

namespace LaurentiuCristofor.Proteus.Common.Random
{
    /// <summary>
    /// A sampler of SampleCount distinct values when the total count is not known.
    ///
    /// The algorithm used here is based on Algorithm R described in Knuth, TAOCP, volume 2, section 3.4.2,
    /// page 144 of the third edition.
    ///
    /// The caller is expected to maintain an array of SampleCount values, initialized with the numbers of the first SampleCount elements.
    /// The caller can then call EvaluateAnotherElement for each other element to determine which sample element should be replaced
    /// by the currently evaluated element.
    /// At the end of the processing, the caller will have the sample element numbers in its array.
    ///
    /// IMPORTANT: The sampler operates in terms of numbers, not index values;
    /// the caller needs to adjust them appropriately, before operating on its sample array.
    /// </summary>
    public sealed class UnknownTotalSampler
    {
        /// <summary>
        /// The number of values to sample.
        /// </summary>
        private int SampleCount { get; set; }

        /// <summary>
        /// The count of elements considered so far.
        /// </summary>
        private int ElementCount { get; set; }

        /// <summary>
        /// The uniform random generator source.
        /// </summary>
        private System.Random RandomGenerator { get; set; }

        /// <summary>
        /// Creates a new random sampler that will produce sampleCount distinct values from an unknown number of calls
        /// using a specific instance of System.Random.
        /// </summary>
        /// <param name="sampleCount">The size of the sample.</param>
        /// <param name="randomGenerator">The System.Random instance to use, or null to generate a new instance.</param>
        public UnknownTotalSampler(int sampleCount, System.Random randomGenerator = null)
        {
            ArgumentChecker.CheckGreaterThanOrEqualTo(sampleCount, 1);

            this.SampleCount = sampleCount;
            this.RandomGenerator = randomGenerator ?? new System.Random();

            // R1. [Initialize]
            //
            this.ElementCount = this.SampleCount;
        }

        /// <summary>
        /// Returns the number of the sample entry that should be replaced by the current element.
        /// </summary>
        /// <returns>Number of the sample array entry that should be replaced by the current element, or 0 if no replacement should take place.</returns>
        public int EvaluateAnotherElement()
        {
            // R3. [Generate and test]
            //
            ++this.ElementCount;

            // Generate an integer value between 1 and this.ElementCount (inclusive).
            //
            int M = this.RandomGenerator.Next(1, this.ElementCount + 1);

            if (M <= this.SampleCount)
            {
                // R4. [Add to reservoir]
                //
                return M;
            }

            // R5. [Skip]
            //
            return 0;
        }
    }
}
