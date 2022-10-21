////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
///
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

namespace LaurentiuCristofor.Proteus.DataGenerators
{
    /// <summary>
    /// Includes parameters common to all data generation operations.
    /// </summary>
    public class BaseGenerationParameters
    {
        /// <summary>
        /// The count of data that should be generated.
        /// </summary>
        public ulong GenerationCount { get; set; }

        public BaseGenerationParameters(
            ulong generationCount)
        {
            GenerationCount = generationCount;
        }
    }
}
