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
    /// A builder of a Dictionary data structure used for lookup by join operations.
    /// </summary>
    public class JoinBuilder : ILookupDataStructureBuilder<OneExtractedValue, Dictionary<IDataHolder, List<string>>>
    {
        /// <summary>
        /// The lookup data structure that we'll construct.
        /// </summary>
        protected Dictionary<IDataHolder, List<string>> LookupDictionary { get; set; }

        public JoinBuilder()
        {
            this.LookupDictionary = new Dictionary<IDataHolder, List<string>>();
        }

        public Dictionary<IDataHolder, List<string>> Execute(OneExtractedValue lineData)
        {
            // The line that we want to join with should not contain the data that we matched on, to prevent redundancy in the join output.
            // We thus have to construct a new line without the content extracted in lineData.
            //
            string lineToJoin = lineData.AssembleWithoutColumn(lineData.ExtractedColumnNumber);

            IDataHolder lineKey = lineData.ExtractedData;

            // If this is the first time we see this key, initalize a List<string>.
            //
            if (!this.LookupDictionary.ContainsKey(lineKey))
            {
                this.LookupDictionary.Add(lineKey, new List<string>());
            }

            this.LookupDictionary[lineKey].Add(lineToJoin);

            return this.LookupDictionary;
        }
    }
}
