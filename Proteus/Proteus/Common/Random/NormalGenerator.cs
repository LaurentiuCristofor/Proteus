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
    /// A generator of floating point values with normal distribution.
    ///
    /// The algorithm used here is Algorithm P described in Knuth, TAOCP, volume 2, section 3.4.1,
    /// page 122 of the third edition.
    ///
    /// This algorithm implements the polar method of G.E.P. Box, M.E. Muller, and G. Marsaglia.
    /// </summary>
    public sealed class NormalGenerator
    {
        /// <summary>
        /// The uniform random generator source.
        /// </summary>
        private System.Random UniformGenerator { get; set; }

        /// <summary>
        /// Tells whether the generator already has the next value,
        /// because generation generates two values at a time.
        /// </summary>
        private bool HasNextValue { get; set; }

        /// <summary>
        /// The next value to return if one was calculated in advance.
        /// </summary>
        private double NextValue { get; set; }

        /// <summary>
        /// Creates a random number generator with normal distribution
        /// using a specific instance of System.Random.
        /// </summary>
        /// <param name="uniformGenerator">The System.Random instance to use, or null to generate a new instance.</param>
        public NormalGenerator(System.Random uniformGenerator = null)
        {
            this.UniformGenerator = uniformGenerator ?? new System.Random();
        }

        /// <summary>
        /// Returns a new double value with the requested normal distribution.
        /// </summary>
        /// <returns>A new double value with the requested normal distribution.</returns>
        public double Next()
        {
            if (this.HasNextValue)
            {
                this.HasNextValue = false;
                return this.NextValue;
            }

            while (true)
            {
                // P1: [Get uniform variables]
                //
                double U1 = this.UniformGenerator.NextDouble();
                double U2 = this.UniformGenerator.NextDouble();

                // Distributes the values between -1 and 1.
                //
                double V1 = 2 * U1 - 1;
                double V2 = 2 * U2 - 1;

                // P2: [Compute S]
                //
                double S = V1 * V1 + V2 * V2;

                // P3: [Is S >= 1]
                //
                if (S >= 1 || S == 0.0)
                {
                    continue;
                }

                // P4: [Compute X1, X2]
                //
                double squareRootFactor = Math.Sqrt(-2 * Math.Log(S) / S);
                double X1 = V1 * squareRootFactor;
                double X2 = V2 * squareRootFactor;

                this.NextValue = X2;
                this.HasNextValue = true;
                return X1;
            }
        }
    }
}
