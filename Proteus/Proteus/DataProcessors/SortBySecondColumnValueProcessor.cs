////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
///
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

using LaurentiuCristofor.Proteus.Common;
using LaurentiuCristofor.Proteus.Common.DataHolders;
using LaurentiuCristofor.Proteus.DataExtractors;
using LaurentiuCristofor.Proteus.DataProcessors.Parameters;
using LaurentiuCristofor.Proteus.FileOperations;

namespace LaurentiuCristofor.Proteus.DataProcessors
{
    /// <summary>
    /// A data processor that expects as input a file sorted by a primary column and will perform a secondary sort on a second column.
    /// </summary>
    public class SortBySecondColumnValueProcessor : BaseOutputProcessor, IDataProcessor<BaseOutputParameters, TwoExtractedValues>
    {
        /// <summary>
        /// Data structure used for loading the lines before sorting them.
        /// </summary>
        protected List<DataPair<IDataHolder, string>> ColumnLinePairs { get; set; }

        /// <summary>
        /// The value being currently processed for the primary sort column.
        /// </summary>
        protected IDataHolder CurrentPrimaryColumnData { get; set; }

        public void Initialize(BaseOutputParameters processingParameters)
        {
            ColumnLinePairs = new List<DataPair<IDataHolder, string>>();

            OutputWriter = new FileWriter(processingParameters.OutputFilePath);
        }

        public bool Execute(ulong lineNumber, TwoExtractedValues lineData)
        {
            // We will also execute these steps when processing the very first line, but nothing will be output.
            //
            if (!lineData.ExtractedData.Equals(CurrentPrimaryColumnData))
            {
                // Sort and output the lines we have collected so far for the CurrentPrimaryColumnData value.
                //
                ColumnLinePairs.Sort();

                foreach (DataPair<IDataHolder, string> dataPair in ColumnLinePairs)
                {
                    OutputWriter.WriteLine(dataPair.SecondData);
                }

                // Clear our tuples array - we'll start collecting a new set of rows.
                //
                ColumnLinePairs.Clear();

                // Verify that the input file is sorted on the primary column.
                //
                if (CurrentPrimaryColumnData != null && lineData.ExtractedData.CompareTo(CurrentPrimaryColumnData) < 0)
                {
                    throw new ProteusException($"Input file is not sorted as expected! Value '{lineData.ExtractedData}' succeeds value '{CurrentPrimaryColumnData}'.");
                }

                // Update CurrentPrimaryColumnData to the newly seen value.
                //
                CurrentPrimaryColumnData = lineData.ExtractedData;
            }

            ColumnLinePairs.Add(new DataPair<IDataHolder, string>(lineData.SecondExtractedData, lineData.OriginalLine));

            return true;
        }

        public override void CompleteExecution()
        {
            if (ColumnLinePairs == null)
            {
                throw new ProteusException("Internal error: An expected data structure has not been initialized!");
            }

            // Output the last remaining batch of lines.
            //
            ColumnLinePairs.Sort();

            foreach (DataPair<IDataHolder, string> dataPair in ColumnLinePairs)
            {
                OutputWriter.WriteLine(dataPair.SecondData);
            }

            base.CompleteExecution();
        }
    }
}
