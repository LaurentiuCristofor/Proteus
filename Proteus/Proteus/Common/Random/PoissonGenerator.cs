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
    /// A generator of unsigned integer values with Poisson distribution.
    ///
    /// The algorithm used here is Algorithm Q described in Knuth, TAOCP, volume 2, section 3.4.1,
    /// page 117 of the second edition.
    /// </summary>
    public sealed class PoissonGenerator : IStringGenerator
    {
        /// <summary>
        /// The mean of the Poisson distribution.
        /// </summary>
        private ulong Mean { get; set; }

        /// <summary>
        /// The uniform random generator source.
        /// </summary>
        private System.Random RandomGenerator { get; set; }

        /// <summary>
        /// This random normal distribution generator will be used for large mean values.
        /// </summary>
        private NormalGenerator NormalGenerator { get; set; }

        /// <summary>
        /// Creates a random number generator with Poisson distribution of the specified mean
        /// using a specific instance of System.Random.
        /// </summary>
        /// <param name="mean">The mean of the Poisson distribution.</param>
        /// <param name="randomGenerator">The System.Random instance to use, or null to generate a new instance.</param>
        public PoissonGenerator(ulong mean, System.Random randomGenerator = null)
        {
            ArgumentChecker.CheckGreaterThanOrEqualTo(mean, 1UL);

            Mean = mean;
            RandomGenerator = randomGenerator ?? new System.Random();

            // For larger mean values we approximate the Poisson distribution
            // with a normal distribution.
            //
            if (Mean > 30)
            {
                NormalGenerator = new NormalGenerator(RandomGenerator);
            }
        }

        /// <summary>
        /// Returns a new unsigned long value with the requested Poisson distribution.
        /// </summary>
        /// <returns>A new unsigned long value with the requested Poisson distribution.</returns>
        public ulong Next()
        {
            // Generate a normal distribution if we initialized the generator for it.
            //
            if (NormalGenerator != null)
            {
                return NextNormal();
            }

            return NextPoisson();
        }

        /// <summary>
        /// Returns a new unsigned long value with the requested Poisson distribution.
        /// </summary>
        /// <returns>A new unsigned long value with the requested Poisson distribution.</returns>
        private ulong NextPoisson()
        {
            // Q1. [Calculate exponential]
            //
            ulong value = 0;
            double p = Math.Exp(-(double)Mean);
            double q = 1.0;

            while (true)
            {
                // Q2. [Get uniform variable]
                //
                double U = RandomGenerator.NextDouble();

                // Q3. [Multiply]
                //
                q *= U;

                // Q4. [Test]
                //
                if (q >= p)
                {
                    ++value;
                }
                else
                {
                    return value;
                }
            }
        }

        /// <summary>
        /// Returns a new unsigned long value using a normal distribution approximation of a Poisson distribution.
        /// </summary>
        /// <returns>A new unsigned long value generated using a normal distribution approximation of a Poisson distribution.</returns>
        private ulong NextNormal()
        {
            double z = NormalGenerator.Next();

            long value = (long)(Mean + z * Math.Sqrt(Mean) + 0.5);

            return (value >= 0) ? (ulong)value : 0;
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
