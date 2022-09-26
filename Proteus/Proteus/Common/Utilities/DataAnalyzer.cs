////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

using LaurentiuCristofor.Proteus.Common.DataHolders;
using LaurentiuCristofor.Proteus.Common.Types;

namespace LaurentiuCristofor.Proteus.Common.Utilities
{
    public class DataAnalyzer
    {
        /// <summary>
        /// The type of data that we analyze.
        /// </summary>
        public DataType DataType;

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
        public IDataHolder MaximumData { get; protected set; }

        /// <summary>
        /// Tracks the minimum data value.
        /// </summary>
        public IDataHolder MinimumData { get; protected set; }

        /// <summary>
        /// For numerical values, tracks the total, for computing the average value.
        /// </summary>
        public double TotalData { get; protected set; }

        /// <summary>
        /// Tracks the data with the longest string representation.
        /// </summary>
        public string LongestString { get; protected set; }

        /// <summary>
        /// Tracks the data with the shortest string representation.
        /// </summary>
        public string ShortestString { get; protected set; }

        /// <summary>
        /// Tracks the total string length, for computing the average string length.
        /// </summary>
        public long TotalStringLength { get; protected set; }

        /// <summary>
        /// The Shannon entropy of the analyzed data.
        /// </summary>
        public double Entropy { get; protected set; }

        /// <summary>
        /// Tracks the number of times we have seen each data.
        /// </summary>
        protected Dictionary<IDataHolder, ulong> MapDataToCount { get; set; }

        /// <summary>
        /// Offers a view of the data, ordered by count of instances encountered.
        /// </summary>
        protected List<Tuple<ulong, IDataHolder>> ListCountedData { get; set; }

        public DataAnalyzer(DataType dataType)
        {
            this.DataType = dataType;
            this.MapDataToCount = new Dictionary<IDataHolder, ulong>();
        }

        /// <summary>
        /// Analyze a piece of data.
        /// </summary>
        /// <param name="data">Data to analyze.</param>
        public void AnalyzeData(IDataHolder data)
        {
            if (data.GetDataType() != this.DataType)
            {
                throw new ProteusException($"DataAnalyzer of type {this.DataType} was called on data of type {data.GetDataType()}!");
            }

            this.TotalDataCount++;

            // Create a map entry if one doesn't already exist.
            //
            if (!this.MapDataToCount.ContainsKey(data))
            {
                this.MapDataToCount.Add(data, 0);
                this.UniqueDataCount++;
            }

            // Count data.
            //
            this.MapDataToCount[data] += 1;

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
            if (this.LongestString == null
                || data.ToString().Length > this.LongestString.Length)
            {
                this.LongestString = data.ToString();
            }

            // Check if we need to update shortest data.
            //
            if (this.ShortestString == null
                || data.ToString().Length < this.ShortestString.Length)
            {
                this.ShortestString = data.ToString();
            }

            // Update total string length.
            //
            this.TotalStringLength += data.ToString().Length;

            // Update total data length.
            //
            if (this.DataType == DataType.Integer)
            {
                this.TotalData += data.GetIntegerValue();
            }
            else if (this.DataType == DataType.UnsignedInteger)
            {
                this.TotalData += data.GetUnsignedIntegerValue();
            }
            else if (this.DataType == DataType.FloatingPoint)
            {
                this.TotalData += data.GetFloatingPointValue();
            }
        }

        /// <summary>
        /// Generate the list of counted data and order it by count.
        /// Compute Shannon entropy for data.
        /// </summary>
        public void PostProcessAnalyzedData()
        {
            // Some sanity checks.
            //
            if (this.ListCountedData != null || this.Entropy != 0.0)
            {
                throw new ProteusException($"DataAnalyzer::PostProcessAnalyzedData() should not be called more than once!");
            }

            if ((ulong)this.MapDataToCount.Keys.Count != this.UniqueDataCount)
            {
                throw new ProteusException($"Internal error: the number of tracked values {this.MapDataToCount.Keys.Count} does not match the unique data count {this.UniqueDataCount}!");
            }

            // If there is no data, we're done.
            //
            if (this.TotalDataCount == 0)
            {
                return;
            }

            OutputInterface.Log("\nPost-processing analyzed data...");

            // Initialize a list into which to collect and sort our data.
            //
            this.ListCountedData = new List<Tuple<ulong, IDataHolder>>();

            // Collect our data into the list and also compute its entropy.
            //
            foreach (IDataHolder data in this.MapDataToCount.Keys)
            {
                ulong dataCount = this.MapDataToCount[data];

                this.ListCountedData.Add(new Tuple<ulong, IDataHolder>(dataCount, data));

                double dataProbability = (double)dataCount / this.TotalDataCount;
                this.Entropy += -dataProbability * Math.Log(dataProbability, 2);
            }

            // Sort the data in the list.
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
            if (this.DataType == DataType.Integer || this.DataType == DataType.UnsignedInteger || this.DataType == DataType.FloatingPoint)
            {
                OutputInterface.OutputLine($"{Constants.Strings.AverageValue}{Constants.Strings.NameValueSeparator}{(double)this.TotalData / this.TotalDataCount}");
            }
            OutputInterface.OutputLine($"{Constants.Strings.LongestString}{Constants.Strings.NameValueSeparator}{this.LongestString}");
            OutputInterface.OutputLine($"{Constants.Strings.ShortestString}{Constants.Strings.NameValueSeparator}{this.ShortestString}");
            OutputInterface.OutputLine($"{Constants.Strings.AverageStringLength}{Constants.Strings.NameValueSeparator}{(double)this.TotalStringLength / this.TotalDataCount}");
            OutputInterface.OutputLine($"{Constants.Strings.Entropy}{Constants.Strings.NameValueSeparator}{this.Entropy}");
        }

        /// <summary>
        /// Outputs the information collected about a single data value.
        /// </summary>
        /// <param name="valueInformation">The information collected.</param>
        private void OutputValueInformation(Tuple<ulong, IDataHolder> valueInformation)
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
