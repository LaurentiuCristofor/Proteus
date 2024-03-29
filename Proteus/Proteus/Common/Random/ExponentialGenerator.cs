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
    /// A generator of floating point values with exponential distribution.
    ///
    /// The method used here is the logarithm method described in Knuth, TAOCP, volume 2, section 3.4.1,
    /// page 133 of the third edition.
    /// </summary>
    public sealed class ExponentialGenerator : IStringGenerator
    {
        /// <summary>
        /// The mean of the exponential distribution.
        /// </summary>
        private double Mean { get; set; }

        /// <summary>
        /// The uniform random generator source.
        /// </summary>
        private System.Random RandomGenerator { get; set; }

        /// <summary>
        /// Creates a random number generator with exponential distribution of the specified mean
        /// using a specific instance of System.Random.
        /// </summary>
        /// <param name="mean">The mean of the exponential distribution.</param>
        /// <param name="randomGenerator">The System.Random instance to use, or null to generate a new instance.</param>
        public ExponentialGenerator(double mean = 1.0, System.Random randomGenerator = null)
        {
            Mean = mean;
            RandomGenerator = randomGenerator ?? new System.Random();
        }

        /// <summary>
        /// Returns a new double value with the requested exponential distribution.
        /// </summary>
        /// <returns>A new double value with the requested exponential distribution.</returns>
        public double Next()
        {
            double U;

            do
            {
                U = RandomGenerator.NextDouble();
            }
            while (U == 0.0);

            return Mean * (-Math.Log(U));
        }

        /// <summary>
        /// Implements IStringGenerator.
        /// </summary>
        /// <returns>A string representation of the next generated value.</returns>
        public string NextString()
        {
            return Next().ToString();
        }
    }
}
