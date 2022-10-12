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
    /// An extension of IStringGenerator that provides an additional API for initializing the generator.
    /// </summary>
    /// <typeparam name="TGenerationParameters">The parameters of the generation operation.</typeparam>
    public interface IDataGenerator<TGenerationParameters> : IStringGenerator
    {
        /// <summary>
        /// Initializes the generator with specific parameters.
        /// </summary>
        /// <param name="generationParameters">The generation parameters.</param>
        void Initialize(TGenerationParameters generationParameters);
    }
}
