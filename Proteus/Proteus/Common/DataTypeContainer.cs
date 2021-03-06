﻿////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;

namespace LaurentiuCristofor.Proteus.Common
{
    /// <summary>
    /// A container of a string interpreted as a specific data type.
    /// This class can be used to compare strings in other orders than lexicographic.
    /// The data types supported are those defined in enum DataType.
    /// </summary>
    public class DataTypeContainer : IComparable
    {
        /// <summary>
        /// The data type of the string value.
        /// </summary>
        public DataType DataType { get; protected set; }

        /// <summary>
        /// The original string value.
        /// </summary>
        protected string StringValue { get; set; }

        /// <summary>
        /// A signed integer value.
        /// Only used if specified by DataType.
        /// </summary>
        protected long IntegerValue { get; set; }

        /// <summary>
        /// An unsigned integer value.
        /// Only used if specified by DataType.
        /// </summary>
        protected ulong UnsignedIntegerValue { get; set; }

        /// <summary>
        /// A floating point value.
        /// Only used if specified by DataType.
        /// </summary>
        protected double FloatingPointValue { get; set; }

        /// <summary>
        /// A DateTime value.
        /// Only used if specified by DataType.
        /// </summary>
        protected DateTime DateTimeValue { get; set; }

        public DataTypeContainer(DataType dataType)
        {
            if (dataType == DataType.NotSet)
            {
                throw new ProteusException("An unset data type value was used with DataTypeContainer!");
            }

            this.DataType = dataType;
        }

        public DataTypeContainer(DataType dataType, string stringValue)
        {
            if (dataType == DataType.NotSet)
            {
                throw new ProteusException("An unset data type value was used with DataTypeContainer!");
            }

            this.DataType = dataType;
            this.StringValue = stringValue;

            this.ParseStringValue();
        }

        /// <summary>
        /// Convert a string value to the data type indicated by the member field DataType.
        /// </summary>
        private void ParseStringValue()
        {
            switch (this.DataType)
            {
                case DataType.String:
                    // No parsing needed in this case.
                    //
                    break;

                case DataType.Integer:
                    long longValue;
                    if (!long.TryParse(this.StringValue, out longValue))
                    {
                        throw new ProteusException($"Invalid signed integer value: {this.StringValue}!");
                    }
                    this.IntegerValue = longValue;
                    break;

                case DataType.UnsignedInteger:
                    ulong ulongValue;
                    if (!ulong.TryParse(this.StringValue, out ulongValue))
                    {
                        throw new ProteusException($"Invalid unsigned integer value: {this.StringValue}!");
                    }
                    this.UnsignedIntegerValue = ulongValue;
                    break;

                case DataType.FloatingPoint:
                    double doubleValue;
                    if (!double.TryParse(this.StringValue, out doubleValue))
                    {
                        throw new ProteusException($"Invalid floating point value: {this.StringValue}!");
                    }
                    this.FloatingPointValue = doubleValue;
                    break;

                case DataType.DateTime:
                    DateTime dateTimeValue;
                    if (!DateTime.TryParse(this.StringValue, out dateTimeValue))
                    {
                        throw new ProteusException($"Invalid DateTime value: {this.StringValue}!");
                    }
                    this.DateTimeValue = dateTimeValue;
                    break;

                default:
                    throw new ProteusException($"Internal error: Proteus is not handling data type: {this.DataType}!");
            }
        }

        /// <summary>
        /// Sets and parses a new string value using the already set DataType.
        /// </summary>
        /// <param name="value">The value to set and parse.</param>
        public void ParseStringValue(string value)
        {
            this.StringValue = value;

            this.ParseStringValue();
        }

        /// <summary>
        /// Variant of ParseStringValue that does not throw exceptions.
        /// </summary>
        /// <param name="value">The value to set and parse.</param>
        /// <returns>True if the value was successfully parsed; false otherwise.</returns>
        public bool TryParseStringValue(string value)
        {
            bool hasSucceeded = false;

            // Store the old value, so we can restore it in case of failure.
            //
            string oldStringValue = this.StringValue;

            try
            {
                this.StringValue = value;
                this.ParseStringValue();
                hasSucceeded = true;
            }
            catch (ProteusException)
            {
                this.StringValue = oldStringValue;
            }

            return hasSucceeded;
        }

        /// <summary>
        /// Returns string representation of stored value.
        /// </summary>
        /// <returns>Original string representation of the data type.</returns>
        public override string ToString()
        {
            return this.StringValue;
        }

        /// <summary>
        /// Performs a type-specific comparison with another value, as indicated by the DataType value.
        /// </summary>
        /// <param name="otherContainer">The other DataTypeContainer value.</param>
        /// <returns>A negative value, zero, or a positive value if this data is less than, equal to, or greater than, respectively, the other value.</returns>
        public int CompareTo(DataTypeContainer otherContainer)
        {
            if (otherContainer.DataType != this.DataType)
            {
                throw new ProteusException($"Attempt to compare a container of type {this.DataType} with a container of type {otherContainer.DataType}!");
            }

            switch (this.DataType)
            {
                case DataType.String:
                    return this.StringValue.CompareTo(otherContainer.StringValue);

                case DataType.Integer:
                    return this.IntegerValue.CompareTo(otherContainer.IntegerValue);

                case DataType.UnsignedInteger:
                    return this.UnsignedIntegerValue.CompareTo(otherContainer.UnsignedIntegerValue);

                case DataType.FloatingPoint:
                    return this.FloatingPointValue.CompareTo(otherContainer.FloatingPointValue);

                case DataType.DateTime:
                    return this.DateTimeValue.CompareTo(otherContainer.DateTimeValue);

                default:
                    throw new ProteusException($"Internal error: Proteus is not handling data type: {this.DataType}!");
            }
        }

