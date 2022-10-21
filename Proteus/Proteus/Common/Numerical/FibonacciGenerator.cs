////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
///
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

namespace LaurentiuCristofor.Proteus.Common.Numerical
{
    /// <summary>
    /// A generator of Fibonacci values.
    /// 
    /// We use double variables to avoid overflowing ulong ones.
    /// </summary>
    public sealed class FibonacciGenerator : IStringGenerator
    {
        /// <summary>
        /// Holds the last two values of the Fibonacci progression.
        /// 0 indicates that the value has not yet been generated.
        /// </summary>
        private double[] LastTwoValues { get; set; }

        /// <summary>
        /// Indicates the index within LastTwoValues where the next value should be stored after it is generated.
        /// </summary>
        private int NextValueIndex { get; set; } 

        /// <summary>
        /// Creates a generator of Fibonacci values.
        /// </summary>
        public FibonacciGenerator()
        {
            LastTwoValues = new double[] { 0, 0 };
            NextValueIndex = 0;
        }

        /// <summary>
        /// Returns the next Fibonacci value.
        /// </summary>
        /// <returns>The next value in the Fibonacci progression.</returns>
        public double Next()
        {
            // Adds the last two values and stores them at the position indicated by NextValueIndex.
            // The first two values represent special cases for which we hardcode the generation.
            //
            LastTwoValues[NextValueIndex] = (LastTwoValues[NextValueIndex] == 0) ? 1 : LastTwoValues[0] + LastTwoValues[1];

            // Copy the value that we should output, before updating the NextValueIndex.
            //
            double nextValue = LastTwoValues[NextValueIndex];

            // Position NextValueIndex to where we should store the next value.
            // Use modulo to nicely wrap around the array end.
            //
            NextValueIndex = (NextValueIndex + 1) % 2;

            return nextValue;
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
