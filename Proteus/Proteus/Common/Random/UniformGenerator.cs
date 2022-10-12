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
    /// A generator of floating point values with uniform distribution.
    ///
    /// This is just a wrapper of System.Random that implements IStringGenerator.
    /// </summary>
    public sealed class UniformGenerator : IStringGenerator
    {
        /// <summary>
        /// The uniform random generator source.
        /// </summary>
        public System.Random RandomGenerator { get; set; }

        /// <summary>
        /// Creates a UniformGenerator wrapper for a System.Random instance.
        /// </summary>
        /// <param name="randomGenerator">The System.Random instance to use, or null to generate a new instance.</param>
        public UniformGenerator(System.Random randomGenerator)
        {
            this.RandomGenerator = randomGenerator ?? new System.Random();
        }

        /// <summary>
        /// Returns a new double value with uniform distribution.
        /// </summary>
        /// <returns>A new double value with uniform distribution.</returns>
        public double Next()
        {
            return this.RandomGenerator.NextDouble();
        }

        /// <summary>
        /// Implements IStringGenerator.
        /// </summary>
        /// <returns>A string representation of the next generated value.</returns>
        public string NextString()
        {
            return this.Next().ToString();
        }
    }
}
