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
    public sealed class PoissonGenerator
    {
        /// <summary>
        /// The mean of the Poisson distribution.
        /// </summary>
        private ulong Mean { get; set; }

        /// <summary>
        /// The uniform random generator source.
        /// </summary>
        private System.Random UniformGenerator { get; set; }

        /// <summary>
        /// This random normal distribution generator will be used for large mean values.
        /// </summary>
        private NormalGenerator NormalGenerator { get; set; }

        /// <summary>
        /// Creates a random number generator with Poisson distribution of the specified mean
        /// using a specific instance of System.Random.
        /// </summary>
        /// <param name="mean">The mean of the Poisson distribution.</param>
        /// <param name="uniformGenerator">The System.Random instance to use, or null to generate a new instance.</param>
        public PoissonGenerator(ulong mean, System.Random uniformGenerator = null)
        {
            ArgumentChecker.CheckNotZero(mean);

            this.Mean = mean;
            this.UniformGenerator = uniformGenerator ?? new System.Random();

            // For larger mean values we approximate the Poisson distribution
            // with a normal distribution.
            //
            if (this.Mean > 30)
            {
                this.NormalGenerator = new NormalGenerator(this.UniformGenerator);
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
            if (this.NormalGenerator != null)
            {
                return NextNormal();
            }

            return NextPoisson();
        }

        private ulong NextPoisson()
        {
            // Q1. [Calculate exponential]
            //
            ulong value = 0;
            double p = Math.Exp(-(double)this.Mean);
            double q = 1.0;

            while (true)
            {
                // Q2. [Get uniform variable]
                //
                double U = this.UniformGenerator.NextDouble();

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

        private ulong NextNormal()
        {
            double z = this.NormalGenerator.Next();

            long value = (long)(this.Mean + z * Math.Sqrt(this.Mean) + 0.5);

            return (value >= 0) ? (ulong)value : 0;
        }
    }
}
