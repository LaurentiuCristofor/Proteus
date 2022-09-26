////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;

using LaurentiuCristofor.Proteus.Common;
using LaurentiuCristofor.Proteus.DataExtractors;

namespace LaurentiuCristofor.Proteus.DataProcessors.Lookup
{
    /// <summary>
    /// A builder of a Dictionary data structure used for lookup by join operations.
    /// </summary>
    public class JoinBuilder : ILookupDataStructureBuilder<ParsedLine, Dictionary<DataTypeContainer, List<string>>>
    {
        /// <summary>
        /// The lookup data structure that we'll construct.
        /// </summary>
        protected Dictionary<DataTypeContainer, List<string>> LookupDictionary { get; set; }

        public JoinBuilder()
        {
            this.LookupDictionary = new Dictionary<DataTypeContainer, List<string>>();
        }

        /// <summary>
        /// Execute the builder on a unit of data.
        /// </summary>
        /// <param name="lineData">The data to process.</param>
        /// <returns>A reference to the lookup data structure that was built so far.</returns>
        public Dictionary<DataTypeContainer, List<string>> Execute(ParsedLine lineData)
        {
            DataProcessorValidation.ValidateExtractedDataIsString(lineData);
            DataProcessorValidation.ValidateColumnInformation(lineData);

            // The line that we want to join with should not contain the data that we matched on, to prevent redundancy in the join output.
            // We thus have to construct a new line without the content extracted in lineData.
            //
            string lineToJoin = lineData.AssembleWithoutColumn(lineData.ExtractedColumnNumber);

            DataTypeContainer lineKey = lineData.ExtractedData;

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
