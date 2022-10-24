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
    /// OutputExtraParameters is expected to contain:
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
            NewFirstColumnsList = processingParameters.StringParameters[0];
            ArgumentChecker.CheckNotNull(NewFirstColumnsList);

            // Parse the column numbers that we need to place first.
            //
            string[] orderedColumnNumbersAsStrings = NewFirstColumnsList.Split(Constants.Strings.ListSeparator.ToCharArray(), StringSplitOptions.None);
            if (orderedColumnNumbersAsStrings.Length == 0)
            {
                throw new ProteusException("The expected list of column numbers is empty!");
            }
            OrderedColumnNumbers = new int[orderedColumnNumbersAsStrings.Length];
            OrderedColumnNumbersSet = new HashSet<int>();
            for (int i = 0; i < orderedColumnNumbersAsStrings.Length; ++i)
            {
                OrderedColumnNumbers[i] = int.Parse(orderedColumnNumbersAsStrings[i]);

                if (OrderedColumnNumbersSet.Contains(OrderedColumnNumbers[i]))
                {
                    throw new ProteusException($"Column number {OrderedColumnNumbers[i]} was specified twice!");
                }

                OrderedColumnNumbersSet.Add(OrderedColumnNumbers[i]);

                if (OrderedColumnNumbers[i] > MaxOrderedColumnNumber)
                {
                    MaxOrderedColumnNumber = OrderedColumnNumbers[i];
                }
            }

            // Find the smallest column number that was not specified in the ordering list.
            //
            for (int columnNumber = 1; columnNumber <= MaxOrderedColumnNumber; ++columnNumber)
            {
                if (!OrderedColumnNumbersSet.Contains(columnNumber))
                {
                    MinUnorderedColumnNumber = columnNumber;
                    break;
                }
            }

            // If no gap was found in the ordering list, set the smallest column number
            // to the number following the largest column number in the ordering list.
            //
            if (MinUnorderedColumnNumber == 0)
            {
                MinUnorderedColumnNumber = MaxOrderedColumnNumber + 1;
            }

            OutputWriter = new FileWriter(processingParameters.OutputFilePath);
        }

        public bool Execute(ulong lineNumber, ExtractedColumnStrings lineData)
        {
            int countColumns = lineData.Columns.Length;

            // If this line doesn't have enough columns, skip it.
            //
            if (countColumns < MaxOrderedColumnNumber)
            {
                return true;
            }

            string line = null;

            // First, pick the values of the new first columns.
            //
            for (int i = 0; i < OrderedColumnNumbers.Length; ++i)
            {
                int columnIndex = OrderedColumnNumbers[i] - 1;
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
            for (int columnIndex = MinUnorderedColumnNumber - 1; columnIndex < countColumns; ++columnIndex)
            {
                int columnNumber = columnIndex + 1;
                if (!OrderedColumnNumbersSet.Contains(columnNumber))
                {
                    line += $"{lineData.ColumnSeparator}{lineData.Columns[columnIndex]}";
                }
            }

            OutputWriter.WriteLine(line);

            return true;
        }
    }
}