        public int CompareTo(object otherObject)
        {
            if (otherObject == null || !(otherObject is DataTypeContainer))
            {
                throw new ProteusException($"Attempt to compare container {this} with object {otherObject}!");
            }

            DataTypeContainer otherContainer = (DataTypeContainer)otherObject;
            return this.CompareTo(otherContainer);
        }

        public override bool Equals(Object otherObject)
        {
            int comparison;

            try
            {
                comparison = this.CompareTo(otherObject);
            }
            catch (ProteusException)
            {
                return false;
            }

            return comparison == 0;
        }

        public override int GetHashCode()
        {
            switch (this.DataType)
            {
                case DataType.String:
                    return this.StringValue.GetHashCode();

                case DataType.Integer:
                    return this.IntegerValue.GetHashCode();

                case DataType.UnsignedInteger:
                    return this.UnsignedIntegerValue.GetHashCode();

                case DataType.FloatingPoint:
                    return this.FloatingPointValue.GetHashCode();

                case DataType.DateTime:
                    return this.DateTimeValue.GetHashCode();

                default:
                    throw new ProteusException($"Internal error: Proteus is not handling data type: {this.DataType}!");
            }
        }

        /// <summary>
        /// Perform a comparison type against the provided arguments.
        /// </summary>
        /// <param name="comparisonType">The comparison type.</param>
        /// <param name="firstArgument">The first argument for the comparison.</param>
        /// <param name="secondArgument">The second argument for the comparison.</param>
        /// <returns>True if the comparison holds; false otherwise.</returns>
        public bool Compare(ComparisonType comparisonType, string firstArgument, string secondArgument)
        {
            switch (comparisonType)
            {
                case ComparisonType.LessThan:
                    return this.ThresholdCompare(comparisonType, firstArgument);

                case ComparisonType.LessThanOrEqual:
                    return this.ThresholdCompare(comparisonType, firstArgument);

                case ComparisonType.Equal:
                    return this.ThresholdCompare(comparisonType, firstArgument);

                case ComparisonType.GreaterThanOrEqual:
                    return this.ThresholdCompare(comparisonType, firstArgument);

                case ComparisonType.GreaterThan:
                    return this.ThresholdCompare(comparisonType, firstArgument);

                case ComparisonType.NotEqual:
                    return this.ThresholdCompare(comparisonType, firstArgument);

                case ComparisonType.Between:
                    return this.ThresholdCompare(comparisonType, firstArgument, secondArgument);

                case ComparisonType.StrictlyBetween:
                    return this.ThresholdCompare(comparisonType, firstArgument, secondArgument);

                case ComparisonType.NotBetween:
                    return this.ThresholdCompare(comparisonType, firstArgument, secondArgument);

                case ComparisonType.NotStrictlyBetween:
                    return this.ThresholdCompare(comparisonType, firstArgument, secondArgument);

                default:
                    throw new ProteusException($"Internal error: Proteus is not handling comparison type: {comparisonType}!");
            }
        }

        /// <summary>
        /// Perform a basic comparison against a threshold.
        /// </summary>
        /// <param name="comparisonType">The comparison type.</param>
        /// <param name="thresholdArgument">The threshold argument for the comparison.</param>
        /// <returns>True if the comparison holds; false otherwise.</returns>
        public bool ThresholdCompare(ComparisonType comparisonType, string thresholdArgument)
        {
            ArgumentChecker.CheckPresence(thresholdArgument);
            DataTypeContainer otherContainer = new DataTypeContainer(this.DataType, thresholdArgument);
            int comparisonResult = this.CompareTo(otherContainer);

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
                    throw new ProteusException($"Internal error: comparison type '{comparisonType}' is not a threshold comparison!");
            }
        }

        /// <summary>
        /// Perform a basic comparison against 2 thresholds.
        /// </summary>
        /// <param name="comparisonType">The comparison type.</param>
        /// <param name="firstThreshold">The first threshold argument for the comparison.</param>
        /// <param name="secondThreshold">The second threshold argument for the comparison.</param>
        /// <returns>True if the comparison holds; false otherwise.</returns>
        public bool ThresholdCompare(ComparisonType comparisonType, string firstThreshold, string secondThreshold)
        {
            ArgumentChecker.CheckPresence(firstThreshold);
            ArgumentChecker.CheckPresence(secondThreshold);
            DataTypeContainer lowerBoundContainer = new DataTypeContainer(this.DataType, firstThreshold);
            DataTypeContainer upperBoundContainer = new DataTypeContainer(this.DataType, secondThreshold);
            int lowerBoundComparisonResult = this.CompareTo(lowerBoundContainer);
            int upperBoundComparisonResult = this.CompareTo(upperBoundContainer);

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
                    throw new ProteusException($"Internal error: comparison type '{comparisonType}' is not a 2 threshold comparison!");
            }
        }
    }
}
