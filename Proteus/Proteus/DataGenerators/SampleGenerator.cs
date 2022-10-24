////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
///
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;

using LaurentiuCristofor.Proteus.Common.Random;

namespace LaurentiuCristofor.Proteus.DataGenerators
{
    /// <summary>
    /// A data generator that emits a sample of data from a specific interval.
    /// </summary>
    public class SampleGenerator : IDataGenerator<SampleGenerationParameters>
    {
        /// <summary>
        /// The sampler to use for sampling the lines.
        /// </summary>
        protected KnownTotalSampler Sampler { get; set; }

        public void Initialize(SampleGenerationParameters generationParameters)
        {
            // If we have a positive seed value, use it for the initialization of the uniform distribution generator.
            //
            Random uniformGenerator = (generationParameters.Seed >= 0) ? new Random(generationParameters.Seed) : new Random();

            // Initialize our sampler.
            //
            Sampler = new KnownTotalSampler(generationParameters.TotalCount, generationParameters.GenerationCount, uniformGenerator);
        }

        public string NextString()
        {
            ulong nextSampleValue = Sampler.Next();

            if (nextSampleValue == 0)
            {
                return null;
            }

            return nextSampleValue.ToString();
        }
    }
}
