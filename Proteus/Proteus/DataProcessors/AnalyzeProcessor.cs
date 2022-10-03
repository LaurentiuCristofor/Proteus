////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// 
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using LaurentiuCristofor.Proteus.Common;
using LaurentiuCristofor.Proteus.Common.Utilities;
using LaurentiuCristofor.Proteus.DataExtractors;
using LaurentiuCristofor.Proteus.DataProcessors.Parameters;

namespace LaurentiuCristofor.Proteus.DataProcessors
{
    /// <summary>
    /// A data processor that performs data analysis to extract some basic stats.
    /// </summary>
    public class AnalyzeProcessor : IDataProcessor<AnalyzeParameters, OneExtractedValue>
    {
        protected AnalyzeParameters Parameters { get; set; }

        /// <summary>
        /// The DataAnalyzer instance that we will use to perform the analysis.
        /// </summary>
        public DataAnalyzer Analyzer { get; protected set; }

        public void Initialize(AnalyzeParameters processingParameters)
        {
            ArgumentChecker.CheckPositive(processingParameters.OutputLimit);

            this.Parameters = processingParameters;

            this.Analyzer = new DataAnalyzer(this.Parameters.DataType);
        }

        public bool Execute(ulong lineNumber, OneExtractedValue lineData)
        {
            this.Analyzer.AnalyzeData(lineData.ExtractedData);

            return true;
        }

        public void CompleteExecution()
        {
            this.Analyzer.PostProcessAnalyzedData();

            this.Analyzer.OutputReport(this.Parameters.OutputLimit);
        }
    }
}
