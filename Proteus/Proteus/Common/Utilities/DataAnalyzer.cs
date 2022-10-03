////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// 
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

using LaurentiuCristofor.Proteus.Common.DataHolders;
using LaurentiuCristofor.Proteus.Common.Logging;
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
        /// Tracks the minimum data value.
        /// </summary>
        public IDataHolder MinimumValue { get; protected set; }

        /// <summary>
        /// Tracks the maximum data value.
        /// </summary>
        public IDataHolder MaximumValue { get; protected set; }

        /// <summary>
        /// For numerical values, tracks the total, for computing the average value.
        /// </summary>
        public double TotalValue { get; protected set; }

        /// <summary>
        /// Tracks the data with the shortest string representation.
        /// </summary>
        public string ShortestStringRepresentation { get; protected set; }

        /// <summary>
        /// Tracks the data with the longest string representation.
        /// </summary>
        public string LongestStringRepresentation { get; protected set; }

        /// <summary>
        /// Tracks the total string length, for computing the average string length.
        /// </summary>
        public long TotalStringRepresentationLength { get; protected set; }

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
        /// Analyzes a piece of data.
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

            // Check if we need to update minimum data.
            //
            if (this.MinimumValue == null
                || data.CompareTo(this.MinimumValue) < 0)
            {
                this.MinimumValue = data;
            }

            // Check if we need to update maximum data.
            //
            if (this.MaximumValue == null
                || data.CompareTo(this.MaximumValue) > 0)
            {
                this.MaximumValue = data;
            }

            // Check if we need to update shortest string representation data.
            //
            if (this.ShortestStringRepresentation == null
                || data.ToString().Length < this.ShortestStringRepresentation.Length)
            {
                this.ShortestStringRepresentation = data.ToString();
            }

            // Check if we need to update longest string representation data.
            //
            if (this.LongestStringRepresentation == null
                || data.ToString().Length > this.LongestStringRepresentation.Length)
            {
                this.LongestStringRepresentation = data.ToString();
            }

            // Update total string representation length.
            //
            this.TotalStringRepresentationLength += data.ToString().Length;

            // Update total data length.
            //
            if (data.IsNumerical())
            {
                this.TotalValue += data.GetFloatingPointValue();
            }
        }

        /// <summary>
        /// Generates the list of counted data and orders it by count.
        /// Computes Shannon entropy for data.
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

            Timer timer = new Timer($"\n\n{Constants.Messages.AnalysisPostProcessingStart}", Constants.Messages.AnalysisPostProcessingEnd, "\n");

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

            timer.StopAndReport();
        }

        /// <summary>
        /// Outputs the results of the analyzer.
        /// </summary>
        /// <param name="outputLimit">How many top and bottom values should be printed; if zero, all values will be output.</param>
        public void OutputReport(int outputLimit = 0)
        {
            ILogger logger = LoggingManager.GetLogger();

            if (outputLimit < 0)
            {
                throw new ProteusException("An invalid (negative) output limit was passed to DataAnalyzer.OutputReport()!");
            }

            // If there is no data, we're done.
            //
            if (this.TotalDataCount == 0)
            {
                logger.LogWarning($"\n{Constants.Messages.NoDataFoundForAnalysis}");
                return;
            }

            // Output information on individual values first.
            //
            // If outputLimit is 0 or if the limit is greater or equal than half the unique count, we can output all values;
            // else we output just the first and last outputLimit values.
            //
            if (outputLimit == 0 || 2UL * (ulong)outputLimit >= this.UniqueDataCount)
            {
                foreach (var tuple in this.ListCountedData)
                {
                    OutputValueInformation(tuple);
                }
            }
            else
            {
                logger.OutputLine($"{Constants.Strings.Bottom}{Constants.Strings.NameValueSeparator}{outputLimit}");

                for (int i = 0; i < outputLimit; ++i)
                {
                    var tuple = this.ListCountedData[i];
                    OutputValueInformation(tuple);
                }

                logger.OutputLine($"{Constants.Strings.Top}{Constants.Strings.NameValueSeparator}{outputLimit}");

                for (int i = this.ListCountedData.Count - outputLimit; i < this.ListCountedData.Count; ++i)
                {
                    var tuple = this.ListCountedData[i];
                    OutputValueInformation(tuple);
                }
            }

            // Output the main statistics.
            //
            logger.OutputLine($"{Constants.Strings.Count}{Constants.Strings.NameValueSeparator}{this.TotalDataCount:N0}");
            logger.OutputLine($"{Constants.Strings.UniqueCount}{Constants.Strings.NameValueSeparator}{this.UniqueDataCount:N0}");
            logger.OutputLine($"{Constants.Strings.MinimumValue}{Constants.Strings.NameValueSeparator}{this.MinimumValue}");
            logger.OutputLine($"{Constants.Strings.MaximumValue}{Constants.Strings.NameValueSeparator}{this.MaximumValue}");
            if (DataHolderOperations.IsNumerical(this.DataType))
            {
                logger.OutputLine($"{Constants.Strings.AverageValue}{Constants.Strings.NameValueSeparator}{(double)this.TotalValue / this.TotalDataCount:N5}");
            }
            logger.OutputLine($"{Constants.Strings.ShortestStringRepresentation}{Constants.Strings.NameValueSeparator}{this.ShortestStringRepresentation}");
            logger.OutputLine($"{Constants.Strings.LongestStringRepresentation}{Constants.Strings.NameValueSeparator}{this.LongestStringRepresentation}");
            logger.OutputLine($"{Constants.Strings.AverageStringRepresentationLength}{Constants.Strings.NameValueSeparator}{(double)this.TotalStringRepresentationLength / this.TotalDataCount:N5}");
            logger.OutputLine($"{Constants.Strings.Entropy}{Constants.Strings.NameValueSeparator}{this.Entropy:N5}");
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
            double valueRatio = (double)valueCount / (double)this.TotalDataCount;

            LoggingManager.GetLogger().OutputLine($"{valueCount:N0}{Constants.Strings.NameValueSeparator}{valueRatio:P5}{Constants.Strings.NameValueSeparator}{value}");
        }
    }
}
