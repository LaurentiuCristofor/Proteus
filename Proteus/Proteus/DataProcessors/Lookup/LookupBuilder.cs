////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
///
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;

using LaurentiuCristofor.Proteus.Common.DataHolders;
using LaurentiuCristofor.Proteus.DataExtractors;

namespace LaurentiuCristofor.Proteus.DataProcessors.Lookup
{
    /// <summary>
    /// A builder of a HashSet data structure, used for plain lookup operations.
    /// </summary>
    public class LookupBuilder : ILookupDataStructureBuilder<OneExtractedValue, HashSet<IDataHolder>>
    {
        /// <summary>
        /// The lookup data structure that we'll construct.
        /// </summary>
        protected HashSet<IDataHolder> LookupSet { get; set; }

        public LookupBuilder()
        {
            this.LookupSet = new HashSet<IDataHolder>();
        }

        public HashSet<IDataHolder> Execute(OneExtractedValue lineData)
        {
            this.LookupSet.Add(lineData.ExtractedData);

            return this.LookupSet;
        }
    }
}
