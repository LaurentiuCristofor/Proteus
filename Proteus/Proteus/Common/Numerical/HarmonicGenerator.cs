////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
///
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

namespace LaurentiuCristofor.Proteus.Common.Numerical
{
    /// <summary>
    /// A generator of values in harmonic progression.
    /// </summary>
    public sealed class HarmonicGenerator : IStringGenerator
    {
        /// <summary>
        /// The arithmetic progression generator used for generating the harmonic progression.
        /// </summary>
        private ArithmeticGenerator Generator;

        /// <summary>
        /// Creates a harmonic progression generator.
        /// 
        /// The harmonic progression is implemented based on an arithmetic one.
        /// </summary>
        /// <param name="startingValue">The starting value of the arithmetic progression.</param>
        /// <param name="deltaValue">The delta (difference) value of the arithmetic progression.</param>
        public HarmonicGenerator(double startingValue, double deltaValue)
        {
            // Starting value cannot be 0, to avoid division by 0.
            //
            ArgumentChecker.CheckNotEqual(startingValue, 0);

            this.Generator = new ArithmeticGenerator(startingValue, deltaValue);
        }

        /// <summary>
        /// Returns the next harmonic progression value.
        /// </summary>
        /// <returns>The next value in the harmonic progression.</returns>
        public double Next()
        {
            // Compute the next arithmetic value and then its reciprocal.
            //
            double arithmeticValue = this.Generator.Next();
            double harmonicValue = 1 / arithmeticValue;

            return harmonicValue;
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
