////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
///
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

namespace LaurentiuCristofor.Proteus.Common.Numerical
{
    /// <summary>
    /// A generator of values in arithmetic progression.
    /// </summary>
    public sealed class ArithmeticGenerator : IStringGenerator
    {
        /// <summary>
        /// The next value that should be generated.
        /// </summary>
        private double NextValue { get; set; }

        /// <summary>
        /// The delta value of the arithmetic progression.
        /// </summary>
        private double DeltaValue { get; set; } 

        /// <summary>
        /// Creates an arithmetic progression generator.
        /// </summary>
        /// <param name="startingValue">The starting value of the progression.</param>
        /// <param name="deltaValue">The delta (difference) value of the progression.</param>
        public ArithmeticGenerator(double startingValue, double deltaValue)
        {
            this.NextValue = startingValue;
            this.DeltaValue = deltaValue;
        }

        /// <summary>
        /// Returns the next arithmetic progression value.
        /// </summary>
        /// <returns>The next value in the arithmetic progression.</returns>
        public double Next()
        {
            double value = this.NextValue;

            this.NextValue += this.DeltaValue;

            return value;
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
