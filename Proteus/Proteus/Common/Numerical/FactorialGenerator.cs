////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
///
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

namespace LaurentiuCristofor.Proteus.Common.Numerical
{
    /// <summary>
    /// A generator of factorial values.
    /// 
    /// We use double variables, because ulong ones would overflow fast.
    /// </summary>
    public sealed class FactorialGenerator : IStringGenerator
    {
        /// <summary>
        /// The next value that should be generated.
        /// </summary>
        private double NextValue { get; set; }

        /// <summary>
        /// The multiplier value that should be used to generate the next value after NextValue.
        /// </summary>
        private double NextMultiplierValue { get; set; } 

        /// <summary>
        /// Creates a generator of factorial values.
        /// </summary>
        public FactorialGenerator()
        {
            NextValue = 1;
            NextMultiplierValue = 2;
        }

        /// <summary>
        /// Returns the next factorial value.
        /// </summary>
        /// <returns>The next value in the factorial progression.</returns>
        public double Next()
        {
            double value = NextValue;

            // Generate next value and then increment multiplier as well.
            //
            NextValue *= NextMultiplierValue;
            ++NextMultiplierValue;

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
