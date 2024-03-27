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
    /// A class that calculates the conditional entropy between a set of value pairs.
    /// 
    /// Calculation happens in 3 steps:
    /// (1) Process() must be called on each data pair.
    /// (2) Calculate() must be called after all data was passed to Process()
    /// and before requesting calculation results.
    /// (3) OutputResult() must be called last.
    /// </summary>
    public class ConditionalEntropyCalculator
    {
        /// <summary>
        /// The first type of data that we process.
        /// </summary>
        public DataType FirstDataType { get; protected set; }

        /// <summary>
        /// The second type of data that we process.
        /// </summary>
        public DataType SecondDataType { get; protected set; }

        /// <summary>
        /// The total number of data pair instances analyzed.
        /// </summary>
        public ulong TotalDataCount { get; protected set; }

        /// <summary>
        /// Tracks the number of times we have seen each data pair.
        /// </summary>
        protected Dictionary<IDataHolder, Dictionary<IDataHolder, ulong>> MapFirstDataToSecondDataToCount { get; set; }

        /// <summary>
        /// The Shannon entropy of the analyzed data.
        /// </summary>
        public double Entropy { get; protected set; }

        /// <summary>
        /// Whether Calculate() has been called.
        /// </summary>
        protected bool HasCalculated { get; set; }

        /// <summary>
        /// Creates a new ConditionalEntropyCalculator instance.
        /// </summary>
        /// <param name="firstDataType">The type of the first data that we'll be analyzing.</param>
        /// <param name="secondDataType">The type of the second data that we'll be analyzing.</param>
        public ConditionalEntropyCalculator(DataType firstDataType, DataType secondDataType)
        {
            FirstDataType = firstDataType;
            SecondDataType = secondDataType;

            MapFirstDataToSecondDataToCount = new Dictionary<IDataHolder, Dictionary<IDataHolder, ulong>>();
        }

        /// <summary>
        /// Process a data pair.
        /// </summary>
        /// <param name="firstData">First data of the pair.</param>
        /// <param name="secondData">Second data of the pair.</param>
        public void Process(IDataHolder firstData, IDataHolder secondData)
        {
            if (HasCalculated)
            {
                throw new ProteusException($"ConditionalEntropyCalculator::Process() cannot be called after ConditionalEntropyCalculator::PostProcessData()!");
            }

            if (firstData.GetDataType() != FirstDataType
                || secondData.GetDataType() != SecondDataType)
            {
                throw new ProteusException($"Incorrect input data: a ConditionalEntropyCalculator of type ({FirstDataType}, {SecondDataType}) was called on a pair of data of type ({firstData.GetDataType()}, {secondData.GetDataType()})!");
            }

            TotalDataCount++;

            // Create a map entry if one doesn't already exist for the first data.
            //
            if (!MapFirstDataToSecondDataToCount.ContainsKey(firstData))
            {
                MapFirstDataToSecondDataToCount.Add(firstData, new Dictionary<IDataHolder, ulong>());
            }

            Dictionary<IDataHolder, ulong> mapSecondDataToCount = MapFirstDataToSecondDataToCount[firstData];

            // Create a map entry if one doesn't already exist for the second data.
            //
            if (!mapSecondDataToCount.ContainsKey(secondData))
            {
                mapSecondDataToCount.Add(secondData, 0);
            }

            // Count data pair.
            //
            mapSecondDataToCount[secondData] += 1;
        }

        /// <summary>
        /// This method must be called after processing all data, to calculate the conditional entropy.
        /// </summary>
        public void Calculate()
        {
            if (HasCalculated)
            {
                throw new ProteusException($"ConditionalEntropyCalculator::Calculate() should not be called more than once!");
            }

            HasCalculated= true;

            // If there is no data, we're done.
            //
            if (TotalDataCount == 0)
            {
                return;
            }

            Timer timer = new Timer($"\n\n{Constants.Messages.ConditionalEntropyCalculationStart}", Constants.Messages.ConditionalEntropyCalculationEnd, countFinalLineEndings: 2);

            // Collect our data into the list and also compute its entropy.
            //
            foreach (IDataHolder firstValue in MapFirstDataToSecondDataToCount.Keys)
            {
                Dictionary<IDataHolder, ulong> mapSecondDataToCount = MapFirstDataToSecondDataToCount[firstValue];

                // We could have computed this in the processing step, but it would have complicated the map definition.
                //
                ulong totalFirstValueCount = 0;
                foreach (ulong count in mapSecondDataToCount.Values)
                {
                    totalFirstValueCount += count;
                }

                // Compute the entropy for each pair of values.
                //
                double firstValueEntropy = 0.0;
                foreach (ulong dataPairCount in mapSecondDataToCount.Values)
                {
                    double dataPairProbability = (double)dataPairCount / totalFirstValueCount;
                    firstValueEntropy += -dataPairProbability * Math.Log(dataPairProbability, 2);
                }

                // We'll normalize the result by the size of each partition block;
                // i.e. by multiplying the entropy of each block with the size of the block
                // and then dividing by the size of the entire set.
                // We do the multiplication here and we'll do the division at the end.
                //
                Entropy += firstValueEntropy * totalFirstValueCount;
            }

            // Complete the normalization of the result.
            //
            Entropy /= TotalDataCount;

            timer.StopAndReport();
        }

        /// <summary>
        /// Outputs the result.
        /// </summary>
        public void OutputResult()
        {
            if (!HasCalculated)
            {
                throw new ProteusException($"ConditionalEntropyCalculator::OutputResult() should be called after ConditionalEntropyCalculator::Calculate()!");
            }

            ILogger logger = LoggingManager.GetLogger();

            // If there is no data, we're done.
            //
            if (TotalDataCount == 0)
            {
                logger.LogWarning($"\n{Constants.Messages.NoDataFoundForProcessing}");
                return;
            }

            // Output the main statistics.
            //
            logger.OutputLine($"{Constants.Strings.ConditionalEntropy}{Constants.Strings.NameValueSeparator}{Entropy:N5}");
        }
    }
}
