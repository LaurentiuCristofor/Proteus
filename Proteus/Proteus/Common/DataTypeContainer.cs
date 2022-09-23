////////////////////////////////////////////////////////////////////////////////////////////////////
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

        /// <summary>
        /// String constructor.
        /// </summary>
        /// <param name="stringValue">Value to store.</param>
        public DataTypeContainer(string stringValue)
        {
            this.DataType = DataType.String;
            this.StringValue = stringValue;
        }

        /// <summary>
        /// Integer constructor.
        /// </summary>
        /// <param name="integerValue">Value to store.</param>
        public DataTypeContainer(int integerValue)
        {
            this.DataType = DataType.Integer;
            this.IntegerValue = integerValue;
            this.StringValue = integerValue.ToString();
        }

        /// <summary>
        /// Unsigned integer constructor.
        /// </summary>
        /// <param name="unsignedIntegerValue">Value to store.</param>
        public DataTypeContainer(ulong unsignedIntegerValue)
        {
            this.DataType = DataType.UnsignedInteger;
            this.UnsignedIntegerValue = unsignedIntegerValue;
            this.StringValue = unsignedIntegerValue.ToString();
        }

        /// <summary>
        /// Double constructor.
        /// </summary>
        /// <param name="floatingPointValue">Value to store.</param>
        public DataTypeContainer(double floatingPointValue)
        {
            this.DataType = DataType.FloatingPoint;
            this.FloatingPointValue = floatingPointValue;
            this.StringValue = floatingPointValue.ToString();
        }

        /// <summary>
        /// DateTime constructor.
        /// </summary>
        /// <param name="dateTimeValue">Value to store.</param>
        public DataTypeContainer(DateTime dateTimeValue)
        {
            this.DataType = DataType.DateTime;
            this.DateTimeValue = dateTimeValue;
            this.StringValue = dateTimeValue.ToString();
        }

        /// <summary>
        /// Constructor that takes type as argument along a string representation of the data.
        /// </summary>
        /// <param name="dataType">The type of the data.</param>
        /// <param name="stringValue">The string representation of the data.</param>
        private DataTypeContainer(DataType dataType, string stringValue)
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
        /// Sets back the string value to the default string representation of the value.
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
                    this.StringValue = longValue.ToString();
                    break;

                case DataType.UnsignedInteger:
                    ulong ulongValue;
                    if (!ulong.TryParse(this.StringValue, out ulongValue))
                    {
                        throw new ProteusException($"Invalid unsigned integer value: {this.StringValue}!");
                    }
                    this.UnsignedIntegerValue = ulongValue;
                    this.StringValue = ulongValue.ToString();
                    break;

                case DataType.FloatingPoint:
                    double doubleValue;
                    if (!double.TryParse(this.StringValue, out doubleValue))
                    {
                        throw new ProteusException($"Invalid floating point value: {this.StringValue}!");
                    }
                    this.FloatingPointValue = doubleValue;
                    this.StringValue = doubleValue.ToString();
                    break;

                case DataType.DateTime:
                    DateTime dateTimeValue;
                    if (!DateTime.TryParse(this.StringValue, out dateTimeValue))
                    {
                        throw new ProteusException($"Invalid DateTime value: {this.StringValue}!");
                    }
                    this.DateTimeValue = dateTimeValue;
                    this.StringValue = dateTimeValue.ToString();
                    break;

                default:
                    throw new ProteusException($"Internal error: Proteus is not handling data type: {this.DataType}!");
            }
        }

        /// <summary>
        /// Attempts to construct a DataTypeContainer without throwing exceptions.
        /// </summary>
        /// <param name="value">The value to set and parse.</param>
        /// <returns>True if the value was successfully parsed; false otherwise.</returns>
        public static DataTypeContainer TryParseStringValue(DataType dataType, string value)
        {
            try
            {
                DataTypeContainer container = new DataTypeContainer(dataType, value);
                return container;
            }
            catch (ProteusException)
            {
            }

            return null;
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
        /// Implements addition on types that support it.
        /// </summary>
        /// <param name="otherContainer">The operation argument.</param>
        /// <returns>Returns the updated container.</returns>
        public DataTypeContainer Add(DataTypeContainer otherContainer)
        {
            if (otherContainer.DataType != this.DataType)
            {
                throw new ProteusException($"Attempting to add a container of type {otherContainer.DataType} to a container of type {this.DataType}!");
            }

            switch (this.DataType)
            {
                case DataType.String:
                    this.StringValue += otherContainer.StringValue;
                    break;

                case DataType.Integer:
                    this.IntegerValue += otherContainer.IntegerValue;
                    this.StringValue = this.IntegerValue.ToString();
                    break;

                case DataType.UnsignedInteger:
                    this.UnsignedIntegerValue += otherContainer.UnsignedIntegerValue;
                    this.StringValue = this.UnsignedIntegerValue.ToString();
                    break;

                case DataType.FloatingPoint:
                    this.FloatingPointValue += otherContainer.FloatingPointValue;
                    this.StringValue = this.FloatingPointValue.ToString();
                    break;

                case DataType.DateTime:
                    throw new ProteusException($"Adding is not supported by data type: {this.DataType}!");

                default:
                    throw new ProteusException($"Internal error: Proteus is not handling data type: {this.DataType}!");
            }

            return this;
        }

        /// <summary>
        /// Implements subtraction on types that support it.
        /// </summary>
        /// <param name="otherContainer">The operation argument.</param>
        /// <returns>Returns the updated container.</returns>
        public DataTypeContainer Subtract(DataTypeContainer otherContainer)
        {
            if (otherContainer.DataType != this.DataType)
            {
                throw new ProteusException($"Attempting to subtract a container of type {otherContainer.DataType} from a container of type {this.DataType}!");
            }

            switch (this.DataType)
            {
                case DataType.Integer:
                    this.IntegerValue -= otherContainer.IntegerValue;
                    this.StringValue = this.IntegerValue.ToString();
                    break;

                case DataType.UnsignedInteger:
                    this.UnsignedIntegerValue -= otherContainer.UnsignedIntegerValue;
                    this.StringValue = this.UnsignedIntegerValue.ToString();
                    break;

                case DataType.FloatingPoint:
                    this.FloatingPointValue -= otherContainer.FloatingPointValue;
                    this.StringValue = this.FloatingPointValue.ToString();
                    break;

                case DataType.String:
                case DataType.DateTime:
                    throw new ProteusException($"Subtracting is not supported by data type: {this.DataType}!");

                default:
                    throw new ProteusException($"Internal error: Proteus is not handling data type: {this.DataType}!");
            }

            return this;
        }

        /// <summary>
        /// Implements multiplication on types that support it.
        /// </summary>
        /// <param name="otherContainer">The operation argument.</param>
        /// <returns>Returns the updated container.</returns>
        public DataTypeContainer Multiply(DataTypeContainer otherContainer)
        {
            if (otherContainer.DataType != this.DataType)
            {
                throw new ProteusException($"Attempting to multiply a container of type {otherContainer.DataType} with a container of type {this.DataType}!");
            }

            switch (this.DataType)
            {
                case DataType.Integer:
                    this.IntegerValue *= otherContainer.IntegerValue;
                    this.StringValue = this.IntegerValue.ToString();
                    break;

                case DataType.UnsignedInteger:
                    this.UnsignedIntegerValue *= otherContainer.UnsignedIntegerValue;
                    this.StringValue = this.UnsignedIntegerValue.ToString();
                    break;

                case DataType.FloatingPoint:
                    this.FloatingPointValue *= otherContainer.FloatingPointValue;
                    this.StringValue = this.FloatingPointValue.ToString();
                    break;

                case DataType.String:
                case DataType.DateTime:
                    throw new ProteusException($"Multiplying is not supported by data type: {this.DataType}!");

                default:
                    throw new ProteusException($"Internal error: Proteus is not handling data type: {this.DataType}!");
            }

            return this;
        }

        /// <summary>
        /// Implements division on types that support it.
        /// </summary>
        /// <param name="otherContainer">The operation argument.</param>
        /// <returns>Returns the updated container.</returns>
        public DataTypeContainer Divide(DataTypeContainer otherContainer)
        {
            if (otherContainer.DataType != this.DataType)
            {
                throw new ProteusException($"Attempting to divide a container of type {this.DataType} by a container of type {otherContainer.DataType}!");
            }

            switch (this.DataType)
            {
                case DataType.Integer:
                    this.IntegerValue /= otherContainer.IntegerValue;
                    this.StringValue = this.IntegerValue.ToString();
                    break;

                case DataType.UnsignedInteger:
                    this.UnsignedIntegerValue /= otherContainer.UnsignedIntegerValue;
                    this.StringValue = this.UnsignedIntegerValue.ToString();
                    break;

                case DataType.FloatingPoint:
                    this.FloatingPointValue /= otherContainer.FloatingPointValue;
                    this.StringValue = this.FloatingPointValue.ToString();
                    break;

                case DataType.String:
                case DataType.DateTime:
                    throw new ProteusException($"Multiplying is not supported by data type: {this.DataType}!");

                default:
                    throw new ProteusException($"Internal error: Proteus is not handling data type: {this.DataType}!");
            }

            return this;
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
                throw new ProteusException($"Attempting to compare a container of type {this.DataType} with a container of type {otherContainer.DataType}!");
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
                throw new ProteusException($"Attempting to compare container {this} with object {otherObject}!");
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
        protected bool ThresholdCompare(ComparisonType comparisonType, string thresholdArgument)
        {
            ArgumentChecker.CheckNotNull(thresholdArgument);
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
                    throw new ProteusException($"Internal error: Comparison type '{comparisonType}' is not a threshold comparison!");
            }
        }

        /// <summary>
        /// Perform a basic comparison against 2 thresholds.
        /// </summary>
        /// <param name="comparisonType">The comparison type.</param>
        /// <param name="firstThreshold">The first threshold argument for the comparison.</param>
        /// <param name="secondThreshold">The second threshold argument for the comparison.</param>
        /// <returns>True if the comparison holds; false otherwise.</returns>
        protected bool ThresholdCompare(ComparisonType comparisonType, string firstThreshold, string secondThreshold)
        {
            ArgumentChecker.CheckNotNull(firstThreshold);
            ArgumentChecker.CheckNotNull(secondThreshold);
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
                    throw new ProteusException($"Internal error: Comparison type '{comparisonType}' is not a 2 threshold comparison!");
            }
        }
    }
}
