////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
///
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using LaurentiuCristofor.Proteus.Common;
using LaurentiuCristofor.Proteus.Common.Types;
using LaurentiuCristofor.Proteus.Common.Random;

namespace LaurentiuCristofor.Proteus.DataGenerators
{
    /// <summary>
    /// A data generator that emits data with a specific distribution.
    /// </summary>
    public class DistributionGenerator : IDataGenerator<DistributionGenerationParameters>
    {
        protected DistributionGenerationParameters Parameters { get; set; }

        /// <summary>
        /// The uniform generator to use with all random generations.
        /// </summary>
        protected System.Random UniformGenerator { get; set; }

        /// <summary>
        /// The generator used for normal distributions.
        /// </summary>
        protected NormalGenerator NormalGenerator { get; set; }

        /// <summary>
        /// The generator used for exponential distributions.
        /// </summary>
        protected ExponentialGenerator ExponentialGenerator { get; set; }

        /// <summary>
        /// The generator used for Poisson distributions.
        /// </summary>
        protected PoissonGenerator PoissonGenerator { get; set; }

        /// <summary>
        /// The count of data generated so far.
        /// </summary>
        protected ulong GeneratedCount { get; set; }

        public void Initialize(DistributionGenerationParameters generationParameters)
        {
            this.Parameters = generationParameters;
            this.GeneratedCount = 0;

            // We always initialize the uniform distribution generator,
            // so that it can be used by any other generator.
            // If we have a positive seed value, use it for the initialization.
            //
            if (generationParameters.Seed >= 0)
            {
                this.UniformGenerator = new System.Random(generationParameters.Seed);
            }
            else
            {
                this.UniformGenerator = new System.Random();
            }

            switch (this.Parameters.DistributionType)
            {
                case DistributionType.Uniform:
                    // Nothing to do - we already initialized the generator.
                    //
                    break;

                case DistributionType.Normal:
                    this.NormalGenerator = new NormalGenerator(this.UniformGenerator);
                    break;

                case DistributionType.Exponential:
                    {
                        ArgumentChecker.CheckNotNull<string>(this.Parameters.DistributionMean);

                        double mean = double.Parse(this.Parameters.DistributionMean);

                        this.ExponentialGenerator = new ExponentialGenerator(mean, this.UniformGenerator);
                        break;
                    }

                case DistributionType.Poisson:
                    {
                        ArgumentChecker.CheckNotNull<string>(this.Parameters.DistributionMean);

                        ulong mean = ulong.Parse(this.Parameters.DistributionMean);

                        this.PoissonGenerator = new PoissonGenerator(mean, this.UniformGenerator);
                        break;
                    }

                default:
                    throw new ProteusException($"Internal error: Proteus is not handling data distribution type '{this.Parameters.DistributionType}'!");
            }
        }

        public string Generate()
        {
            // Indicate end of generation if we achieved the requested output.
            //
            if (this.GeneratedCount == this.Parameters.GenerationCount)
            {
                return null;
            }

            // Count this data generation.
            //
            ++this.GeneratedCount;

            switch (this.Parameters.DistributionType)
            {
                case DistributionType.Uniform:
                    return this.UniformGenerator.NextDouble().ToString();

                case DistributionType.Normal:
                    return this.NormalGenerator.Next().ToString();

                case DistributionType.Exponential:
                    return this.ExponentialGenerator.Next().ToString();

                case DistributionType.Poisson:
                    return this.PoissonGenerator.Next().ToString();

                default:
                    throw new ProteusException($"Internal error: Proteus is not handling data distribution type '{this.Parameters.DistributionType}'!");
            }
        }
    }
}
