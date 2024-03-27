////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
///
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using LaurentiuCristofor.Proteus.Common.DataHolders;
using LaurentiuCristofor.Proteus.Common.Utilities;
using LaurentiuCristofor.Proteus.DataExtractors;
using LaurentiuCristofor.Proteus.DataProcessors.Parameters;

namespace LaurentiuCristofor.Proteus.DataProcessors
{
    /// <summary>
    /// A data processor that performs data analysis to extract some basic stats.
    /// </summary>
    public class ConditionalEntropyProcessor : IDataProcessor<ConditionalEntropyParameters, TwoExtractedValues>
    {
        /// <summary>
        /// The ConditionalEntropyCalculator instance that we will use to perform the calculation.
        /// </summary>
        public ConditionalEntropyCalculator Calculator { get; protected set; }

        public void Initialize(ConditionalEntropyParameters processingParameters)
        {
            Calculator = new ConditionalEntropyCalculator(processingParameters.FirstDataType, processingParameters.SecondDataType);
        }

        public bool Execute(ulong lineNumber, TwoExtractedValues lineData)
        {
            Calculator.Process(lineData.ExtractedData, lineData.SecondExtractedData);

            return true;
        }

        public void CompleteExecution()
        {
            Calculator.Calculate();

            Calculator.OutputResult();
        }
    }
}
