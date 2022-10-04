////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
///
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

namespace LaurentiuCristofor.Proteus.DataProcessors
{
    /// <summary>
    /// An interface for a data processing unit.
    /// </summary>
    /// <typeparam name="TProcessingParameters">The parameters of the processing operation.</typeparam>
    /// <typeparam name="TInputData">The type of data that will be processed.</typeparam>
    public interface IDataProcessor<TProcessingParameters, TInputData>
    {
        /// <summary>
        /// Initializes the processor with specific processing parameters.
        /// </summary>
        /// <param name="processingParameters">The parameters for the processing.</param>
        void Initialize(TProcessingParameters processingParameters);

        /// <summary>
        /// Executes the processor on a unit of data.
        /// </summary>
        /// <param name="lineNumber">The current line number corresponding to the input data.</param>
        /// <param name="inputData">The data to process.</param>
        /// <returns>True if processing should continue; false if it should end.</returns>
        bool Execute(ulong lineNumber, TInputData inputData);

        /// <summary>
        /// This method should be called after completing the processing, to finalize the processing.
        /// </summary>
        void CompleteExecution();
    }
}
