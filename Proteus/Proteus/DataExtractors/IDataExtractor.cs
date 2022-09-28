////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// 
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

namespace LaurentiuCristofor.Proteus.DataExtractors
{
    /// <summary>
    /// An interface for extracting data from an input text line.
    /// </summary>
    /// <typeparam name="TExtractionParameters">The parameters of the extraction operation.</typeparam>
    /// <typeparam name="TExtractedData">The type of object that will be extracted.</typeparam>
    public interface IDataExtractor<TExtractionParameters, TExtractedData>
    {
        /// <summary>
        /// Initializes the extractor with specific parameters.
        /// </summary>
        /// <param name="extractionParameters">The extraction parameters.</param>
        void Initialize(TExtractionParameters extractionParameters);

        /// <summary>
        /// Extract data from a line.
        /// </summary>
        /// <param name="lineNumber">The current line number corresponding to the line.</param>
        /// <param name="inputLine">The line to process.</param>
        /// <returns>The data extracted from the line or null if the extraction was not possible.</returns>
        TExtractedData ExtractData(ulong lineNumber, string line);
    }
}
