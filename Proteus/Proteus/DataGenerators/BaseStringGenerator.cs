////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
///
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using LaurentiuCristofor.Proteus.Common;

namespace LaurentiuCristofor.Proteus.DataGenerators
{
    /// <summary>
    /// A base data generator class that uses an IStringGenerator instance to generate data up to a requested count.
    /// </summary>
    public abstract class BaseStringGenerator : IStringGenerator
    {
        /// <summary>
        /// The count of data to generate.
        /// </summary>
        protected ulong GenerationCount { get; set; }

        /// <summary>
        /// The generator instance.
        /// </summary>
        protected IStringGenerator Generator { get; set; }

        /// <summary>
        /// The count of data generated so far.
        /// </summary>
        protected ulong GeneratedCount { get; set; }

        public string NextString()
        {
            // Indicate end of generation if we achieved the requested output.
            //
            if (GeneratedCount == GenerationCount)
            {
                return null;
            }

            // Count this data generation.
            //
            ++GeneratedCount;

            // Return the string representation of the next generated value.
            //
            return Generator.NextString();
        }
    }
}
