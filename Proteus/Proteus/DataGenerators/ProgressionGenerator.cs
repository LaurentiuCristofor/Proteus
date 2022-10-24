////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
///
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using LaurentiuCristofor.Proteus.Common;
using LaurentiuCristofor.Proteus.Common.Types;
using LaurentiuCristofor.Proteus.Common.Numerical;

namespace LaurentiuCristofor.Proteus.DataGenerators
{
    /// <summary>
    /// A data generator that emits data of a specific progression.
    /// </summary>
    public class ProgressionGenerator : BaseStringGenerator, IDataGenerator<ProgressionGenerationParameters>
    {
        public void Initialize(ProgressionGenerationParameters generationParameters)
        {
            GenerationCount = generationParameters.GenerationCount;
            GeneratedCount = 0;

            switch (generationParameters.ProgressionType)
            {
                case ProgressionType.Arithmetic:
                    ArgumentChecker.CheckPresence(generationParameters.ProgressionParameters, 0);
                    ArgumentChecker.CheckPresence(generationParameters.ProgressionParameters, 1);

                    Generator = new ArithmeticGenerator(generationParameters.ProgressionParameters[0], generationParameters.ProgressionParameters[1]);
                    break;

                case ProgressionType.Geometric:
                    ArgumentChecker.CheckPresence(generationParameters.ProgressionParameters, 0);
                    ArgumentChecker.CheckPresence(generationParameters.ProgressionParameters, 1);

                    Generator = new GeometricGenerator(generationParameters.ProgressionParameters[0], generationParameters.ProgressionParameters[1]);
                    break;

                case ProgressionType.Harmonic:
                    ArgumentChecker.CheckPresence(generationParameters.ProgressionParameters, 0);
                    ArgumentChecker.CheckPresence(generationParameters.ProgressionParameters, 1);

                    Generator = new HarmonicGenerator(generationParameters.ProgressionParameters[0], generationParameters.ProgressionParameters[1]);
                    break;

                case ProgressionType.Factorial:
                    Generator = new FactorialGenerator();
                    break;

                case ProgressionType.Fibonacci:
                    Generator = new FibonacciGenerator();
                    break;

                default:
                    throw new ProteusException($"Internal error: Proteus is not handling progression type '{generationParameters.ProgressionType}'!");
            }
        }
    }
}
