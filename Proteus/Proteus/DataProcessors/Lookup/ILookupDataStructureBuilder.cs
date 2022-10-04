////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
///
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

namespace LaurentiuCristofor.Proteus.DataProcessors.Lookup
{
    /// <summary>
    /// An interface for a lookup data structure builder.
    /// </summary>
    /// <typeparam name="TInputData">The type of data that will be processed.</typeparam>
    /// <typeparam name="TLookupDataStructure">The type of data structure that will be produced.</typeparam>
    public interface ILookupDataStructureBuilder<TInputData, TLookupDataStructure>
    {
        /// <summary>
        /// Executes the builder on a unit of data.
        /// </summary>
        /// <param name="inputData">The data to process.</param>
        /// <returns>A reference to the lookup data structure that was built so far.</returns>
        TLookupDataStructure Execute(TInputData inputData);
    }
}
