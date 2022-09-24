////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

using LaurentiuCristofor.Proteus.Common;
using LaurentiuCristofor.Proteus.DataExtractors;

namespace LaurentiuCristofor.Proteus.DataProcessors.Lookup
{
    /// <summary>
    /// A builder of a Dictionary&lt;string, string&gt; data structure used for lookup by join operations.
    /// </summary>
    public class JoinBuilder : ILookupDataStructureBuilder<ParsedLine, Dictionary<string, string>>
    {
        /// <summary>
        /// The lookup data structure that we'll construct.
        /// </summary>
        private Dictionary<string, string> LookupDictionary { get; set; }

        public JoinBuilder()
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

            // The line that we want to join with should not contain the data that we matched on, to prevent redundancy in the join output.
            // We thus have to construct a new line without the content extracted in lineData.
            //
            string lineToJoin = LineAssembler.AssembleWithoutColumn(lineData.ColumnSeparator, lineData.Columns, lineData.ExtractedColumnNumber);

            this.LookupDictionary.Add(lineData.ExtractedData.ToString(), lineToJoin);

            return this.LookupDictionary;
        }
    }
}
