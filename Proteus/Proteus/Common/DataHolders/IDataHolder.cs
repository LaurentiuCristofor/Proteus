////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;

using LaurentiuCristofor.Proteus.Common.Types;

namespace LaurentiuCristofor.Proteus.Common.DataHolders
{
    /// <summary>
    /// An interface for data type representations.
    /// </summary>
    public interface IDataHolder: IComparable
    {
        /// <summary>
        /// Return the data type of this instance.
        /// </summary>
        DataType GetDataType();

        /// <summary>
        /// If the underlying value can be mapped to an integer, return it.
        /// Otherwise, throw an exception.
        /// </summary>
        long GetIntegerValue();

        /// <summary>
        /// If the underlying value can be mapped to an unsigned integer, return it.
        /// Otherwise, throw an exception.
        /// </summary>
        ulong GetUnsignedIntegerValue();

        /// <summary>
        /// If the underlying value can be mapped to a floating point value, return it.
        /// Otherwise, throw an exception.
        /// </summary>
        double GetFloatingPointValue();

        /// <summary>
        /// If the underlying value can be mapped to a DateTime value, return it.
        /// Otherwise, throw an exception.
        /// </summary>
        DateTime GetDateTimeValue();

        /// <summary>
        /// Returns string representation of stored value.
        /// </summary>
        /// <returns>String representation of the data type.</returns>
        string ToString();

        /// <summary>
        /// Implements addition on types that support it.
        /// </summary>
        /// <param name="otherData">The operation argument.</param>
        /// <returns>Returns the operation result.</returns>
        IDataHolder Add(IDataHolder otherData);

        /// <summary>
        /// Implements subtraction on types that support it.
        /// </summary>
        /// <param name="otherData">The operation argument.</param>
        /// <returns>Returns the operation result.</returns>
        IDataHolder Subtract(IDataHolder otherData);

        /// <summary>
        /// Implements multiplication on types that support it.
        /// </summary>
        /// <param name="otherData">The operation argument.</param>
        /// <returns>Returns the operation result.</returns>
        IDataHolder Multiply(IDataHolder otherData);

        /// <summary>
        /// Implements division on types that support it.
        /// </summary>
        /// <param name="otherData">The operation argument.</param>
        /// <returns>Returns the operation result.</returns>
        IDataHolder Divide(IDataHolder otherData);

        /// <summary>
        /// Performs a type-specific comparison with another data.
        /// </summary>
        /// <param name="otherData">The other IDataHolder value.</param>
        /// <returns>A negative value, zero, or a positive value if this data is less than, equal to, or greater than, respectively, the other value.</returns>
        int CompareTo(IDataHolder otherData);

        bool Equals(Object otherObject);

        int GetHashCode();
    }
}
