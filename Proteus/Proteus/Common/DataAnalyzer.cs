////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;

namespace LaurentiuCristofor.Proteus.Common
{
    public class DataAnalyzer
    {
        /// <summary>
        /// Tracks the number of times we have seen each data.
        /// </summary>
        public Dictionary<DataTypeContainer, ulong> MapDataCounters { get; protected set; }

        /// <summary>
        /// Offers a view of the data, ordered by count of instances encountered.
        /// </summary>
        public List<Tuple<ulong, DataTypeContainer>> ListCountedData { get; protected set; }

        /// <summary>
        /// The total number of data instances analyzed.
        /// </summary>
        public ulong TotalDataCount { get; protected set; }

        /// <summary>
        /// The number of unique data instances analyzed.
        /// </summary>
        public ulong UniqueDataCount { get; protected set; }

        /// <summary>
        /// Tracks the maximum data value.
        /// </summary>
        public DataTypeContainer MaximumData { get; protected set; }

        /// <summary>
        /// Tracks the minimum data value.
        /// </summary>
        public DataTypeContainer MinimumData { get; protected set; }

        /// <summary>
        /// Tracks the data with the longest string representation.
        /// </summary>
        public DataTypeContainer LongestData { get; protected set; }

        /// <summary>
        /// Tracks the data with the shortest string representation.
        /// </summary>
        public DataTypeContainer ShortestData { get; protected set; }

        public DataAnalyzer()
        {
            this.MapDataCounters = new Dictionary<DataTypeContainer, ulong>();
            this.ListCountedData = new List<Tuple<ulong, DataTypeContainer>>();
        }

        /// <summary>
        /// Analyze a piece of data.
        /// </summary>
        /// <param name="data">Data to analyze.</param>
        public void AnalyzeData(DataTypeContainer data)
        {
            this.TotalDataCount++;

            // Create a map entry if one doesn't already exist.
            //
            if (!this.MapDataCounters.ContainsKey(data))
            {
                this.MapDataCounters.Add(data, 0);
                this.UniqueDataCount++;
            }

            // Count data.
            //
            this.MapDataCounters[data] += 1;

            // Check if we need to update maximum data.
            //
            if (this.MaximumData == null
                || data.CompareTo(this.MaximumData) > 0)
            {
                this.MaximumData = data;
            }

            // Check if we need to update minimum data.
            //
            if (this.MinimumData == null
                || data.CompareTo(this.MinimumData) < 0)
            {
                this.MinimumData = data;
            }

            // Check if we need to update longest data.
            //
            if (this.LongestData == null
                || data.ToString().Length > this.LongestData.ToString().Length)
            {
                this.LongestData = data;
            }

            // Check if we need to update shortest data.
            //
            if (this.ShortestData == null
                || data.ToString().Length < this.ShortestData.ToString().Length)
            {
                this.ShortestData = data;
            }
        }

        /// <summary>
        /// Generate the list of counted data and order it by count.
        /// </summary>
        public void OrderAnalyzedData()
        {
            // A quick sanity check.
            //
            if ((ulong)this.MapDataCounters.Keys.Count != this.UniqueDataCount)
            {
                throw new ProteusException($"Internal error: the number of tracked values {this.MapDataCounters.Keys.Count} does not match the unique data count {this.UniqueDataCount}!");
            }

            // If there is no data, we're done.
            //
            if (this.TotalDataCount == 0)
            {
                return;
            }

            OutputInterface.Log("\nSorting analyzed data...");

            // First we collect the data in a list.
            //
            foreach (DataTypeContainer data in this.MapDataCounters.Keys)
            {
                ulong dataCount = this.MapDataCounters[data];
                this.ListCountedData.Add(new Tuple<ulong, DataTypeContainer>(dataCount, data));
            }

            // Then we sort the data.
            //
            this.ListCountedData.Sort();

            OutputInterface.LogLine("done!\n");
        }

        /// <summary>
        /// Output the results of the analyzer.
        /// </summary>
        /// <param name="valuesLimit">How many top and bottom values should be printed; if zero, all values will be output.</param>
        public void OutputReport(int valuesLimit = 0)
        {
            if (valuesLimit < 0)
            {
                throw new ProteusException("An invalid (negative) values limit was passed to DataAnalyzer.OutputReport()!");
            }

            // If there is no data, we're done.
            //
            if (this.TotalDataCount == 0)
            {
                OutputInterface.LogLine("\nNo data was found to analyze!");
                return;
            }

            // Output information on individual values first.
            //
            // If limitCount is 0 or if the limit is greater or equal than half the total count, we can output all values;
            // else we output just the first and last limitCount values.
            //
            if (valuesLimit == 0 || 2UL * (ulong)valuesLimit >= this.TotalDataCount)
            {
                foreach (var tuple in this.ListCountedData)
                {
                    OutputValueInformation(tuple);
                }
            }
            else
            {
                OutputInterface.OutputLine($"{Constants.Strings.Bottom}{Constants.Strings.NameValueSeparator}{valuesLimit}");

                for (int i = 0; i < valuesLimit; i++)
                {
                    var tuple = this.ListCountedData[i];
                    OutputValueInformation(tuple);
                }

                OutputInterface.OutputLine($"{Constants.Strings.Top}{Constants.Strings.NameValueSeparator}{valuesLimit}");

                for (int i = this.ListCountedData.Count - valuesLimit; i < this.ListCountedData.Count; i++)
                {
                    var tuple = this.ListCountedData[i];
                    OutputValueInformation(tuple);
                }
            }

            // Output the main statistics.
            //
            OutputInterface.OutputLine($"{Constants.Strings.Count}{Constants.Strings.NameValueSeparator}{this.TotalDataCount}");
            OutputInterface.OutputLine($"{Constants.Strings.UniqueCount}{Constants.Strings.NameValueSeparator}{this.UniqueDataCount}");
            OutputInterface.OutputLine($"{Constants.Strings.MaximumValue}{Constants.Strings.NameValueSeparator}{this.MaximumData}");
            OutputInterface.OutputLine($"{Constants.Strings.MinimumValue}{Constants.Strings.NameValueSeparator}{this.MinimumData}");
            OutputInterface.OutputLine($"{Constants.Strings.LongestValue}{Constants.Strings.NameValueSeparator}{this.LongestData}");
            OutputInterface.OutputLine($"{Constants.Strings.ShortestValue}{Constants.Strings.NameValueSeparator}{this.ShortestData}");
        }

        /// <summary>
        /// Outputs the information collected about a single data value.
        /// </summary>
        /// <param name="valueInformation">The information collected.</param>
        private void OutputValueInformation(Tuple<ulong, DataTypeContainer> valueInformation)
        {
            // We'll display the count and corresponding percentile value.
            //
            ulong valueCount = valueInformation.Item1;
            string value = valueInformation.Item2.ToString();
            double valuePercentage = (double)valueCount / (double)this.TotalDataCount;

            OutputInterface.OutputLine($"{value}{Constants.Strings.NameValueSeparator}{valueCount}{Constants.Strings.NameValueSeparator}{valuePercentage}");
        }
    }
}
