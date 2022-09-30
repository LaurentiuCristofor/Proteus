////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// 
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

namespace LaurentiuCristofor.Proteus.DataProcessors.Dual
{
    /// <summary>
    /// An interface for a data processing unit that processes two pieces of data at a time.
    /// </summary>
    /// <typeparam name="TProcessingParameters">The parameters of the processing operation.</typeparam>
    /// <typeparam name="TInputData">The type of data that will be processed.</typeparam>
    public interface IDualDataProcessor<TProcessingParameters, TInputData>
    {
        /// <summary>
        /// Initializes the processor with specific processing parameters.
        /// </summary>
        /// <param name="processingParameters">The parameters for the processing.</param>
        void Initialize(TProcessingParameters processingParameters);

        /// <summary>
        /// Executes the processor on two units of data.
        /// </summary>
        /// <param name="hasProcessedFirstFile">Indicates that we have processed the first file.</param>
        /// <param name="firstLineNumber">The current line number corresponding to the first input data.</param>
        /// <param name="firstInputData">The first data to process.</param>
        /// <param name="hasProcessedSecondFile">Indicates that we have processed the second file.</param>
        /// <param name="secondLineNumber">The current line number corresponding to the second input data.</param>
        /// <param name="secondInputData">The second data to process.</param>
        /// <returns>True if processing should continue; false if it should end.</returns>
        ProcessingActionType Execute(
            bool hasProcessedFirstFile, ulong firstLineNumber, TInputData firstInputData,
            bool hasProcessedSecondFile, ulong secondLineNumber, TInputData secondInputData);

        /// <summary>
        /// This method should be called after completing the processing, to finalize the processing.
        /// </summary>
        void CompleteExecution();
    }
}
