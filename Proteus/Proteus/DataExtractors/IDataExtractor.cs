////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

namespace LaurentiuCristofor.Proteus.DataExtractors
{
    /// <summary>
    /// An interface for extracting specific data from an input text line.
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
        /// Extract data from input string.
        /// </summary>
        /// <param name="lineNumber">The current line number corresponding to the input data.</param>
        /// <param name="inputData">The input string; typically, a row from a text file.</param>
        /// <returns>The data that could be extracted from the string or null if extraction was not possible.</returns>
        TExtractedData ExtractData(ulong lineNumber, string inputData);
    }
}
