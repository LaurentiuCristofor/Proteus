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
    /// A shuffler of TotalCount values.
    ///
    /// The algorithm used here is Algorithm P described in Knuth, TAOCP, volume 2, section 3.4.2,
    /// page 145 of the third edition.
    /// </summary>
    public sealed class Shuffler
    {
        /// <summary>
        /// The remaining shuffling steps.
        /// </summary>
        private int RemainingShufflingSteps { get; set; }

        /// <summary>
        /// The random generator source.
        /// </summary>
        private System.Random RandomGenerator { get; set; }

        /// <summary>
        /// Creates a new random shuffler using a specific instance of System.Random.
        /// </summary>
        /// <param name="totalCount">The count of values to shuffle.</param>
        /// <param name="randomGenerator">The System.Random instance to use, or null to generate a new instance.</param>
        public Shuffler(int totalCount, System.Random randomGenerator = null)
        {
            ArgumentChecker.CheckPositive(totalCount);

            this.RandomGenerator = randomGenerator ?? new System.Random();

            // P1. [Initialize]
            //
            this.RemainingShufflingSteps = totalCount;
        }

        /// <summary>
        /// Returns the next pair of values to exchange.
        /// </summary>
        /// <returns>A tuple indicating the numbers of the values to exchange, or null if the shuffling has ended.</returns>
        public Tuple<int, int> NextShuffle()
        {
            if (this.RemainingShufflingSteps == 0)
            {
                return null;
            }

            // P2. [Generate U]
            //
            double U = this.RandomGenerator.NextDouble();

            // P3. [Exchange]
            // valueNumber will get a value between 1 and this.RemainingShufflingSteps.
            //
            int valueNumber = (int)Math.Floor(this.RemainingShufflingSteps * U) + 1;
            int otherValueNumber = this.RemainingShufflingSteps;

            // P4. [Decrease step]
            //
            --this.RemainingShufflingSteps;

            return new Tuple<int, int>(valueNumber, otherValueNumber);
        }
    }
}
