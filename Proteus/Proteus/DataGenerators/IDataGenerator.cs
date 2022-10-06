////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
///
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

namespace LaurentiuCristofor.Proteus.DataGenerators
{
    /// <summary>
    /// An interface for generating data.
    /// </summary>
    /// <typeparam name="TGenerationParameters">The parameters of the generation operation.</typeparam>
    public interface IDataGenerator<TGenerationParameters>
    {
        /// <summary>
        /// Initializes the generator with specific parameters.
        /// </summary>
        /// <param name="extractionParameters">The generation parameters.</param>
        void Initialize(TGenerationParameters generationParameters);

        /// <summary>
        /// Generates a piece of data and outputs its string representation.
        /// </summary>
        /// <returns>The string representation of the generated data or null if the generation has been completed.</returns>
        string Generate();
    }
}
