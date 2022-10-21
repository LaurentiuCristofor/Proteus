////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
///
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

namespace LaurentiuCristofor.Proteus.Common.Numerical
{
    /// <summary>
    /// A generator of values in geometric progression.
    /// </summary>
    public sealed class GeometricGenerator : IStringGenerator
    {
        /// <summary>
        /// The next value that should be generated.
        /// </summary>
        private double NextValue { get; set; }

        /// <summary>
        /// The ratio value of the geometric progression.
        /// </summary>
        private double RatioValue { get; set; }

        /// <summary>
        /// Creates a geometric progression generator.
        /// </summary>
        /// <param name="startingValue">The starting value of the progression.</param>
        /// <param name="ratioValue">The ratio value of the progression.</param>
        public GeometricGenerator(double startingValue, double ratioValue)
        {
            NextValue = startingValue;
            RatioValue = ratioValue;
        }

        /// <summary>
        /// Returns the next geometric progression value.
        /// </summary>
        /// <returns>The next value in the geometric progression.</returns>
        public double Next()
        {
            double value = NextValue;

            NextValue *= RatioValue;

            return value;
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
