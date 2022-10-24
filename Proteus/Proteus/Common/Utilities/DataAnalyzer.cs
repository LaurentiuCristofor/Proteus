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
    /// <summary>
    /// A class that collects various statistics about a set of data passed in.
    /// 
    /// Analyzed data must be of the same type used to initialize the DataAnalyzer.
    /// Analysis happens in 3 steps:
    /// (1) Analyze() must be called on each data piece.
    /// (2) PostProcessAnalyzedData() must be called after all data was passed to analyze
    /// and before requesting analysis results.
    /// (3) OutputReport() must be called last, to obtain the analysis report.
    /// </summary>
    public class DataAnalyzer
    {
        /// <summary>
        /// The type of data that we'll analyze.
        /// </summary>
        public DataType DataType { get; protected set; }

        /// <summary>
        /// A limit on how many top and bottom values we should output.
        /// </summary>
        public int OutputLimit { get; protected set; }

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

        /// <summary>
        /// Creates a new DataAnalyzer instance.
        /// </summary>
        /// <param name="dataType">The type of data that we'll be analyzing.</param>
        /// <param name="outputLimit">How many top and bottom values should be output; if zero, all values will be output.</param>
        public DataAnalyzer(DataType dataType, int outputLimit = 0)
        {
            ArgumentChecker.CheckGreaterThanOrEqualTo(outputLimit, 0);

            DataType = dataType;
            OutputLimit = outputLimit;
            MapDataToCount = new Dictionary<IDataHolder, ulong>();
        }

        /// <summary>
        /// Analyzes a piece of data.
        /// </summary>
        /// <param name="data">Data to analyze.</param>
        public void Analyze(IDataHolder data)
        {
            if (ListCountedData != null || Entropy != 0.0)
            {
                throw new ProteusException($"DataAnalyzer::Analyze() cannot be called after DataAnalyzer::PostProcessAnalyzedData()!");
            }

            if (data.GetDataType() != DataType)
            {
                throw new ProteusException($"Incorrect input data: a DataAnalyzer of type {DataType} was called on data of type {data.GetDataType()}!");
            }

            TotalDataCount++;

            // Create a map entry if one doesn't already exist.
            //
            if (!MapDataToCount.ContainsKey(data))
            {
                MapDataToCount.Add(data, 0);
                UniqueDataCount++;
            }

            // Count data.
            //
            MapDataToCount[data] += 1;

            // Check if we need to update minimum data.
            //
            if (MinimumValue == null
                || data.CompareTo(MinimumValue) < 0)
            {
                MinimumValue = data;
            }

            // Check if we need to update maximum data.
            //
            if (MaximumValue == null
                || data.CompareTo(MaximumValue) > 0)
            {
                MaximumValue = data;
            }

            // Check if we need to update shortest string representation data.
            //
            if (ShortestStringRepresentation == null
                || data.ToString().Length < ShortestStringRepresentation.Length)
            {
                ShortestStringRepresentation = data.ToString();
            }

            // Check if we need to update longest string representation data.
            //
            if (LongestStringRepresentation == null
                || data.ToString().Length > LongestStringRepresentation.Length)
            {
                LongestStringRepresentation = data.ToString();
            }

            // Update total string representation length.
            //
            TotalStringRepresentationLength += data.ToString().Length;

            // Update total data length.
            //
            if (data.IsNumerical())
            {
                TotalValue += data.GetFloatingPointValue();
            }
        }

        /// <summary>
        /// This method must be called after analyzing all data, to complete the analysis.
        /// 
        /// Generates the list of counted data and orders it by count.
        /// Computes Shannon entropy for data.
        /// </summary>
        public void PostProcessAnalyzedData()
        {
            if (ListCountedData != null || Entropy != 0.0)
            {
                throw new ProteusException($"DataAnalyzer::PostProcessAnalyzedData() should not be called more than once!");
            }

            if ((ulong)MapDataToCount.Keys.Count != UniqueDataCount)
            {
                throw new ProteusException($"Internal error: the number of tracked values {MapDataToCount.Keys.Count} does not match the unique data count {UniqueDataCount}!");
            }

            // If there is no data, we're done.
            //
            if (TotalDataCount == 0)
            {
                return;
            }

            Timer timer = new Timer($"\n\n{Constants.Messages.AnalysisPostProcessingStart}", Constants.Messages.AnalysisPostProcessingEnd, countFinalLineEndings: 2);

            // Initialize a list into which to collect and sort our data.
            //
            ListCountedData = new List<Tuple<ulong, IDataHolder>>();

            // Collect our data into the list and also compute its entropy.
            //
            foreach (IDataHolder data in MapDataToCount.Keys)
            {
                ulong dataCount = MapDataToCount[data];

                ListCountedData.Add(new Tuple<ulong, IDataHolder>(dataCount, data));

                double dataProbability = (double)dataCount / TotalDataCount;
                Entropy += -dataProbability * Math.Log(dataProbability, 2);
            }

            // Sort the data in the list.
            //
            ListCountedData.Sort();

            timer.StopAndReport();
        }

        /// <summary>
        /// Outputs the results of the analyzer.
        /// </summary>
        public void OutputReport()
        {
            if (ListCountedData == null)
            {
                throw new ProteusException($"DataAnalyzer::OutputReport() should be called after DataAnalyzer::PostProcessAnalyzedData()!");
            }

            ILogger logger = LoggingManager.GetLogger();

            // If there is no data, we're done.
            //
            if (TotalDataCount == 0)
            {
                logger.LogWarning($"\n{Constants.Messages.NoDataFoundForAnalysis}");
                return;
            }

            // Output information on individual values first.
            //
            // If outputLimit is 0 or if the limit is greater or equal than half the unique count, we can output all values;
            // else we output just the first and last outputLimit values.
            //
            if (OutputLimit == 0 || 2UL * (ulong)OutputLimit >= UniqueDataCount)
            {
                foreach (var tuple in ListCountedData)
                {
                    OutputValueInformation(tuple);
                }
            }
            else
            {
                logger.OutputLine($"{Constants.Strings.Bottom}{Constants.Strings.NameValueSeparator}{OutputLimit}");

                for (int i = 0; i < OutputLimit; ++i)
                {
                    var tuple = ListCountedData[i];
                    OutputValueInformation(tuple);
                }

                logger.OutputLine($"{Constants.Strings.Top}{Constants.Strings.NameValueSeparator}{OutputLimit}");

                for (int i = ListCountedData.Count - OutputLimit; i < ListCountedData.Count; ++i)
                {
                    var tuple = ListCountedData[i];
                    OutputValueInformation(tuple);
                }
            }

            // Output the main statistics.
            //
            logger.OutputLine($"{Constants.Strings.Count}{Constants.Strings.NameValueSeparator}{TotalDataCount:N0}");
            logger.OutputLine($"{Constants.Strings.UniqueCount}{Constants.Strings.NameValueSeparator}{UniqueDataCount:N0}");
            logger.OutputLine($"{Constants.Strings.MinimumValue}{Constants.Strings.NameValueSeparator}{MinimumValue}");
            logger.OutputLine($"{Constants.Strings.MaximumValue}{Constants.Strings.NameValueSeparator}{MaximumValue}");
            if (DataHolderOperations.IsNumerical(DataType))
            {
                logger.OutputLine($"{Constants.Strings.AverageValue}{Constants.Strings.NameValueSeparator}{(double)TotalValue / TotalDataCount:N5}");
            }
            logger.OutputLine($"{Constants.Strings.ShortestStringRepresentation}{Constants.Strings.NameValueSeparator}{ShortestStringRepresentation}");
            logger.OutputLine($"{Constants.Strings.LongestStringRepresentation}{Constants.Strings.NameValueSeparator}{LongestStringRepresentation}");
            logger.OutputLine($"{Constants.Strings.AverageStringRepresentationLength}{Constants.Strings.NameValueSeparator}{(double)TotalStringRepresentationLength / TotalDataCount:N5}");
            logger.OutputLine($"{Constants.Strings.Entropy}{Constants.Strings.NameValueSeparator}{Entropy:N5}");
        }

        /// <summary>
        /// Outputs the information collected about a single data value.
        /// </summary>
        /// <param name="valueInformation">The information collected.</param>
        protected void OutputValueInformation(Tuple<ulong, IDataHolder> valueInformation)
        {
            // We'll display the count and corresponding percentile value.
            //
            ulong valueCount = valueInformation.Item1;
            string value = valueInformation.Item2.ToString();
            double valueRatio = (double)valueCount / (double)TotalDataCount;

            LoggingManager.GetLogger().OutputLine($"{valueCount:N0}{Constants.Strings.NameValueSeparator}{valueRatio:P5}{Constants.Strings.NameValueSeparator}{value}");
        }
    }
}
