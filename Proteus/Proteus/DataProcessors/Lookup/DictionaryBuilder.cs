////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;

using LaurentiuCristofor.Proteus.DataExtractors;

namespace LaurentiuCristofor.Proteus.DataProcessors.Lookup
{
    /// <summary>
    /// A builder of a Dictionary&lt;string, string&gt; data structure.
    /// </summary>
    public class DictionaryBuilder : ILookupDataStructureBuilder<ParsedLine, Dictionary<string, string>>
    {
        /// <summary>
        /// The lookup data structure that we'll construct.
        /// </summary>
        private Dictionary<string, string> LookupDictionary { get; set; }

        public DictionaryBuilder()
        {
            this.LookupDictionary = new Dictionary<string, string>();
        }

        /// <summary>
        /// Execute the builder on a unit of data.
        /// </summary>
        /// <param name="inputData">The data to process.</param>
        /// <returns>A reference to the lookup data structure that was built so far.</returns>
        public Dictionary<string, string> Execute(ParsedLine inputData)
        {
            DataProcessorValidation.ValidateExtractedDataIsString(inputData);
            DataProcessorValidation.ValidateOriginalLine(inputData);

            this.LookupDictionary.Add(inputData.ExtractedData.ToString(), inputData.OriginalLine);

            return this.LookupDictionary;
        }
    }
}
