////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
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
        /// <param name="lineData">The data to process.</param>
        /// <returns>A reference to the lookup data structure that was built so far.</returns>
        public Dictionary<string, string> Execute(ParsedLine lineData)
        {
            DataProcessorValidation.ValidateExtractedDataIsString(lineData);
            DataProcessorValidation.ValidateColumnInformation(lineData);

            string linePrefix = string.Join(lineData.ColumnSeparator, lineData.Columns, 0, lineData.ExtractedColumnNumber - 1);
            string lineSuffix = string.Join(lineData.ColumnSeparator, lineData.Columns, lineData.ExtractedColumnNumber, lineData.Columns.Length - lineData.ExtractedColumnNumber);
            string lineToJoin = linePrefix;
            if (String.IsNullOrEmpty(lineToJoin))
            {
                lineToJoin = lineSuffix;
            }
            else if (!String.IsNullOrEmpty(lineSuffix))
            {
                lineToJoin += lineData.ColumnSeparator + lineSuffix;
            }

            this.LookupDictionary.Add(lineData.ExtractedData.ToString(), lineToJoin);

            return this.LookupDictionary;
        }
    }
}
