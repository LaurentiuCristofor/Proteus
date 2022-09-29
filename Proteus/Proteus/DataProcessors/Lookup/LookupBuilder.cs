////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// 
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;

using LaurentiuCristofor.Proteus.Common.ValueHolders;
using LaurentiuCristofor.Proteus.DataExtractors;

namespace LaurentiuCristofor.Proteus.DataProcessors.Lookup
{
    /// <summary>
    /// A builder of a HashSet data structure, used for plain lookup operations.
    /// </summary>
    public class LookupBuilder : ILookupDataStructureBuilder<OneExtractedValue, HashSet<IValueHolder>>
    {
        /// <summary>
        /// The lookup data structure that we'll construct.
        /// </summary>
        protected HashSet<IValueHolder> LookupSet { get; set; }

        public LookupBuilder()
        {
            this.LookupSet = new HashSet<IValueHolder>();
        }

        /// <summary>
        /// Execute the builder on a unit of data.
        /// </summary>
        /// <param name="lineData">The data to process.</param>
        /// <returns>A reference to the lookup data structure that was built so far.</returns>
        public HashSet<IValueHolder> Execute(OneExtractedValue lineData)
        {
            this.LookupSet.Add(lineData.ExtractedData);

            return this.LookupSet;
        }
    }
}
