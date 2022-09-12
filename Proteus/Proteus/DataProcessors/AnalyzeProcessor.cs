﻿////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;

using LaurentiuCristofor.Proteus.Common;
using LaurentiuCristofor.Proteus.DataExtractors;

namespace LaurentiuCristofor.Proteus.DataProcessors
{
    /// <summary>
    /// A data processor that performs data analysis to extract some basic stats.
    /// </summary>
    public class AnalyzeProcessor : IDataProcessor<AnalyzeParameters, ParsedLine>
    {
        /// <summary>
        /// Parameters of this operation.
        /// </summary>
        protected AnalyzeParameters Parameters { get; set; }

        /// <summary>
        /// The DataAnalyzer instance that we will use to perform the analysis.
        /// </summary>
        public DataAnalyzer Analyzer { get; protected set; }

        public void Initialize(AnalyzeParameters processingParameters)
        {
            ArgumentChecker.CheckNotNegative(processingParameters.ValuesLimit);

            this.Parameters = processingParameters;

            this.Analyzer = new DataAnalyzer();
        }

        public bool Execute(ulong lineNumber, ParsedLine lineData)
        {
            // We may not always be able to extract data.
            // Ignore these cases; the extractor will already have printed a warning message.
            //
            if (lineData == null)
            {
                return true;
            }

            this.Analyzer.AnalyzeData(lineData.ExtractedData);

            return true;
        }

        public void CompleteExecution()
        {
            this.Analyzer.PostProcessAnalyzedData();

            this.Analyzer.OutputReport(this.Parameters.ValuesLimit);
        }
    }
}
