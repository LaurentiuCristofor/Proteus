////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// 
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;

using LaurentiuCristofor.Proteus.Common.Types;

namespace LaurentiuCristofor.Proteus.Common.DataHolders
{
    /// <summary>
    /// A class providing static functions that operate on IDataHolder instances.
    /// </summary>
    public abstract class DataHolderOperations
    {
        /// <summary>
        /// Interprets a string value as the specified data type
        /// and returns an instance of IDataHolder representing the equivalent value of that type.
        /// </summary>
        /// <param name="dataType">The data type to interpret the string as.</param>
        /// <param name="stringValue">The string to interpret.</param>
        /// <returns>An instance of IDataHolder, or null if the string did not represent a value of the specified data type.</returns>
        public static IDataHolder BuildDataHolder(DataType dataType, string stringValue)
        {
            switch (dataType)
            {
                case DataType.String:
                    return new StringDataHolder(stringValue);

                case DataType.Integer:
                    if (!long.TryParse(stringValue, out long longValue))
                    {
                        return null;
                    }
                    return new IntegerDataHolder(longValue);

                case DataType.UnsignedInteger:
                    if (!ulong.TryParse(stringValue, out ulong ulongValue))
                    {
                        return null;
                    }
                    return new UnsignedIntegerDataHolder(ulongValue);

                case DataType.FloatingPoint:
                    if (!double.TryParse(stringValue, out double doubleValue))
                    {
                        return null;
                    }
                    return new FloatingPointDataHolder(doubleValue);

                case DataType.DateTime:
                    if (!DateTime.TryParse(stringValue, out DateTime dateTimeValue))
                    {
                        return null;
                    }
                    return new DateTimeDataHolder(dateTimeValue);

                default:
                    throw new ProteusException($"Internal error: Proteus is not handling data type: {dataType}!");
            }
        }

        /// <summary>
        /// Returns whether a specified data type is a numerical one.
        /// </summary>
        /// <param name="dataType">The data type.</param>
        /// <returns>True if the data type is numerical; false otherwise.</returns>
        public static bool IsNumerical(DataType dataType)
        {
            return (dataType == DataType.Integer || dataType == DataType.UnsignedInteger || dataType == DataType.FloatingPoint);
        }

        /// <summary>
        /// Performs a comparison of data against the provided arguments.
        /// </summary>
        /// <param name="data">The data to compare.</param>
        /// <param name="comparisonType">The comparison type.</param>
        /// <param name="firstArgument">The first argument for the comparison.</param>
        /// <param name="secondArgument">The second argument for the comparison.</param>
        /// <returns>True if the comparison holds; false otherwise.</returns>
        public static bool Compare(IDataHolder data, ComparisonType comparisonType, string firstArgument, string secondArgument)
        {
            switch (comparisonType)
            {
                case ComparisonType.LessThan:
                    return ThresholdCompare(data, comparisonType, firstArgument);

                case ComparisonType.LessThanOrEqual:
                    return ThresholdCompare(data, comparisonType, firstArgument);

                case ComparisonType.Equal:
                    return ThresholdCompare(data, comparisonType, firstArgument);

                case ComparisonType.GreaterThanOrEqual:
                    return ThresholdCompare(data, comparisonType, firstArgument);

                case ComparisonType.GreaterThan:
                    return ThresholdCompare(data, comparisonType, firstArgument);

                case ComparisonType.NotEqual:
                    return ThresholdCompare(data, comparisonType, firstArgument);

                case ComparisonType.Between:
                    return ThresholdCompare(data, comparisonType, firstArgument, secondArgument);

                case ComparisonType.StrictlyBetween:
                    return ThresholdCompare(data, comparisonType, firstArgument, secondArgument);

                case ComparisonType.NotBetween:
                    return ThresholdCompare(data, comparisonType, firstArgument, secondArgument);

                case ComparisonType.NotStrictlyBetween:
                    return ThresholdCompare(data, comparisonType, firstArgument, secondArgument);

                default:
                    throw new ProteusException($"Internal error: Proteus is not handling comparison type: {comparisonType}!");
            }
        }

