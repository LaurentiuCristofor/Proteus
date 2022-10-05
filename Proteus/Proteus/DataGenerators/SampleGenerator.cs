////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
///
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using LaurentiuCristofor.Proteus.Common.Random;

namespace LaurentiuCristofor.Proteus.DataGenerators
{
    /// <summary>
    /// A data generator that emits a sample of data from a specific interval.
    /// </summary>
    public class SampleGenerator : IDataGenerator<SampleGenerationParameters>
    {
        protected SampleGenerationParameters Parameters { get; set; }

        protected KnownTotalSampler Sampler { get; set; }

        public void Initialize(SampleGenerationParameters generationParameters)
        {
            this.Parameters = generationParameters;

            // If we have a positive seed value, use it for the initialization of the uniform distribution generator.
            //
            System.Random uniformGenerator;
            if (generationParameters.Seed >= 0)
            {
                uniformGenerator = new System.Random(generationParameters.Seed);
            }
            else
            {
                uniformGenerator = new System.Random();
            }

            // Initialize our sampler.
            //
            this.Sampler = new KnownTotalSampler(this.Parameters.TotalCount, this.Parameters.GenerationCount, uniformGenerator);
        }

        public string Generate()
        {
            ulong nextSampleValue = this.Sampler.NextSampleValue();

            if (nextSampleValue == 0)
            {
                return null;
            }

            return nextSampleValue.ToString();
        }
    }
}
