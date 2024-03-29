﻿////////////////////////////////////////////////////////////////////////////////////////////////////
/// (c) Laurentiu Cristofor
///
/// This file is part of the Proteus Library and is made available under the MIT license.
/// Do not use it if you have not received an associated LICENSE file.
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;

using LaurentiuCristofor.Proteus.Common.Types;

namespace LaurentiuCristofor.Proteus.Common.DataHolders
{
    public class UnsignedIntegerDataHolder : IDataHolder
    {
        protected ulong Data { get; set; }

        public UnsignedIntegerDataHolder(ulong data)
        {
            Data = data;
        }

        public DataType GetDataType()
        {
            return DataType.UnsignedInteger;
        }

        public bool IsNumerical()
        {
            return true;
        }

        public long GetIntegerValue()
        {
            throw new NotImplementedException();
        }

        public ulong GetUnsignedIntegerValue()
        {
            return Data;
        }

        public double GetFloatingPointValue()
        {
            return Data;
        }

        public DateTime GetDateTimeValue()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return Data.ToString();
        }

        public IDataHolder Add(IDataHolder otherData)
        {
            Data += otherData.GetUnsignedIntegerValue();
            return this;
        }

        public IDataHolder Subtract(IDataHolder otherData)
        {
            Data -= otherData.GetUnsignedIntegerValue();
            return this;
        }

        public IDataHolder Multiply(IDataHolder otherData)
        {
            Data *= otherData.GetUnsignedIntegerValue();
            return this;
        }

        public IDataHolder Divide(IDataHolder otherData)
        {
            Data /= otherData.GetUnsignedIntegerValue();
            return this;
        }

        public int CompareTo(IDataHolder otherData)
        {
            if (otherData == null)
            {
                throw new ProteusException("Attempting to compare with null!");
            }

            return Data.CompareTo(otherData.GetUnsignedIntegerValue());
        }

        public int CompareTo(object otherObject)
        {
            UnsignedIntegerDataHolder otherData = (UnsignedIntegerDataHolder)otherObject;
            return CompareTo(otherData);
        }

        public override bool Equals(Object otherObject)
        {
            if (otherObject == null)
            {
                return false;
            }

            int comparison;

            try
            {
                comparison = CompareTo(otherObject);
            }
            catch (ProteusException)
            {
                return false;
            }

            return comparison == 0;
        }

        public override int GetHashCode()
        {
            return Data.GetHashCode();
        }
    }
}