        /// <summary>
        /// Performs a comparison of data against a threshold.
        /// </summary>
        /// <param name="data">The data to compare.</param>
        /// <param name="comparisonType">The comparison type.</param>
        /// <param name="threshold">The threshold argument for the comparison.</param>
        /// <returns>True if the comparison holds; false otherwise.</returns>
        protected static bool ThresholdCompare(IDataHolder data, ComparisonType comparisonType, string threshold)
        {
            ArgumentChecker.CheckNotNull(threshold);

            IDataHolder thresholdData = BuildDataHolder(data.GetDataType(), threshold);
            if (thresholdData == null)
            {
                throw new ProteusException($"Comparison argument '{threshold}' is not a valid {data.GetDataType()} value!");
            }

            int comparisonResult = data.CompareTo(thresholdData);
            switch (comparisonType)
            {
                case ComparisonType.LessThan:
                    return comparisonResult < 0;

                case ComparisonType.LessThanOrEqual:
                    return comparisonResult <= 0;

                case ComparisonType.Equal:
                    return comparisonResult == 0;

                case ComparisonType.GreaterThanOrEqual:
                    return comparisonResult >= 0;

                case ComparisonType.GreaterThan:
                    return comparisonResult > 0;

                case ComparisonType.NotEqual:
                    return comparisonResult != 0;

                default:
                    throw new ProteusException($"Internal error: Comparison type '{comparisonType}' is not a threshold comparison!");
            }
        }

        /// <summary>
        /// Performs a comparison of data against 2 thresholds.
        /// </summary>
        /// <param name="data">The data to compare.</param>
        /// <param name="comparisonType">The comparison type.</param>
        /// <param name="firstThreshold">The first threshold argument for the comparison.</param>
        /// <param name="secondThreshold">The second threshold argument for the comparison.</param>
        /// <returns>True if the comparison holds; false otherwise.</returns>
        protected static bool ThresholdCompare(IDataHolder data, ComparisonType comparisonType, string firstThreshold, string secondThreshold)
        {
            ArgumentChecker.CheckNotNull(firstThreshold);
            ArgumentChecker.CheckNotNull(secondThreshold);

            IDataHolder lowerBound = BuildDataHolder(data.GetDataType(), firstThreshold);
            IDataHolder upperBound = BuildDataHolder(data.GetDataType(), secondThreshold);

            if (lowerBound == null)
            {
                throw new ProteusException($"Comparison argument '{firstThreshold}' is not a valid {data.GetDataType()} value!");
            }
            if (upperBound == null)
            {
                throw new ProteusException($"Comparison argument '{secondThreshold}' is not a valid {data.GetDataType()} value!");
            }

            ArgumentChecker.CheckInterval(lowerBound, upperBound);

            int lowerBoundComparisonResult = data.CompareTo(lowerBound);
            int upperBoundComparisonResult = data.CompareTo(upperBound);

            switch (comparisonType)
            {
                case ComparisonType.Between:
                    {
                        return lowerBoundComparisonResult >= 0 && upperBoundComparisonResult <= 0;
                    }

                case ComparisonType.StrictlyBetween:
                    {
                        return lowerBoundComparisonResult > 0 && upperBoundComparisonResult < 0;
                    }

                case ComparisonType.NotBetween:
                    {
                        return lowerBoundComparisonResult < 0 || upperBoundComparisonResult > 0;
                    }

                case ComparisonType.NotStrictlyBetween:
                    {
                        return lowerBoundComparisonResult <= 0 || upperBoundComparisonResult >= 0;
                    }

                default:
                    throw new ProteusException($"Internal error: Comparison type '{comparisonType}' is not a two-threshold comparison!");
            }
        }
    }
}
