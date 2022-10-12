﻿////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
///
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;

using LaurentiuCristofor.Proteus.Common;
using LaurentiuCristofor.Proteus.Common.Types;
using LaurentiuCristofor.Proteus.Common.Random;

namespace LaurentiuCristofor.Proteus.DataGenerators
{
    /// <summary>
    /// A data generator that emits data with a specific distribution.
    /// </summary>
    public class DistributionGenerator : BaseStringGenerator, IDataGenerator<DistributionGenerationParameters>
    {
        public void Initialize(DistributionGenerationParameters generationParameters)
        {
            this.GenerationCount = generationParameters.GenerationCount;
            this.GeneratedCount = 0;

            Random uniformGenerator = (generationParameters.Seed >= 0) ? new Random(generationParameters.Seed) : new Random();

            switch (generationParameters.DistributionType)
            {
                case DistributionType.Uniform:
                    this.Generator = new UniformGenerator(uniformGenerator);
                    break;

                case DistributionType.Normal:
                    this.Generator = new NormalGenerator(uniformGenerator);
                    break;

                case DistributionType.Exponential:
                    {
                        ArgumentChecker.CheckNotNull<string>(generationParameters.DistributionMean);

                        double mean = double.Parse(generationParameters.DistributionMean);

                        this.Generator = new ExponentialGenerator(mean, uniformGenerator);
                        break;
                    }

                case DistributionType.Poisson:
                    {
                        ArgumentChecker.CheckNotNull<string>(generationParameters.DistributionMean);

                        ulong mean = ulong.Parse(generationParameters.DistributionMean);

                        this.Generator = new PoissonGenerator(mean, uniformGenerator);
                        break;
                    }

                default:
                    throw new ProteusException($"Internal error: Proteus is not handling distribution type '{generationParameters.DistributionType}'!");
            }
        }
    }
}
