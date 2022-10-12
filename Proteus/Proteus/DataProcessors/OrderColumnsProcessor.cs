////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
///
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

using LaurentiuCristofor.Proteus.Common;
using LaurentiuCristofor.Proteus.DataExtractors;
using LaurentiuCristofor.Proteus.DataProcessors.Parameters;
using LaurentiuCristofor.Proteus.FileOperations;

namespace LaurentiuCristofor.Proteus.DataProcessors
{
    /// <summary>
    /// A data processor that re-orders the columns of the input row.
    /// 
    /// OutputParameters is expected to contain:
    /// StringParameters[0] - new first columns list
    /// </summary>
    public class OrderColumnsProcessor : BaseOutputProcessor, IDataProcessor<OutputExtraParameters, ExtractedColumnStrings>
    {
        /// <summary>
        /// The new first columns list.
        /// </summary>
        protected string NewFirstColumnsList { get; set; }

        /// <summary>
        /// An array of column numbers that should be ordered first.
        /// </summary>
        protected int[] OrderedColumnNumbers { get; set; }

        /// <summary>
        /// The set of column numbers that should be ordered first, for quick lookup.
        /// </summary>
        protected HashSet<int> OrderedColumnNumbersSet { get; set; }

        /// <summary>
        /// The largest column number that we are ordering.
        /// </summary>
        protected int MaxOrderedColumnNumber { get; set; }

        /// <summary>
        /// The smallest column number that we are not ordering.
        /// </summary>
        protected int MinUnorderedColumnNumber { get; set; }

        public void Initialize(OutputExtraParameters processingParameters)
        {
            ArgumentChecker.CheckPresence(processingParameters.StringParameters, 0);
            this.NewFirstColumnsList = processingParameters.StringParameters[0];

            // Parse the column numbers that we need to place first.
            //
            string[] orderedColumnNumbersAsStrings = this.NewFirstColumnsList.Split(Constants.Strings.ListSeparator.ToCharArray(), StringSplitOptions.None);
            if (orderedColumnNumbersAsStrings.Length == 0)
            {
                throw new ProteusException("The expected list of column numbers is empty!");
            }
            this.OrderedColumnNumbers = new int[orderedColumnNumbersAsStrings.Length];
            this.OrderedColumnNumbersSet = new HashSet<int>();
            for (int i = 0; i < orderedColumnNumbersAsStrings.Length; ++i)
            {
                this.OrderedColumnNumbers[i] = int.Parse(orderedColumnNumbersAsStrings[i]);

                if (this.OrderedColumnNumbersSet.Contains(this.OrderedColumnNumbers[i]))
                {
                    throw new ProteusException($"Column number {this.OrderedColumnNumbers[i]} was specified twice!");
                }

                this.OrderedColumnNumbersSet.Add(this.OrderedColumnNumbers[i]);

                if (this.OrderedColumnNumbers[i] > this.MaxOrderedColumnNumber)
                {
                    this.MaxOrderedColumnNumber = this.OrderedColumnNumbers[i];
                }
            }

            // Find the smallest column number that was not specified in the ordering list.
            //
            for (int columnNumber = 1; columnNumber <= this.MaxOrderedColumnNumber; ++columnNumber)
            {
                if (!this.OrderedColumnNumbersSet.Contains(columnNumber))
                {
                    this.MinUnorderedColumnNumber = columnNumber;
                    break;
                }
            }

            // If no gap was found in the ordering list, set the smallest column number
            // to the number following the largest column number in the ordering list.
            //
            if (this.MinUnorderedColumnNumber == 0)
            {
                this.MinUnorderedColumnNumber = this.MaxOrderedColumnNumber + 1;
            }

            this.OutputWriter = new FileWriter(processingParameters.OutputFilePath);
        }

        public bool Execute(ulong lineNumber, ExtractedColumnStrings lineData)
        {
            int countColumns = lineData.Columns.Length;

            // If this line doesn't have enough columns, skip it.
            //
            if (countColumns < this.MaxOrderedColumnNumber)
            {
                return true;
            }

            string line = null;

            // First, pick the values of the new first columns.
            //
            for (int i = 0; i < this.OrderedColumnNumbers.Length; ++i)
            {
                int columnIndex = this.OrderedColumnNumbers[i] - 1;
                if (String.IsNullOrEmpty(line))
                {
                    line = lineData.Columns[columnIndex];
                }
                else
                {
                    line += $"{lineData.ColumnSeparator}{lineData.Columns[columnIndex]}";
                }
            }

            // Finally, pick the values of the remaining columns.
            //
            for (int columnIndex = this.MinUnorderedColumnNumber - 1; columnIndex < countColumns; ++columnIndex)
            {
                int columnNumber = columnIndex + 1;
                if (!this.OrderedColumnNumbersSet.Contains(columnNumber))
                {
                    line += $"{lineData.ColumnSeparator}{lineData.Columns[columnIndex]}";
                }
            }

            this.OutputWriter.WriteLine(line);

            return true;
        }
    }
}
