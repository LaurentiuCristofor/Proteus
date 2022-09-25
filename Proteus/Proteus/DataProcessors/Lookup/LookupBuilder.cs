﻿////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;

namespace LaurentiuCristofor.Proteus.DataProcessors.Lookup
{
    /// <summary>
    /// A builder of a HashSet&lt;string&gt; data structure, used for plain lookup operations.
    /// </summary>
    public class LookupBuilder : ILookupDataStructureBuilder<string, HashSet<string>>
    {
        /// <summary>
        /// The lookup data structure that we'll construct.
        /// </summary>
        protected HashSet<string> LookupSet { get; set; }

        public LookupBuilder()
        {
            this.LookupSet = new HashSet<string>();
        }

        /// <summary>
        /// Execute the builder on a unit of data.
        /// </summary>
        /// <param name="inputData">The data to process.</param>
        /// <returns>A reference to the lookup data structure that was built so far.</returns>
        public HashSet<string> Execute(string inputData)
        {
            this.LookupSet.Add(inputData);

            return this.LookupSet;
        }
    }
}
