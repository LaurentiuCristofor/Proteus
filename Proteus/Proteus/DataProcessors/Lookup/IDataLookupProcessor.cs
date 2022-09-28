////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// 
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

namespace LaurentiuCristofor.Proteus.DataProcessors.Lookup
{
    /// <summary>
    /// An interface for a data processing unit that uses a lookup data structure.
    /// </summary>
    /// <typeparam name="TProcessingParameters">The parameters of the processing operation.</typeparam>
    /// <typeparam name="TLookupDataStructure">The type of lookup data structure that will be used in processing.</typeparam>
    /// <typeparam name="TInputData">The type of data that will be processed.</typeparam>
    public interface IDataLookupProcessor<TProcessingParameters, TLookupDataStructure, TInputData> : IDataProcessor<TProcessingParameters, TInputData>
    {
        /// <summary>
        /// The lookup data structure may not be available at the time of the creation of the processor,
        /// so this method allows setting it separately from the processing parameters.
        /// </summary>
        /// <param name="lookupDataStructure">The lookup data structure to use in processing.</param>
        void AddLookupDataStructure(TLookupDataStructure lookupDataStructure);
    }
}
